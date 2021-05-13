CREATE DATABASE Fidelity;
GO
USE Fidelity

CREATE TABLE Card(
	IDCard INT PRIMARY KEY NOT NULL IDENTITY(1,1),
	Name VARCHAR(15),
	Surname VARCHAR(20),
	Age INT CHECK(Age >= 18),
	Address VARCHAR(50),
	City VARCHAR(15)
);

CREATE TABLE Operation(
	CurrentPoints INT CHECK(CurrentPoints >= 0),
	FirstThreshold REAL,
	SecondThreshold REAL, 
	CurrentDate DATETIME2,
	IDCard INT REFERENCES Card(IDCard) ON DELETE CASCADE ON UPDATE CASCADE,
	[Message] VARCHAR(100),
	CONSTRAINT IDOperation PRIMARY KEY (IDCard, CurrentPoints)
);