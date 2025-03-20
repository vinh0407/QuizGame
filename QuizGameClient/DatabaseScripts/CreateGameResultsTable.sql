CREATE DATABASE IF NOT EXISTS QuizGameDB;

USE QuizGameDB;

CREATE TABLE GameResults (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    PlayerIp VARCHAR(50),
    Score INT,
    DateTime DATETIME DEFAULT CURRENT_TIMESTAMP
);

SELECT * FROM GameResults;

CREATE TABLE Questions (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    Content VARCHAR(255) NOT NULL,
    Options TEXT NOT NULL,  -- Store the options as a JSON or a delimited string
    CorrectAnswer INT NOT NULL
);

SELECT * FROM Question;
