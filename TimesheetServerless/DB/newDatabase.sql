-- Script Date: 10/8/2015 12:12 PM  - ErikEJ.SqlCeScripting version 3.5.2.56
-- Database information:
-- Locale Identifier: 1033
-- Encryption Mode: Platform Default
-- Case Sensitive: False
-- Database: C:\Users\Alfredo\Documents\Visual Studio 2013\Projects\Portfolio Projects\TimesheetServerless\TimesheetServerless\DB\timesheetDB.sdf
-- ServerVersion: 4.0.8876.1
-- DatabaseSize: 64 KB
-- Created: 10/7/2015 8:02 PM

-- User Table information:
-- Number of tables: 2
-- EMPLOYEE: 0 row(s)
-- TIMESHEET: 0 row(s)

SELECT 1;
PRAGMA foreign_keys=OFF;
BEGIN TRANSACTION;
CREATE TABLE [TIMESHEET] (
  [TableID] INTEGER NOT NULL
, [EmployeeID] nchar(3) NOT NULL
, [FirstName] nchar(20) NOT NULL
, [LastName] nchar(20) NOT NULL
, [PunchIn] nchar(56) NOT NULL
, [LunchIn] nchar(56) NOT NULL
, [LunchOut] nchar(56) NOT NULL
, [PunchOut] nchar(56) NOT NULL
, [Reason] nchar(100) NULL
, [Assoc] nchar(56) NOT NULL
, [Admin] nchar(56) NOT NULL
, [Week] nchar(56) DEFAULT GetDate() NOT NULL
, CONSTRAINT [PK_TIMESHEET] PRIMARY KEY ([TableID])
);
CREATE TABLE [EMPLOYEE] (
  [Id] INTEGER NOT NULL
, [firstName] nchar(25) NOT NULL
, [lastName] nchar(25) NOT NULL
, [department] nchar(25) NOT NULL
, [phone] nchar(12) NOT NULL
, [email] nchar(25) NOT NULL
, [date] nchar(10) DEFAULT GetDate() NOT NULL
, CONSTRAINT [PK_EMPLOYEE] PRIMARY KEY ([Id])
);
COMMIT;

