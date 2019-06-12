from bs4 import BeautifulSoup
from selenium import webdriver
import re
import math
import datetime
import time
from datetime import date, timedelta
import pyodbc
import pandas as pd

# GameDictionary will hold the DB game Id as the key and a 3 element array of the date and MLB.com name for the home and away team.
DBGames = {}

# ScoreBoardGames will hold the games from the MLB scoreboard for comparisson to the DBGames
ScoreBoardGames = {}

# Create connection to SQL DB
conn = pyodbc.connect('Driver={SQL Server};'
                      'Server=DESKTOP-IL1934L\SQLEXPRESS;'
                      'Database=MLB;'
                      'Trusted_Connection=yes;')
cursor = conn.cursor()

# Get yesterdays/todays date to update scores for today and yesterday, ensures games that end past midnight will be closed out in database
yesterday = date.today() - timedelta(days=1)
yesterdayForDB = (f'{yesterday} 00:00:00.000')

# Gets Games table from DB and replaces team Ids with MLB.com team names. Only get games from yesterday and today and put them into the gameDictionary. Only get games that have a GameComplete value of 0(not complete)
cursor.execute('''SELECT GameId, FirstPitchDateTime, AwayTeam.MLBName, HomeTeam.MLBName
                FROM Games g
                JOIN Teams AS HomeTeam
                ON g.HomeTeamId = HomeTeam.teamId
                JOIN Teams AS AwayTeam
                ON g.AwayTeamId = AwayTeam.TeamId
                WHERE g.FirstPitchDateTime > (?)
                AND g.GameComplete = 0''',
               (yesterdayForDB))
for game in cursor:
    DBGames[game[0]] = game[1:]

# Sets options for Chrome: Incognito, No Browser Window, No logs unless fatal,
option = webdriver.ChromeOptions()
option.add_argument("--incognito")
option.add_argument("--headless")
option.add_argument("--log-level=3")
# prefs = {'profile.managed_default_content_settings.images': 2}
# option.add_experimental_option("prefs", prefs)
browser = webdriver.Chrome(options=option)

# Make lists to hold scraped data:
teams = []
scores = []
innings = []
# will be used to determine if result is from yesterday or today
dateChangeHolder = 0

# scrapes MLB Scoreboard for yesterday and today:
dates = [yesterday, date.today()]
for idx, date in enumerate(dates):
    url = "https://www.mlb.com/scores/%s" % date
    browser.get(url)

    # parse HTML
    soup = BeautifulSoup(browser.page_source, 'html.parser')

    # Find the current "inning string" of the game
    inningHeaders = soup.find_all(
        "span", {
            "class": "g5-component--mlb-scores__linescore__gameinfo-short"
        })
    for span in inningHeaders:
        innings.append(span.get_text(strip=True))
    if(idx == 0):
        dateChangeHolder = len(innings)

    # Find the rows on MLB scoreboard that have the team names
    scoreboardTeamTables = soup.find_all("tr", {
        "class": "g5-component--mlb-scores__linescore__table__team-row"
    })
    # Scrape the team name that corresponds with the DB MLBTeamName and store it in teams list
    for row in scoreboardTeamTables:
        teamSpans = row.find_all("span", {
            "class": "g5-component--mlb-scores__team__info__name--long"
        })
        for team in teamSpans:
            teams.append(team.get_text(strip=True))

    # Scrape the score for each team and store it in Scores list
    scoreboardTeamTablesRunTDs = soup.find_all("td", {
        "class": "g5-component--mlb-scores__linescore__table--summary__cell--runs"
    })
    for td in scoreboardTeamTablesRunTDs:
        scores.append(td.get_text(strip=True))


count = 0
# Add Date, Inning, Away Team, Away Score, Home Team, Home Score to ScoreBoardGames:
for i in range(0, len(teams), 2):
    # Sets away and home team scores to 0, attempts to replace them with an updated score
    awayScore = 0
    homeScore = 0
    # this will assign the correct date to the ScoreBoardGames game:
    gameDate = pd.Timestamp(yesterday).to_pydatetime()
    if(count >= dateChangeHolder):
        gameDate = pd.Timestamp(date).to_pydatetime()

    try:
        awayScore = scores[i]
        homeScore = scores[i+1]
    except:
        pass

    gameInfo = [
        # converting date/time to datetime.datetime type from pandas timestamp
        gameDate,
        innings[count],
        teams[i],
        awayScore,
        teams[i + 1],
        homeScore
    ]
    ScoreBoardGames[count] = gameInfo
    #  add 1 for index for gameDictionary and inningHeaders
    count = count + 1


# for x in DBGames:
#     print(x, DBGames[x])

# for x in ScoreBoardGames:
#     print(x, ScoreBoardGames[x])


# Match DBGames(i) and scraped ScoreBoardGames(j), and update data in database
for i in DBGames:
    for j in ScoreBoardGames:
        # If Month, Day, AwayTeam, and HomeTeam are the same
        if DBGames[i][0].month == ScoreBoardGames[j][0].month and DBGames[i][0].day == ScoreBoardGames[j][0].day and DBGames[i][1] == ScoreBoardGames[j][2] and DBGames[i][2] == ScoreBoardGames[j][4]:
            # Check to see if the "inning" value indicates score is Final ("Final" or "F/#"):
            inning = ScoreBoardGames[j][1]
            awayScore = ScoreBoardGames[j][3]
            homeScore = ScoreBoardGames[j][5]
            gameId = i
            if inning.startswith("F"):
                cursor.execute('''UPDATE MLB.dbo.Games
                SET AwayScore = (?),
                    HomeScore = (?),
                    GameStarted = 1,
                    GameComplete = 1,
                    Inning = (?)
                WHERE GameId = (?)''',
                               (awayScore, homeScore, inning, i))
                cursor.commit()

            # else if the "inning" value indicates score is not Final ("Top #", "Mid #", or "Bot #"):
            elif inning.startswith("Top") or inning.startswith("Bot") or inning.startswith("Mid"):
                cursor.execute('''UPDATE MLB.dbo.Games
                SET AwayScore = (?),
                    HomeScore = (?),
                    GameStarted = 1,
                    Inning = (?)
                WHERE GameId = (?)''',
                               (awayScore, homeScore, inning, i))
                cursor.commit()

            # else the "inning" value indicates the game has not started yet or is postponed ("PPD" or time string):
            else:
                cursor.execute('''UPDATE MLB.dbo.Games
                SET AwayScore = (?),
                    HomeScore = (?),
                    Inning = (?)
                WHERE GameId = (?)''',
                               (awayScore, homeScore, inning, i))
                cursor.commit()
conn.close()
