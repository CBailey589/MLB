import requests
from bs4 import BeautifulSoup
import re
import math
import datetime
from datetime import date
import pyodbc

# TeamDictionary will hold the VegasInsiderName as the key and the DB TeamId as the value for each team
teamDictionary = {}
# Create connection to SQL DB
conn = pyodbc.connect('Driver={SQL Server};'
                      'Server=DESKTOP-IL1934L\SQLEXPRESS;'
                      'Database=MLB;'
                      'Trusted_Connection=yes;')
cursor = conn.cursor()
# Gets Teams table from DB and add teams to TeamDictionary
cursor.execute('''SELECT *
                FROM MLB.dbo.Teams''')
for team in cursor:
    teamDictionary[team[2]] = team[0]


# launch url
url = "http://www.vegasinsider.com/mlb/odds/las-vegas/"
response = requests.get(url)
# partse response.txt as BeautifulSoup obj
html_soup = BeautifulSoup(response.text, 'html.parser')
type(html_soup)

# make lists to hold scraped data:
gameDateAndTimeList = []
teamList = []
gameMoneyLine = []
gameStartingPitchers = []

# oddsTableGameSquares represents the first square in rows of odds table that contain the date/start time in red text
oddsTableGameSquares = html_soup.find_all("span", {"class": "cellTextHot"})
for square in oddsTableGameSquares:
    # Scrapes strings that contain a date formatted like MM/DD and a time like HH:MM PM in Eastern Time
    gameDateAndTimeList.append(square.get_text(strip=True))

    # Finds both team divs inside of the row represented by this game square, and strips the inner text out
    teamsInCurrentGame = square.findParent("td").find_all("a")
    for teamLine in teamsInCurrentGame:
        teamList.append(teamLine.get_text(strip=True))

    # Finds the opening Over/Under & respective moneyline odds for each team
    moneyLineSquare = square.findParent("tr").find_all("td")[1]
    gameMoneyLine.append(moneyLineSquare.get_text(strip=True))


# finds all <b> in the odds table that list the projected pitchers for each team
oddsTableStartingPitcherBTags = html_soup.find_all("b", text="Pitchers:")
for BTag in oddsTableStartingPitcherBTags:
    # finds the two starting pitchers for the game:
    gameStartingPitchers.append(BTag.findNext(
        "b").get_text(strip=True).replace("  ", " "))
    gameStartingPitchers.append(BTag.findNext("b").findNext(
        "b").get_text(strip=True).replace("  ", " "))


# used to scrape only games for today because those days should have opening lines
todaysDate = str(date.today()).replace("-", "/")[-5:]
# used to insert into the date string so strptime does not replace the year with 1900 when a year is not specified
currentYear = str(date.today())[:4]


for idx in range(len(teamList)):
    # used as an index to match up short lists [gameDateAndTimeList & gameMoneyLine] with long lists [teamList]
    gameNum = math.ceil(idx/2)
    shortListIndex = gameNum - 1
    # used to check against current date
    gameDate = gameDateAndTimeList[shortListIndex][:5].strip()
    gameDateTime = gameDateAndTimeList[shortListIndex].strip()
    # makes sure to only run code once per two teams (for each game), if there is a full set of moneyline odds with run total over/under, and game date is same as todays date
    if (idx % 2 != 0 and len(gameMoneyLine[shortListIndex]) > 8 and todaysDate == gameDate):
        # get the DB id's of the teams to use in sql statement to see if game is already in the DB:
        awayTeam = teamList[idx - 1]
        awayTeamDBId = teamDictionary.get(awayTeam)
        homeTeam = teamList[idx]
        homeTeamDBId = teamDictionary.get(homeTeam)

        # creates a date object for first game first pitch
        gameDateObj = datetime.datetime.strptime(currentYear + '/' + gameDateTime, '%Y/%m/%d  %I:%M %p')

        # Execute a SELECT from the Games table to see if the game already exists, if it does NOT, then insert the game into the table:
        cursor.execute('''SELECT *
                        FROM MLB.dbo.Games g
                        WHERE g.AwayTeamId = (?)
                        AND g.HomeTeamId = (?)
                        AND g.FirstPitchDateTime = (?)''', (awayTeamDBId, homeTeamDBId, gameDateObj))
        if (cursor.fetchone() == None):
            # Get remaining info for DB columns and store in variables
            awayLine = gameMoneyLine[shortListIndex][-8:-4]
            awaySP = gameStartingPitchers[idx - 1]
            homeLine = gameMoneyLine[shortListIndex][-4:]
            homeSP = gameStartingPitchers[idx]
            cursor.execute('''INSERT INTO MLB.dbo.Games
                            (FirstPitchDateTime, AwayTeamId, AwayLine, AwaySP, HomeTeamId, HomeLine, HomeSP, AwayScore, HomeScore, GameStarted, GameComplete, Inning)
                            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)''', (gameDateObj, awayTeamDBId, awayLine, awaySP, homeTeamDBId, homeLine, homeSP, 0, 0, 0, 0, ""))
            cursor.commit()
conn.close()