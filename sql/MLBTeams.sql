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

--***************************************************************************************

SELECT * FROM Teams
SELECT * FROM Games