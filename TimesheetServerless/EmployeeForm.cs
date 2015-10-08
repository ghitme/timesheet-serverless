using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;                                  //StringBuilder
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;


namespace TimesheetServerless
{
    public partial class EmployeeForm : Form
    {
        //Data for autocomplete
        List<Employee> allEmployees;
        List<Control> allTextBoxControls;				//Have autocomplete     
        
        public EmployeeForm()
        {
            InitializeComponent();

            //AutoComplete
            //-Initializing Controls (TextBox)
            allTextBoxControls = UtilWinforms.GetAllControlsOfType(this, typeof(TextBox)).ToList();

            allEmployees = EmployeeDatabase.GetAllEmployees();                                         //Read table for updates  

            //Textbox - Settings
            foreach (TextBox e in allTextBoxControls)
            {
                //Assign events
                e.KeyUp += new KeyEventHandler(txt_EnterKeyUp);

                //Assign a tag to all text boxes in form2
                e.Tag = "Search";                
            }

            //Run AutoComplete
            UtilWinforms.UpdateAutoComplete(ref allTextBoxControls, ref allEmployees);			            
        }


        //Updates ListView
        private void Form2_Load(object sender, EventArgs e)
        {
            listView1.Items.Clear();                                //Refresh
            try
            {
				//Load all employees
                allEmployees = EmployeeDatabase.GetAllEmployees();

                if (allEmployees.Count > 0)
                {
                    Employee employee;                              //Temp
                    for (int i = 0; i < allEmployees.Count; i++)
                    {
                        employee = allEmployees[i];

                        //Add this employee number to the list of items in view; (primary key)
                        listView1.Items.Add(employee.EmployeeID.ToString());

                        //Add also its columns
                        listView1.Items[i].SubItems.Add(employee.FirstName);
                        listView1.Items[i].SubItems.Add(employee.LastName);
                        listView1.Items[i].SubItems.Add(employee.Department);
                        listView1.Items[i].SubItems.Add(employee.Phone);
                        listView1.Items[i].SubItems.Add(employee.Email);
                        listView1.Items[i].SubItems.Add(employee.StartingDate);

                    }
                }
                else { MessageBox.Show("No employee found.", "Alert"); }
            }
            catch (Exception exception)
            {

                MessageBox.Show(exception.Message, exception.GetType().ToString());
            }
        }



        //Submit button event, add employee, update auto complete, and include employee in timesheet
        private void btnSubmit_Click(object sender, EventArgs e)
        {
            //Exit this function if there is any blank text leave out
            if (txtFirstName.Text == "" || txtLastName.Text == "" || txtDepartment.Text == "" || txtPhone.Text == "" || txtEmail.Text == "")
            {
                MessageBox.Show("Missing data; please fill in all fields.");
                return;
            }

			/* ==================================================================
			 * Format invariants
			 * - Phone: xxx-xxx-xxxx
			 * - Department: enum match
			 * - Email: _@_.com
			 * - Date: MM/dd/yyyy (just tryparse)
			 */
			if (!UtilDotNET.IsMatch(UtilDotNET.phonePattern1, txtPhone.Text))
			{
				MessageBox.Show("Error: not a valid phone format.");
				return;
			}
				

			if (!UtilDotNET.IsMatch(UtilDotNET.emailPattern, txtEmail.Text))
			{
				MessageBox.Show("Error: invalid email format.");
				return;
			}


			if (!UtilDotNET.ValidateDate(txtStartingDate.Text) && txtStartingDate.Text != "")
			{
				MessageBox.Show("Invalid date format");
				return;
			}
			


			//Exit if employee is already in list
			Employee solicitude = EmployeeDatabase.GetEmployee(txtFirstName.Text, txtLastName.Text);
			if (solicitude == null)
			{
				MessageBox.Show(string.Format("Employee {0} {1} already in records.", solicitude.FirstName, solicitude.LastName));
				return;
			}
			
            EmployeeDatabase.AddEmployee(txtFirstName.Text, txtLastName.Text, txtDepartment.Text, txtPhone.Text, txtEmail.Text, txtStartingDate.Text);
			//Load all employees
			allEmployees = EmployeeDatabase.GetAllEmployees();
			
            UtilWinforms.UpdateAutoComplete(ref allTextBoxControls, ref allEmployees);            //Update AutoComplete values

            //Some feedback on add employee
            MessageBox.Show("Added 1 employee to Employee table.");               

			//Update timesheet table with new employee - name and employee id, the rest of the fields empty
			Employee employee = EmployeeDatabase.GetEmployee(txtFirstName.Text, txtLastName.Text);

			//Add to timsheet table
			TimeSheetDatabase.AddWeekWork(employee.EmployeeID.ToString(), employee.FirstName, employee.LastName, WorkWeek.zeroDummy, WorkWeek.zeroDummy, WorkWeek.zeroDummy, WorkWeek.zeroDummy, WorkWeek.ellipsisDummy, WorkWeek.dashDummy, WorkWeek.dashDummy, employee.StartingDate);
			
            //Clear out the form
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtDepartment.Text = "";
            txtPhone.Text = "";
            txtEmail.Text = "";
            txtStartingDate.Text = "";
			
			this.Form2_Load(this, null);                            //Update listview

			MessageBox.Show("Employee added to timesheet records.\nYou can start now updating employee's work days!");
        }



