# timesheet-serverless 
-----------------------------------------------------------------------
This is a version of the timesheet project that uses SQLite instead of SQL. By removing the inconvenience for the client of having to install a SQL server instance, SQLite brings more portability at expenses unnoticed for this small project.

2 Tables: 1 is a timesheet with job-like functionality that gives managers the opportunity to track employees' clocking times, and note any reason for irregularities; and 2nd, an employee datatable records employees' basic information: first and last names, department they work at, phone, email, and starting date of employment, as well as remove employees from the records, or update their information.

SQLite usage is limited to CRUD operations, and Winforms technologies are used for the interface.

Executable location:
> TimesheetServerless/bin/x86/Release/TimesheetServerless.exe

Requirements: .Net 4.0
