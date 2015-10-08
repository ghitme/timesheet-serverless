using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows.Forms;


/*
 * Handles Timesheet table functions
 * TODO: consider implement a base SQLite class
 */
namespace TimesheetServerless
{
    public static class TimeSheetDatabase
    {
        //Same database as databaseclass
        public static SQLiteConnection GetConnectionTimeSheet()
        {
			string connStr = @"Data Source=|DataDirectory|/DB/employee.db; version=3";		//[loc] is actual location of the database; found inside the Server Explorer
            //C:\Users\Alfredo\Documents\Visual Studio 2013\Projects\Portfolio Projects\TimeSheetPortfolio\TimeSheetPortfolio
            //string connStr = SimpleDatabaseProject.Properties.Settings.;
            SQLiteConnection conn = new SQLiteConnection(connStr);
            return conn;
        }


        //Add employee to table, using parameters
        public static void AddWeekWork(string employeeNumber, string firstName, string lastName,
            string punchIn,
            string lunchIn,
            string lunchOut,
            string punchOut,            
            string reason,
            string assoc,
            string admin,
            string week)
        {
			string searchStatement = "INSERT INTO TIMESHEET (EmployeeID, FirstName, LastName, PunchIn, PunchOut, LunchIn, LunchOut, Reason, Assoc, Admin, Week) VALUES (@employeeID, @firstName, @lastName, @punchIn, @punchOut, @lunchIn, @lunchOut, @reason, @assoc, @admin, @week)";
            SQLiteConnection conn = GetConnectionTimeSheet();
            SQLiteCommand cmd = new SQLiteCommand(searchStatement, conn);
            
            //Fill in the command parameters         
			cmd.Parameters.AddWithValue("@employeeID", employeeNumber);
            cmd.Parameters.AddWithValue("@firstName", firstName);            //text field
            cmd.Parameters.AddWithValue("@lastName", lastName);
            
            cmd.Parameters.AddWithValue("@punchIn", punchIn);               //string 
            cmd.Parameters.AddWithValue("@punchOut", punchOut);
            cmd.Parameters.AddWithValue("@lunchIn", lunchIn);
            cmd.Parameters.AddWithValue("@lunchOut", lunchOut);            
            cmd.Parameters.AddWithValue("@reason", reason);
            cmd.Parameters.AddWithValue("@assoc", assoc);
            cmd.Parameters.AddWithValue("@admin", admin);
            cmd.Parameters.AddWithValue("@week", week);

            //Try to execute            
            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();                                      //Execute INSERT statement
            }
            catch (SQLiteException exception) { throw exception; }
            finally { conn.Close(); }
         
        }



