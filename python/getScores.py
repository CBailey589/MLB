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
                      'Database=MLBPickem;'
                      'Trusted_Connection=yes;')
cursor = conn.cursor()

# Get yesterdays/todays date to update scores for today and yesterday, ensures games that end past midnight will be closed out in database
yesterday = date.today() - timedelta(days=1)
yesterdayForDB = (f'{yesterday} 00:00:00.000')

# Gets Games table from DB and replaces team Ids with MLB.com team names. Only get games from yesterday and today and put them into the gameDictionary. Only get games that have a GameComplete value of 0(not complete)
cursor.execute('''SELECT GameId, FirstPitchDateTime, AwayTeam.MLBName, HomeTeam.MLBName, MLBScoreBoardId
                FROM Games g
                JOIN Teams AS HomeTeam
                ON g.HomeTeamId = HomeTeam.teamId
                JOIN Teams AS AwayTeam
                ON g.AwayTeamId = AwayTeam.TeamId
                WHERE g.FirstPitchDateTime > (?)
                AND g.GameComplete = 0''',
               (yesterdayForDB))
for game in cursor:
    # turns DB datetime string into datetime.datetime object
    game[1] = datetime.datetime.strptime(game[1].split(".")[0], '%Y-%m-%d %H:%M:%S')
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
gamePrimaryKeys = []
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

    # Scrape MLB game Primary Keys
    regex = re.compile('[\d]')
    MLBPrimaryKeys = soup.find_all("div", {"data-gamepk": regex})
    for PK in MLBPrimaryKeys:
        # note this value is appended as a tuple and will need to be joined to get a string/int
        gamePrimaryKeys.append(PK.get('data-gamepk'))



count = 0
# Replacing all blank scores with 0 if necessary (happens for postponed games)
for score in scores:
    if score == '':
        scores[scores.index(score)] = '0'
# Add Date, Inning, Away Team, Away Score, Home Team, Home Score to ScoreBoardGames:
for i in range(0, len(innings)):
    # if the game has started this will not be changed, it the game is later in the day this will be changed to the start pitch datetime object. This is necessary to tell the difference between days with double headers.
    startTimeStrpTime = datetime.datetime(1901, 1, 1, 0, 0)
    # Checks to see if game has a time listed in the innings (still in the future)
    if innings[i][0].isdigit():
        # adds 0s for the score, MLB scoreboard does not have these until the game status is Warmup
        scores.insert(i * 2, '0')
        scores.insert(i * 2, '0')
        # changes inning to a datetime.datetime to use to assign MLB Primary Key to database
        startTimeStrpTime = datetime.datetime.strptime(' '.join(innings[i].split())[:-3], '%I:%M %p')

    # this will assign the correct date to the ScoreBoardGames game:
    gameDate = pd.Timestamp(yesterday).to_pydatetime()
    if(count >= dateChangeHolder):
        gameDate = pd.Timestamp(date).to_pydatetime()

    awayTeam = teams[i * 2]
    awayScore = scores[i * 2]
    homeTeam = teams[(i * 2) + 1]
    homeScore = scores[(i * 2) + 1]
    gamePK = gamePrimaryKeys[i],

    gameInfo = [
        # converting date/time to datetime.datetime type from pandas timestamp
        gameDate,
        innings[count],
        awayTeam,
        awayScore,
        homeTeam,
        homeScore,
        gamePK,
        startTimeStrpTime
    ]
    ScoreBoardGames[count] = gameInfo
    #  add 1 for index for gameDictionary and inningHeaders
    count = count + 1

# #************************* UNCOMMENT TO SEE REPORTS PRINTED IN CONSOLE
# for x in DBGames:
#     print(x, DBGames[x])

# for x in ScoreBoardGames:
#     print(x, ScoreBoardGames[x])

# This Section will loop throug the games and assign the MLB primary keys to games that don't have them assigned:
for i in DBGames:
    if DBGames[i][3] == 0:
        for j in ScoreBoardGames:
            if ScoreBoardGames[j][7].hour == DBGames[i][0].hour and ScoreBoardGames[j][2] == DBGames[i][1] and ScoreBoardGames[j][4] == DBGames[i][2]:
                MLBSBID = int(''.join(ScoreBoardGames[j][6]))
                cursor.execute('''UPDATE MLBPickem.dbo.Games
                SET MLBScoreBoardId = (?)
                WHERE GameId = (?)
                ''', (MLBSBID, i))
                cursor.commit()

# Match DBGames(i) and scraped ScoreBoardGames(j), and update data in database
for i in DBGames:
    for j in ScoreBoardGames:
        # If Month, Day, AwayTeam, and HomeTeam are the same
        MLBSBID = int(''.join(ScoreBoardGames[j][6]))
        if MLBSBID == DBGames[i][3]:
            # Check to see if the "inning" value indicates score is Final ("Final" or "F/#"):
            inning = ScoreBoardGames[j][1]
            awayScore = ScoreBoardGames[j][3]
            homeScore = ScoreBoardGames[j][5]
            gameId = i
            if inning.startswith("F"):
                cursor.execute('''UPDATE MLBPickem.dbo.Games
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
                cursor.execute('''UPDATE MLBPickem.dbo.Games
                SET AwayScore = (?),
                    HomeScore = (?),
                    GameStarted = 1,
                    Inning = (?)
                WHERE GameId = (?)''',
                               (awayScore, homeScore, inning, i))
                cursor.commit()

            # else the "inning" value indicates the game has not started yet or is postponed ("PPD" or time string):
            else:
                cursor.execute('''UPDATE MLBPickem.dbo.Games
                SET AwayScore = (?),
                    HomeScore = (?),
                    Inning = (?)
                WHERE GameId = (?)''',
                               (awayScore, homeScore, inning, i))
                cursor.commit()
conn.close()
