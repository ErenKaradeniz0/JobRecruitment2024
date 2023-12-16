-- User table
CREATE TABLE [User] (
    UserID INT PRIMARY KEY,
    TC INT,
    name VARCHAR(100),
    surname VARCHAR(100),
    insurance_num VARCHAR(100),
    salary DECIMAL(10, 2),
    email VARCHAR(255),
    password VARCHAR(255),
    emp_status VARCHAR(50),
    phone_num VARCHAR(255)
);

-- Manager table
CREATE TABLE Manager (
    managerID INT PRIMARY KEY,
    username VARCHAR(100),
    password VARCHAR(255),
    name VARCHAR(100),
    surname VARCHAR(100),
    email VARCHAR(255),
    depID INT, -- Foreign key to Department table
    phone_num VARCHAR(255)
);

-- Department table
CREATE TABLE Department (
    depID INT PRIMARY KEY,
    depname VARCHAR(100)
);

-- Job table
CREATE TABLE Job (
    JobId INT PRIMARY KEY,
    JobName VARCHAR(100),
    employee_limit INT,
    depID INT -- Foreign key to Department table
);

-- Application table
CREATE TABLE Application (
    applicationID INT PRIMARY KEY,
    App_status VARCHAR(50),
    JobID INT, -- Foreign key to Job table
    TC INT -- Foreign key to User table
);

-- History table
CREATE TABLE History (
    HistoryID INT PRIMARY KEY,
    recruitment_date DATE,
    dismissal_date DATE,
    TC INT, -- Foreign key to User table
    JobID INT -- Foreign key to Job table
);