        //Remove button event
        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (txtFirstName.Text == "" || txtLastName.Text == "")
            {
                MessageBox.Show("Missing data; please enter first and last name.");
                return;
            }

            Employee employee = EmployeeDatabase.GetEmployee(txtFirstName.Text, txtLastName.Text);
            if (employee.EmployeeID == 0)
            {
                MessageBox.Show("No employee found with that reference.");
                return;
            }
                        
            EmployeeDatabase.RemoveEmployee(txtFirstName.Text, txtLastName.Text);

            //Clear out the form
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtDepartment.Text = "";
            txtPhone.Text = "";
            txtEmail.Text = "";
            txtStartingDate.Text = "";

            this.Form2_Load(this, null);                        //Update listview

			//Add to timsheet table
			TimeSheetDatabase.RemoveWorkWeekWithName(employee.EmployeeID.ToString(), employee.FirstName);


            //Some more feedback on remove employee  
            MessageBox.Show("Removed 1 employee.");

        }
        

        //EmployeeForm - Handles tab and enter key presses for autocomplete purposes
        private void txt_EnterKeyUp(Object obj, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                if (txtFirstName.Text != "" && txtLastName.Text != ""
					)
                {
                    //Self auto-complete pattern
                    Employee employee = EmployeeDatabase.GetEmployee(txtFirstName.Text, txtLastName.Text);
                    //No employee record
                    if (employee.EmployeeID == 0)
                    {
                        return;
                    }
                    UtilWinforms.AutoCompleteEmpties(ref allTextBoxControls, employee);
                }
            }
        }

		//Updates Employee's information
		private void btnUpdate_Click(object sender, EventArgs e)
		{
			if (!UtilDotNET.IsMatch(UtilDotNET.phonePattern1, txtPhone.Text))
			{
				MessageBox.Show("Error: not a valid phone format.");
				return;
			}


			if (!UtilDotNET.IsMatch(UtilDotNET.emailPattern, txtEmail.Text))
			{
				MessageBox.Show("Error: invalid email format.");
				return;
			}


			if (!UtilDotNET.ValidateDate(txtStartingDate.Text) && txtStartingDate.Text != "")
			{
				MessageBox.Show("Invalid date format");
				return;
			}

			EmployeeDatabase.UpdateEmployee(txtFirstName.Text, txtLastName.Text, txtDepartment.Text, txtPhone.Text, txtEmail.Text, txtStartingDate.Text);

			MessageBox.Show(string.Format("Updated: {0} {1}", txtFirstName.Text, txtLastName.Text));

			//Update listview display
			this.Form2_Load(this, null);
		}
    }
}