        //Read from table Timsheet first workweek of employee's: only need FirstName and LastName
        public static List<WorkWeek> GetAllWorkWeekForSingleEmployee(string firstName, string lastName)
        {
            string searchStatement = "SELECT * FROM TIMESHEET WHERE FirstName = @firstName AND LastName = @lastName ORDER BY TableID";
            SQLiteConnection conn = GetConnectionTimeSheet();
            SQLiteCommand cmd = new SQLiteCommand(searchStatement, conn);

            //Fill in the command parameters           
            cmd.Parameters.AddWithValue("@firstName", firstName);
            cmd.Parameters.AddWithValue("@lastName", lastName);

            List<WorkWeek> workWeekList = new List<WorkWeek>();

            //Try to execute            
            try
            {
                conn.Open();
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    WorkWeek workWeek = new WorkWeek();

					workWeek.TableID = int.Parse(reader["TableID"].ToString());
					workWeek.EmployeeID = reader["EmployeeID"].ToString().Trim();
					workWeek.FirstName = reader["FirstName"].ToString().Trim();
					workWeek.LastName = reader["LastName"].ToString().Trim();

                    workWeek.PunchIn = reader["PunchIn"].ToString().Split();			//Split
                    workWeek.PunchOut = reader["PunchOut"].ToString().Split();
                    workWeek.LunchIn = reader["LunchIn"].ToString().Split();
                    workWeek.LunchOut = reader["LunchOut"].ToString().Split();
                    workWeek.Reason = reader["Reason"].ToString().Split();
                    workWeek.Assoc = reader["Assoc"].ToString().Split();
                    workWeek.Admin = reader["Admin"].ToString().Split();

					workWeek.Week = reader["Week"].ToString().Trim();

                    workWeekList.Add(workWeek);
                }

            }
            catch (SQLiteException exception) { throw exception; }
            finally { conn.Close(); }
            return workWeekList;
        }

        //Get all employees work in terms of weeks
        public static List<WorkWeek> GetAllTimeSheet()
        {
            List<WorkWeek> workWeekList = new List<WorkWeek>();
            string statement = "SELECT * FROM TIMESHEET ORDER BY TableID";
            SQLiteConnection conn = GetConnectionTimeSheet();
            SQLiteCommand cmd = new SQLiteCommand(statement, conn);

            //Try to read
            try
            {
                conn.Open(); 
                SQLiteDataReader reader = cmd.ExecuteReader();         //Read from SQLite with SQLiteDataReader 
                while (reader.Read())
                {
                    WorkWeek workWeek = new WorkWeek();

                    //Initialize properties
                    workWeek.TableID = (int)reader["TableID"];
					workWeek.EmployeeID = reader["EmployeeID"].ToString().Trim();
					workWeek.FirstName = reader["FirstName"].ToString().Trim();
					workWeek.LastName = reader["LastName"].ToString().Trim();
                    
                    workWeek.PunchIn = reader["PunchIn"].ToString().Split();
                    workWeek.PunchOut = reader["PunchOut"].ToString().Split();
                    workWeek.LunchIn = reader["LunchIn"].ToString().Split();
                    workWeek.LunchOut = reader["LunchOut"].ToString().Split();
                    workWeek.Reason = reader["Reason"].ToString().Split();
                    workWeek.Assoc = reader["Assoc"].ToString().Split();
                    workWeek.Admin = reader["Admin"].ToString().Split();
                    
					workWeek.Week = reader["Week"].ToString().Trim();
                    
                    //Add to list
                    workWeekList.Add(workWeek);
                }

                reader.Close();
            }
            catch (Exception exception) { throw exception;}
            finally { conn.Close(); }

            return workWeekList;
        }
		

		//INPUT: date coming from PrepareSave: tableID, and each column in longColumns
		public static void UpdateTimeSheet(int tableID, string PunchIn, string LunchIn, string LunchOut, string PunchOut, string Reason, string Assoc, string Admin)
		{
			SQLiteConnection conn = GetConnectionTimeSheet();
			string statement = "UPDATE TIMESHEET SET PunchIn = @PunchIn, LunchIn = @LunchIn, LunchOut = @LunchOut, PunchOut = @PunchOut, Reason = @Reason, Assoc = @Assoc, Admin = @Admin WHERE TableID=@tableID";
			SQLiteCommand cmd = new SQLiteCommand(statement, conn);
			
			//Parameters         
			cmd.Parameters.AddWithValue("@tableID", tableID);
			cmd.Parameters.AddWithValue("@PunchIn", PunchIn);
			cmd.Parameters.AddWithValue("@LunchIn", LunchIn);
			cmd.Parameters.AddWithValue("@LunchOut", LunchOut);
			cmd.Parameters.AddWithValue("@PunchOut", PunchOut);
			cmd.Parameters.AddWithValue("@Reason", Reason);
			cmd.Parameters.AddWithValue("@Assoc", Assoc);
			cmd.Parameters.AddWithValue("@Admin", Admin);

			try
			{
				conn.Open();
				cmd.ExecuteNonQuery();
			}
			catch (Exception exc){ throw exc;}
			finally
			{
				conn.Close();
			}
		}


		//Remove employee from table in cell: only need FirstName and LastName
		public static void RemoveWorkWeekWithName(string firstName, string lastName)
		{
			string searchStatement = "DELETE FROM TIMESHEET WHERE FirstName = @firstName AND LastName = @lastName ";
			SQLiteConnection conn = GetConnectionTimeSheet();
			SQLiteCommand cmd = new SQLiteCommand(searchStatement, conn);

			//Fill in the command parameters           
			cmd.Parameters.AddWithValue("@firstName", firstName);
			cmd.Parameters.AddWithValue("@lastName", lastName);

			//Try to execute            
			try
			{
				conn.Open();
				cmd.ExecuteNonQuery();                              //Execute INSERT statement
			}
			catch (SQLiteException exception) { throw exception; }
			finally { conn.Close(); }
		}
        
    }
}
