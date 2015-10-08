using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows.Forms;
using System.Data.SQLite;

/*
 * Handles Employee table operations:
 * 1. GetConnections: sets connection string path and sql connection
 * 2. AddEmployee: all employee data required
 * 3. RemoveEmployee: removes employee given first and last name
 * 4. GetAllEmployees: returns a list with all employees in Employee data table
 */
namespace TimesheetServerless
{
	public enum Departments { ARTS, STAFF, ADMINISTRATION, CUSTOMER_SERVICE}

    public static class EmployeeDatabase
    {
		

		public static int totalNumberOfEmployees = 20;
		public static SQLiteConnection GetConnection()
        {
			string connStr = @"Data Source=|DataDirectory|/DB/employee.db; version=3";		//[loc] is actual location of the database; found inside the Server Explorer
			//C:\Users\Alfredo\Documents\Visual Studio 2013\Projects\Portfolio Projects\TimeSheetPortfolio\TimeSheetPortfolio
            //string connStr = SimpleDatabaseProject.Properties.Settings.;
			SQLiteConnection conn = new SQLiteConnection(connStr);
            return conn;
        }

        //Add employee to table, using parameters
        public static void AddEmployee(string firstName, string lastName, string department, string phone, string email, string date)
        {
			//Check employees have not surpassed the limit
			List<Employee> checkAll = GetAllEmployees();
			int count = checkAll.Count;
			if (count > totalNumberOfEmployees)
				return;

			string searchStatement = "INSERT INTO EMPLOYEESQLITE (FirstName, LastName, Department, Phone, Email, Date) VALUES (@firstName, @lastName, @department, @phone, @email, @date)";
			if (date == "")
				date = DateTime.Now.Date.ToString("MM/dd/yyyy");
                
            SQLiteConnection conn = GetConnection();
			SQLiteCommand cmd = new SQLiteCommand(searchStatement, conn);
            
            //Fill in the command parameters 
            cmd.Parameters.AddWithValue("@firstName", firstName);            //[this, is this] type of function
            cmd.Parameters.AddWithValue("@lastName", lastName);
            cmd.Parameters.AddWithValue("@department", department);
            cmd.Parameters.AddWithValue("@phone", phone);
            cmd.Parameters.AddWithValue("@email", email);
			cmd.Parameters.AddWithValue("@date", date);
            
            //Try to execute            
            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();                                      //Execute INSERT statement
            }
			catch (SQLiteException exception) { throw exception; }
            finally { conn.Close(); }

        }


        //Remove employee from table in cell: only need FirstName and LastName
        public static void RemoveEmployee(string firstName, string lastName)
        {
			string searchStatement = "DELETE FROM EMPLOYEESQLITE WHERE FirstName = @firstName AND LastName = @lastName ";
			SQLiteConnection conn = GetConnection();
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


        //Get first employee from table: only need FirstName and LastName
        public static Employee GetEmployee(string firstName, string lastName)
        {
			string searchStatement = "SELECT * FROM EMPLOYEESQLITE WHERE FirstName = @firstName AND LastName = @lastName ";
			SQLiteConnection conn = GetConnection();
			SQLiteCommand cmd = new SQLiteCommand(searchStatement, conn);

            //Fill in the command parameters           
            cmd.Parameters.AddWithValue("@firstName", firstName);
            cmd.Parameters.AddWithValue("@lastName", lastName);

            Employee employee = new Employee();

            //Try to execute            
            try
            {
                conn.Open();
				SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
					employee.EmployeeID = int.Parse(reader["Id"].ToString().Trim());
                    employee.FirstName = reader["firstName"].ToString().Trim();
					employee.LastName = reader["lastName"].ToString().Trim();
					employee.Department = reader["department"].ToString().Trim();
					employee.Phone = reader["phone"].ToString().Trim();
					employee.Email = reader["email"].ToString().Trim();
					employee.StartingDate = reader["date"].ToString().Trim();
                }
               
            }
			catch (SQLiteException exception) { throw exception; }
            finally { conn.Close(); }
            return employee;
        }

