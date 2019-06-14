-- USE MASTER
-- GO

-- IF NOT EXISTS (
--     SELECT [name]
--     FROM sys.databases
--     WHERE [name] = N'MLB'
-- )
-- CREATE DATABASE MLB
-- GO

-- USE MLB
-- GO

DELETE FROM Teams;
DELETE FROM Games;

DROP TABLE IF EXISTS Teams;
DROP TABLE IF EXISTS Games;

CREATE TABLE Teams (
    TeamId INTEGER NOT NULL PRIMARY KEY IDENTITY,
    City VARCHAR(55) NOT NULL,
    VIName VARCHAR(55) NOT NULL,
    MLBName VARCHAR(55) NOT NULL,
);

CREATE TABLE Games (
    GameId INTEGER NOT NULL PRIMARY KEY IDENTITY,
    FirstPitchDateTime DATETIME NOT NULL,
    AwayTeamId INTEGER NOT NULL,
    AwayLine VARCHAR(4) NOT NULL,
    AwaySP VARCHAR(55) NOT NULL,
    HomeTeamId VARCHAR(55) NOT NULL,
    HomeLine VARCHAR(4) NOT NULL,
    HomeSp VARCHAR(55) NOT NULL,
    AwayScore INT NOT NULL,
    HomeScore INT NOT NULL,
    GameStarted BIT,
    GameComplete BIT,
    Inning VARCHAR(25)
)

--*********** INITIAL DATA ***************--
INSERT INTO Teams (City, VIName, MLBName) VALUES ('Arizona', 'Arizona', 'D-backs');
INSERT INTO Teams (City, VIName, MLBName) VALUES ('Atlanta', 'Atlanta', 'Braves');
INSERT INTO Teams (City, VIName, MLBName) VALUES ('Baltimore', 'Baltimore', 'Orioles');
INSERT INTO Teams (City, VIName, MLBName) VALUES ('Boston', 'Boston', 'Red Sox');
INSERT INTO Teams (City, VIName, MLBName) VALUES ('Chicago Cubs', 'Chi. Cubs', 'Cubs');
INSERT INTO Teams (City, VIName, MLBName) VALUES ('Chicago White Sox', 'Chi. White Sox', 'White Sox');
INSERT INTO Teams (City, VIName, MLBName) VALUES ('Cincinnati', 'Cincinnati', 'Reds');
INSERT INTO Teams (City, VIName, MLBName) VALUES ('Cleveland', 'Cleveland', 'Indians');
INSERT INTO Teams (City, VIName, MLBName) VALUES ('Colorado', 'Colorado', 'Rockies');
INSERT INTO Teams (City, VIName, MLBName) VALUES ('Detroit', 'Detroit', 'Tigers');
INSERT INTO Teams (City, VIName, MLBName) VALUES ('Houston', 'Houston', 'Astros');
INSERT INTO Teams (City, VIName, MLBName) VALUES ('Kansas City', 'Kansas City', 'Royals');
INSERT INTO Teams (City, VIName, MLBName) VALUES ('Los Angeles Angels', 'L.A. Angels', 'Angels');
INSERT INTO Teams (City, VIName, MLBName) VALUES ('Los Angeles Dodgers', 'L.A. Dodgers', 'Dodgers');
INSERT INTO Teams (City, VIName, MLBName) VALUES ('Miami', 'Miami', 'Marlins');
INSERT INTO Teams (City, VIName, MLBName) VALUES ('Milwaukee', 'Milwaukee', 'Brewers');
INSERT INTO Teams (City, VIName, MLBName) VALUES ('Minnesota', 'Minnesota', 'Twins');
INSERT INTO Teams (City, VIName, MLBName) VALUES ('New York Mets', 'N.Y. Mets', 'Mets');
INSERT INTO Teams (City, VIName, MLBName) VALUES ('New York Yankees', 'N.Y. Yankees', 'Yankees');
INSERT INTO Teams (City, VIName, MLBName) VALUES ('Oakland', 'Oakland', 'Athletics');
INSERT INTO Teams (City, VIName, MLBName) VALUES ('Philadelphia', 'Philadelphia', 'Phillies');
INSERT INTO Teams (City, VIName, MLBName) VALUES ('Pittsburgh', 'Pittsburgh', 'Pirates');
INSERT INTO Teams (City, VIName, MLBName) VALUES ('San Diego', 'San Diego', 'Padres');
INSERT INTO Teams (City, VIName, MLBName) VALUES ('San Francisco', 'San Francisco', 'Giants');
INSERT INTO Teams (City, VIName, MLBName) VALUES ('Seattle', 'Seattle', 'Mariners');
INSERT INTO Teams (City, VIName, MLBName) VALUES ('St. Louis', 'St. Louis', 'Cardinals');
INSERT INTO Teams (City, VIName, MLBName) VALUES ('Tampa Bay', 'Tampa Bay', 'Rays');
INSERT INTO Teams (City, VIName, MLBName) VALUES ('Texas', 'Texas', 'Rangers');
INSERT INTO Teams (City, VIName, MLBName) VALUES ('Toronto', 'Toronto', 'Blue Jays');
INSERT INTO Teams (City, VIName, MLBName) VALUES ('Washington', 'Washington', 'Nationals');



