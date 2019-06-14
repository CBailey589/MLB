import pyodbc

#Dictionary will hold GameId as Key and WinningTeamId as value
WinnerDictionary = {}

#Create Connection to SQL DB
conn = pyodbc.connect('Driver={SQL Server};'
                      'Server=DESKTOP-IL1934L\SQLEXPRESS;'
                      'Database=MLBPickem;'
                      'Trusted_Connection=yes;')
cursor = conn.cursor()

#Get all games from table that have a GameComplete value of 1, but a WinningTeamId of 0:
cursor.execute('''SELECT g.GameId, g.AwayTeamId, g.AwayScore, g.HomeTeamId, g.HomeScore, g.WinningTeamId
    FROM Games g
    WHERE g.GameComplete = 1
    AND g.WinningTeamId = 0
''')

# for x in cursor:
#     print(x)

for game in cursor:
    #Set variables for ease of reading
    gameId = game[0]
    awayTeamId = game[1]
    awayScore = game[2]
    homeTeamId = game[3]
    homeScore = game[4]
    winningTeamId = game[5]

    #if away team score was higher, away team won
    if awayScore > homeScore:
        WinnerDictionary[gameId] = awayTeamId
    #if home team score was higher, home team won
    else:
        WinnerDictionary[gameId] = homeTeamId

#Using a dictionary to loop over instead of the cursor becasue looping over the cursor is not working:
for gameId in WinnerDictionary:
    cursor.execute('''UPDATE MLBPickem.dbo.Games
    SET WinningTeamId = (?)
    WHERE GameId = (?)
    ''', (WinnerDictionary[gameId], gameId))
    cursor.commit()

conn.close()