        //Get all employees
        public static List<Employee> GetAllEmployees()
        {
            List<Employee> employeeList = new List<Employee>();

			string statement = "SELECT * FROM EMPLOYEESQLITE ORDER BY Id";
			SQLiteConnection conn = GetConnection();
			SQLiteCommand cmd = new SQLiteCommand(statement, conn);
			
            //Try to read
            try
            {
				conn.Open();
				SQLiteDataReader reader = cmd.ExecuteReader();         //Read from SQL with SqlDataReader 
				if(reader.HasRows)
				{
					while (reader.Read())
					{
						Employee employee = new Employee();
						
						//Initialize properties
						employee.EmployeeID = int.Parse(reader["Id"].ToString().Trim());       //OR reader.GetString(counter)
						employee.FirstName = reader["firstName"].ToString().Trim();
						employee.LastName = reader["lastName"].ToString().Trim();
						employee.Department = reader["department"].ToString().Trim();
						employee.Phone = reader["phone"].ToString().Trim();
						employee.Email = reader["email"].ToString().Trim();
						employee.StartingDate = reader["date"].ToString().Trim();

						//Add to list
						employeeList.Add(employee);
					}
				}

                //Sort uses custom CompareTo
                //employeeList.Sort();
                reader.Close();
            }
            catch (Exception exception) { throw exception;}
            finally { conn.Close(); }

            return employeeList;
        }

		//Print all employees in Message Box popup
		public static void UpdateEmployee(string firstName, string lastName, string department, string phone, string email, string startingDate)
		{
			SQLiteConnection conn = GetConnection();
			string statement = "UPDATE EMPLOYEESQLITE SET FirstName = @firstName, LastName = @lastName, Department = @department, Phone = @phone, Email = @email, StartingDate = @startingDate WHERE FirstName = @firstName AND LastName = @lastName";
			SQLiteCommand cmd = new SQLiteCommand(statement, conn);

			//Parameters         
			cmd.Parameters.AddWithValue("@firstName", firstName);
			cmd.Parameters.AddWithValue("@lastName", lastName);
			cmd.Parameters.AddWithValue("@department", department);
			cmd.Parameters.AddWithValue("@phone", phone);
			cmd.Parameters.AddWithValue("@email", email);
			cmd.Parameters.AddWithValue("@date", startingDate);
			try
			{
				conn.Open();
				cmd.ExecuteNonQuery();
			}
			catch (Exception exc) { throw exc; }
			finally
			{
				conn.Close();
			}
		}
		
		

		//Get employees by department, USE enum
		public static List<Employee> GetByDepartment(Departments department)
		{
			List<Employee> result = new List<Employee>();

			string departmentStr = Enum.GetName(typeof(Departments),
				department);

			string statement = "SELECT * FROM EMPLOYEESQLITE ORDER BY EmployeeID WHERE Department = @department";
			SQLiteConnection conn = GetConnection();
			SQLiteCommand cmd = new SQLiteCommand(statement, conn);

			//Fill in with parameters
			cmd.Parameters.AddWithValue("@department", departmentStr);

			try
			{
				conn.Open();

				SQLiteDataReader reader = cmd.ExecuteReader();

				while (reader.Read())
				{
					Employee employee = new Employee();

					//Initialize properties
					employee.EmployeeID = (int)reader["EmployeeID"];       //OR reader.GetString(counter)
					employee.FirstName = reader["FirstName"].ToString().Trim();
					employee.LastName = reader["LastName"].ToString().Trim();
					employee.Department = reader["Department"].ToString().Trim();
					employee.Phone = reader["Phone"].ToString().Trim();
					employee.Email = reader["Email"].ToString().Trim();
					employee.StartingDate = reader["Date"].ToString().Trim();

					//Add to list
					result.Add(employee);
				}
			}
			catch (Exception exc){throw exc;}
			finally
			{
				conn.Close();
			}
			return result;
		}



		//Misc______________________________________________________________________________

		//Get total of employees
		public static int GetTotalEmployees()
		{
			List<Employee> allEmployees = GetAllEmployees();
			return allEmployees.Count;
		}

		
    }
}