UPDATE Teams SET PrimaryColor = '#A71930', SecondaryColor = '#E3D4AD' WHERE TeamId = 1
UPDATE Teams SET PrimaryColor = '#13274F', SecondaryColor = '#CE1141' WHERE TeamId = 2
UPDATE Teams SET PrimaryColor = '#DF4601', SecondaryColor = '#000000' WHERE TeamId = 3
UPDATE Teams SET PrimaryColor = '#0C2340', SecondaryColor = '#BD3039' WHERE TeamId = 4
UPDATE Teams SET PrimaryColor = '#0E3386', SecondaryColor = '#CC3433' WHERE TeamId = 5
UPDATE Teams SET PrimaryColor = '#27251F', SecondaryColor = '#C4CED4' WHERE TeamId = 6
UPDATE Teams SET PrimaryColor = '#000000', SecondaryColor = '#C6011F' WHERE TeamId = 7
UPDATE Teams SET PrimaryColor = '#0C2340', SecondaryColor = '#E31937' WHERE TeamId = 8
UPDATE Teams SET PrimaryColor = '#33006F', SecondaryColor = '#C4CED4' WHERE TeamId = 9
UPDATE Teams SET PrimaryColor = '#0C2340', SecondaryColor = '#FA4616' WHERE TeamId = 10
UPDATE Teams SET PrimaryColor = '#002D62', SecondaryColor = '#EB6E1F' WHERE TeamId = 11
UPDATE Teams SET PrimaryColor = '#BD9B60', SecondaryColor = '#004687' WHERE TeamId = 12
UPDATE Teams SET PrimaryColor = '#003263', SecondaryColor = '#BA0021' WHERE TeamId = 13
UPDATE Teams SET PrimaryColor = '#005A9C', SecondaryColor = '#EF3E42' WHERE TeamId = 14
UPDATE Teams SET PrimaryColor = '#00A3E0', SecondaryColor = '#EF3340' WHERE TeamId = 15
UPDATE Teams SET PrimaryColor = '#0A2351', SecondaryColor = '#B6922E' WHERE TeamId = 16
UPDATE Teams SET PrimaryColor = '#002B5C', SecondaryColor = '#D31145' WHERE TeamId = 17
UPDATE Teams SET PrimaryColor = '#002D72', SecondaryColor = '#FF5910' WHERE TeamId = 18
UPDATE Teams SET PrimaryColor = '#0C2340', SecondaryColor = '#C4CED3' WHERE TeamId = 19
UPDATE Teams SET PrimaryColor = '#003831', SecondaryColor = '#EFB21E' WHERE TeamId = 20
UPDATE Teams SET PrimaryColor = '#002D72', SecondaryColor = '#E81828' WHERE TeamId = 21
UPDATE Teams SET PrimaryColor = '#27251F', SecondaryColor = '#FDB827' WHERE TeamId = 22
UPDATE Teams SET PrimaryColor = '#002D62', SecondaryColor = '#A2AAAD' WHERE TeamId = 23
UPDATE Teams SET PrimaryColor = '#27251F', SecondaryColor = '#FD5A1E' WHERE TeamId = 24
UPDATE Teams SET PrimaryColor = '#0C2C56', SecondaryColor = '#005C5C' WHERE TeamId = 25
UPDATE Teams SET PrimaryColor = '#0C2340', SecondaryColor = '#C41E3A' WHERE TeamId = 26
UPDATE Teams SET PrimaryColor = '#8FBCE6', SecondaryColor = '#092C5C' WHERE TeamId = 27
UPDATE Teams SET PrimaryColor = '#003278', SecondaryColor = '#C0111F' WHERE TeamId = 28
UPDATE Teams SET PrimaryColor = '#134A8E', SecondaryColor = '#E8291C' WHERE TeamId = 29
UPDATE Teams SET PrimaryColor = '#14225A', SecondaryColor = '#AB0003' WHERE TeamId = 30

--***************************************************************************************

SELECT * FROM Teams
SELECT * FROM Games