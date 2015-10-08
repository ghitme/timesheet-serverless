-- Script Date: 10/7/2015 9:11 PM  - ErikEJ.SqlCeScripting version 3.5.2.56
-- Database information:
-- Locale Identifier: 1033
-- Encryption Mode: Platform Default
-- Case Sensitive: False
-- Database: C:\Users\Alfredo\Documents\Visual Studio 2013\Projects\Portfolio Projects\TimesheetServerless\TimesheetServerless\DB\timesheetDB.sdf
-- ServerVersion: 4.0.8876.1
-- DatabaseSize: 64 KB
-- Created: 10/7/2015 8:02 PM

-- User Table information:
-- Number of tables: 1
-- EMPLOYEE: 0 row(s)

SELECT 1;
PRAGMA foreign_keys=OFF;
BEGIN TRANSACTION;
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

