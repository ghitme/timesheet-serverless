using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Globalization;
using System.Diagnostics;
using System.IO;


namespace TimesheetServerless
{
	public partial class TimeSheetForm : Form
	{
		//Data for autocomplete
		List<Employee> allEmployees;                                        //Create dropdown list with all employee names
		List<Control> allCells = new List<Control>();                       //List cells
		List<Control> allTextBoxControlsSearch = new List<Control>();       //Text boxes for first and last name, and id

		List<WorkWeek> employeeWorkWeekList;									//Single employee all workweeks

		string[][] grid;                                      //What it is actually in display    
		string[] LongColumns;													//Columns ready for SQL input
		WorkWeek currentWorkWeek;
		const int MAX_COL = 7;
		const int MAX_ROW = 7;

		const string dummies = "0000 ... --";
		const string timeAreas = "PunchIn LunchIn LunchOut PunchOut";

		public bool CanIChangeLayout { get; set; }
		public TimeSheetForm()
		{
			InitializeComponent();

			allEmployees = EmployeeDatabase.GetAllEmployees();
			List<Control> tempCells = UtilWinforms.GetAllControlsOfType(this, typeof(TextBox)).ToList();
			foreach (TextBox e in tempCells)
			{
				//By default Cells (grid textboxes) do have null tags
				if (e.Tag == null)
				{
					allCells.Add(e);

					//Hint type of cells
					CheckForDummies(e);

					//Event handlers assignments
					e.Click += new EventHandler(cell_Click);
					e.Leave += new EventHandler(cell_Leave);

					e.Enter += new EventHandler(cell_Click);

					//Set autocomplete for all cells
					e.AutoCompleteMode = AutoCompleteMode.Suggest;
					e.AutoCompleteSource = AutoCompleteSource.CustomSource;

					AutoCompleteStringCollection data = new AutoCompleteStringCollection();
					data.Add("0700");
					data.Add("0800");
					data.Add("0800");
					data.Add("0900");
					data.Add("0930");
					data.Add("1030");
					data.Add("1100");
					data.Add("1200");
					data.Add("1230");
					data.Add("1400");
					data.Add("1430");
					data.Add("1630");
					data.Add("1700");
					e.AutoCompleteCustomSource = data;

					//Parsing cell to gather info
					WorkWeek.GetIndexes(ref grid);

				}
				else if (e.Tag.ToString() == "Search")
				{
					allTextBoxControlsSearch.Add(e);
				}
			}
			#region Initialize text boxes with name with autocomplete (reflection)

			//Populate first and last name text boxes autocomplete
			UtilWinforms.UpdateAutoComplete(ref allTextBoxControlsSearch, ref allEmployees);
			#endregion

			//Assign delegate events to all menu items
			foreach (ToolStripMenuItem item in menu.DropDownItems)
			{
				item.Click += new EventHandler(DropDownItemClicked);
			}

			//Event for print page, assigned to linklabel control
			lblPrint.LinkClicked += new LinkLabelLinkClickedEventHandler(lblPrint_LinkClicked);
		}


		//Some housekeeping and updates 
		private void Form1_Load(object sender, EventArgs e)
		{
			//Select mouse to start at txtName
			txtFirstName.Select();

			cmbEmployeeList.Items.Clear();
			allEmployees = EmployeeDatabase.GetAllEmployees();					//When exit EmployeeList, load Timesheet form, and check list again
			//Populate drop down list for combobox with all employees
			for (int i = 0; i < allEmployees.Count; i++)
			{
				cmbEmployeeList.Items.Add(allEmployees[i].FirstName.Trim() + " " + allEmployees[i].LastName);
				
			}			

		}


		private void menuItem1_Click(object sender, EventArgs e)
		{
			//UpdateMenuItem(sender, e);

		}


		private void menuItem2_Click(object sender, EventArgs e)
		{
			if (!EmployeeForm.ActiveForm.IsAccessible)
			{
				//UpdateMenuItem(sender, e);
				//if (menuItem2.Checked)
				//	PopForm2();
			}
		}


		//Form2 only possible with menuItem2 checked true
		//Desc: Resets menuItem1 as selected
		private void UpdateMenu_Form2Leave(object sender, EventArgs e)
		{

			foreach (ToolStripMenuItem item in menu.DropDownItems)
			{
				//Enable all menu items
				item.Enabled = true;

				//Check only menu item corresponding to this form
				if (item == menuItem1)
				{
					item.Checked = true;
				}
				else
				{
					item.Checked = false;
				}
			}
			
			this.Form1_Load(this, null);
		}


		//Just hide first layout, by hiding group box, then pop form2 and assign to its closing event a function
		private void PopForm2()
		{
			//Test all controls            
			box2.Hide();

			EmployeeForm form2 = new EmployeeForm();
			form2.Location = new Point(100, 100);
			form2.BringToFront();

			form2.Show();

			//Assign function to event leave form
			form2.FormClosed += new FormClosedEventHandler(UpdateMenu_Form2Leave);

			//Disable menu items on this, form1
			menuItem1.Enabled = false;
			menuItem2.Enabled = false;
			//menuItem3.Enabled = false;
		}


		void PopScheduleForm()
		{
			//Test all controls            
			box2.Hide();

			ScheduleForm schedule = new ScheduleForm();
			schedule.Location = new Point(100, 100);
			schedule.BringToFront();

			schedule.Show();

			//Assign function to event leave form
			schedule.FormClosed += new FormClosedEventHandler(UpdateMenu_Form2Leave);

			//Disable menu items on this, form1
			menuItem1.Enabled = false;
			menuItem2.Enabled = false;
			//menuItem3.Enabled = false;
		}


		//- Menu clear check mark
		private void UpdateMenuItem(object sender, EventArgs e)
		{
			ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;             //Grab menu item that produced event

			//Clear all submenus
			ClearSubmenuItems(menuItem);

			//Check only the menu item clicked
			menuItem.Checked = true;
		}


		//-- Utilities
		private void ClearSubmenuItems(ToolStripMenuItem myMenuItem)
		{
			if (!myMenuItem.Checked)
			{
				//Clear menu items
				foreach (ToolStripMenuItem item in menu.DropDownItems)
				{
					item.Checked = false;
				}
			}
		}


		private void logo_Click(object sender, EventArgs e)
		{

		}


		private void menuItem1_CheckedChanged(object sender, EventArgs e)
		{
			//ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
			//if (menuItem.Checked)
			//	UpdateMenuItem(sender, e);

			//Show first layout
			box2.Show();
		}


		//Clear cells on click
		private void cell_Click(object sender, EventArgs e)
		{
			TextBox cell = (TextBox)sender;
			if(allCells.Contains(cell))
			{
				//If cell has dummy
				if (dummies.IndexOf(cell.Text) != -1 )
				{
					cell.Text = "";
					cell.ForeColor = SystemColors.WindowText;           //WindowText is the default color for text boxes				
				}
				
				if(UtilDotNET.ValidateMilitaryTextFormat(cell.Text))
				{
					//Upgrade grid cell with new info
					GridCellUpdate(cell);
					//Display Hours
					double hrs = CalculateHours();
					lblTotal.Text = Math.Round(hrs, 1).ToString();
				}
			}			
						
		}


		//Reset "hint" on leave if nothing was written
		private void cell_Leave(object sender, EventArgs e)
		{
			TextBox cell = (TextBox)sender;

			//Reset default value on leave, if nothing was written
			CheckForDummies(cell);
								
			//If military time, and in columns of military time, then proceed
			if (timeAreas.IndexOf(ParseCellName(cell.Name)[2]) != -1  && cell.Text != "")
			{
				if (!UtilDotNET.ValidateMilitaryTextFormat(cell.Text))							//Leaving empty cell
				{
					cell.Text = "0000";
					cell.ForeColor = Color.DarkGray;
				}
				else
				{
					//Update grid cell with valid time input
					GridCellUpdate(cell);

					//Check valid time input: greater than previous
					//- Date format known to be valid (tryparse exact in validatemilitary...)
					DateTime dateLeaving = DateTime.ParseExact(cell.Text, "HHmm", CultureInfo.InvariantCulture, DateTimeStyles.None);
					
					Point pos = GridPos(cell);
					int movingLeft = pos.Y; 
					while (movingLeft > 0)								//Columns to the left
					{
						movingLeft--;										//Starting for first column to the left of current cell
						string comparisonText = grid[pos.X][movingLeft];	//Text from left cell

						DateTime tempDate;
						if(DateTime.TryParseExact(comparisonText, "HHmm", CultureInfo.InvariantCulture, DateTimeStyles.None, out tempDate))
						{							
							//Pass - text is military time
							if (tempDate > dateLeaving)							//Break; after check against a military time
							{
								cell.Text = "0000";
								cell.ForeColor = Color.DarkGray;
							}
							break;												
						}						
					}					
				}				
			}
		}

		//Click from instructions menu item

		//Click from about menu item


		private void btnBack_Click(object sender, EventArgs e)
		{
			//Move to next workWeek when not in last week
			int index = employeeWorkWeekList.IndexOf(currentWorkWeek);
			if (index == 0)				//Just exit if cannot proceed
				return;

			cmbWeekSpan.SelectedItem = cmbWeekSpan.Items[index - 1];

			//Display Hours
			double hrs = CalculateHours();
			lblTotal.Text = Math.Round(hrs, 1).ToString();
		}


		/*	==================================================================
		 *	btnForth_click event
		 *	*****************************************************************
		 *	Description: Creates a new WorkWeek, only 1 above current date
		 *	Notes:
		 *	1. Create new WorkWeek with EmployeeID, FirstName, LastName, 
		 *	empty data cells (like when in EmployeeForm).
		 *	2. Set Week to be +7 days from current date.
		 *	3. Display (includes add to weekspan combobox)
		 *	4. Only 1 week above current date allowed.
		 *	
		 *	==================================================================
		 */
		private void btnForth_Click(object sender, EventArgs e)
		{
			//Prevent logic error - user changes names but does not click 'open records'
			if (CheckValidTransition())
			{
				//Correct the error - user change dropdown list selection but did not open 
				txtFirstName.Text = currentWorkWeek.FirstName;
				txtLastName.Text = currentWorkWeek.LastName;
			}				
			else
				return;


			//Move to next workWeek when not in last week
			int index = employeeWorkWeekList.IndexOf(currentWorkWeek);

			if (index == (employeeWorkWeekList.Count - 1))					//Last item
			{
				//Add new WorkWeek to table TimeSheet, with Week +7 of current week ()
				//--Update timesheet table with new employee - name and employee id, the rest of the fields empty
				DateTime dateNextWeek;
				if (DateTime.TryParse(currentWorkWeek.Week, out dateNextWeek))
				{
					dateNextWeek = dateNextWeek.AddDays(7);
				}

				//Add to timesheet table
				TimeSheetDatabase.AddWeekWork(currentWorkWeek.EmployeeID.ToString(), currentWorkWeek.FirstName, currentWorkWeek.LastName, WorkWeek.zeroDummy, WorkWeek.zeroDummy, WorkWeek.zeroDummy, WorkWeek.zeroDummy, WorkWeek.ellipsisDummy, WorkWeek.dashDummy, WorkWeek.dashDummy, dateNextWeek.ToString());

				//Update employee WorkWeek list, and set currentWorkWeek
				employeeWorkWeekList = TimeSheetDatabase.GetAllWorkWeekForSingleEmployee(txtFirstName.Text, txtLastName.Text);
				currentWorkWeek = employeeWorkWeekList.Last();
				//MessageBox.Show(currentWorkWeek.Week.ToString());

				//Display			
				UpdateWeekComboBox(currentWorkWeek);
				Display(currentWorkWeek);
				
				//Display Hours
				double hrs = CalculateHours();
				lblTotal.Text = Math.Round(hrs, 1).ToString();

				//Feedback
				MessageBox.Show(string.Format("Added new Time Sheet for employee {0} {1}", currentWorkWeek.FirstName, currentWorkWeek.LastName));
			}	
			else
			{
				//Change combobox
				cmbWeekSpan.SelectedItem = cmbWeekSpan.Items[index + 1];
			}
			
		}

		

		//Updates first and last names textboxes every time users change the combobox dropdown list 
		private void cmbEmployeeList_SelectedValueChanged(object sender, EventArgs e)
		{
			txtFirstName.Text = cmbEmployeeList.SelectedItem.ToString().Split()[0];
			txtLastName.Text = cmbEmployeeList.SelectedItem.ToString().Split()[1];
		}

		
		/*
		 * Open and Save events
		 */
		//Display current week of data
		private void btnOpen_Click(object sender, EventArgs e)
		{
			if (txtFirstName.Text != "" || txtLastName.Text != "")
			{
				//Update for autocompletion...
				employeeWorkWeekList = TimeSheetDatabase.GetAllWorkWeekForSingleEmployee(txtFirstName.Text, txtLastName.Text);

				//Exit on empty list
				if (employeeWorkWeekList.Count == 0)
					return;

				currentWorkWeek = WorkWeek.GetCurrentWorkWeek(employeeWorkWeekList);					

				//Split the string array first, before putting them in cells
				Display(currentWorkWeek);				
				UpdateWeekComboBox(currentWorkWeek);

				//Display Hours
				double hrs = CalculateHours();
				lblTotal.Text = Math.Round(hrs, 3).ToString();

				//Feedback
				MessageBox.Show(string.Format("WorkWeek for employee:\n{0} {1}.", currentWorkWeek.FirstName.Trim(), currentWorkWeek.LastName.Trim()));

			}
			else
			{
				MessageBox.Show("Missing data! Please enter employee's first and last name.");
			}
		}
		

		/*
		 * =============================================================
		 * Save event
		 * 
		 * 
		 * =============================================================
		 */
		private void btnSave_Click(object sender, EventArgs e)
		{
			//Loops through allCells, storing e.Text in currentGridDisplay; uses dictionaries and ParseCellName
			UpdateGrid();

			//Display Hours
			double hrs = CalculateHours();
			lblTotal.Text = Math.Round(hrs, 1).ToString();

			if(CheckValidTransition())
			{
				//Updates workweek for specific tableid
				PrepareLongColumns();

				//Call TimeSheetDatabase Update function
				TimeSheetDatabase.UpdateTimeSheet(currentWorkWeek.TableID, LongColumns[0], LongColumns[1], LongColumns[2], LongColumns[3], LongColumns[4], LongColumns[5], LongColumns[6]);
			}
			else
			{
				//Correct the error - user change dropdown list selection but did not open 
				txtFirstName.Text = currentWorkWeek.FirstName;
				txtLastName.Text = currentWorkWeek.LastName;
			}
		}


		/*	================================================================================
		 *	Event for combo box items change
		 *	- Includes display
		 *	- Changes current workweek
		 *	================================================================================
		 */
		private void cmbWeekSpan_SelectedValueChanged(object sender, EventArgs e)
		{
			//When initialized disable buttons save, back, and forth.
			if (cmbWeekSpan.Text == "")
			{
				btnSave.Enabled = false;
				btnBack.Enabled = false;
				btnForth.Enabled = false;
			}
			else
			{
				btnSave.Enabled = true;
				btnBack.Enabled = true;
				btnForth.Enabled = true;

				//Disable forth 
				DateTime cmbDate = WorkWeek.LastBeginningOfWeek(currentWorkWeek.Week);		//Date we are in
				DateTime cmbMonday = WorkWeek.LastBeginningOfWeek(cmbDate.ToString());

				DateTime date = UtilDotNET.StringToDate(WorkWeek.ConvertWeekSpanToBeginningOfWeek(cmbWeekSpan.Text));	//Date we want to go to (because it change)
				//MessageBox.Show(cmbMonday + " " + date);
				//Only execute on different date selections
				if (cmbMonday != date)
				{
					//Load corresponding week
					int currentIndex = cmbWeekSpan.Items.IndexOf(cmbWeekSpan.Text);
					currentWorkWeek = employeeWorkWeekList[currentIndex];
					Display(currentWorkWeek);
				}
			}
		}


		/*	==========================================================
		 * Timesheet database function 'UpdateGrid'
		 * DESCRIPTION: Update grid, and calculateTime functions
		 *	==========================================================
		 */
		private void UpdateGrid()
		{
			//Loop through cells setting current grid display 
			foreach (TextBox e in allCells)
			{
				string[] cellParse = ParseCellName(e.Name);
				grid[(int)WorkWeek.daysDict[cellParse[1]]][(int)WorkWeek.recsDict[cellParse[2]]] = e.Text;			
			}
		}


		/*
		 * ===================================================
		 * Create 'columns' for SQL update
		 * - Initialize Long Columns with Column Major order
		 * 
		 * ===================================================
		 */
		private void PrepareLongColumns()
		{
			LongColumns = new string[MAX_COL];

			for (int i = 0; i < MAX_COL; i++)
			{
				for (int j = 0; j < MAX_ROW; j++)
				{
					LongColumns[i] += grid[j][i] + " ";					//Column major order...
				}
			}
		}


		//Update changes in cells
		//Split the string array first, before putting them in cells
		private void Display(WorkWeek workWeek)
		{
			Type type = typeof(WorkWeek);

			//Initialize grid Search for all text boxes     
			for (int i = 0; i < allCells.Count(); i++)
			{
				//Reflection; get field inside employee of name stem
				PropertyInfo property = type.GetProperty(allCells[i].Name.Substring(6));   //Calling property of name

				if (property != null && property.GetValue(workWeek, null).ToString().Trim() != String.Empty)
				{
					string[] cellParse = ParseCellName(allCells[i].Name);
					string[] tempPropertyArr = (string[])property.GetValue(workWeek, null);

					//Assign row values to grid - property based on column
					grid[(int)WorkWeek.daysDict[cellParse[1]]][(int)WorkWeek.recsDict[cellParse[2]]] = tempPropertyArr[(int)WorkWeek.daysDict[cellParse[1]]];
					allCells[i].Text = grid[(int)WorkWeek.daysDict[cellParse[1]]][(int)WorkWeek.recsDict[cellParse[2]]];

					//Check for dummies and apply format to them
					CheckForDummies((TextBox)allCells[i]);
				}
			}

			//Upgrade grid
			//UpdateGrid();

			//Display Hours
			double hrs = CalculateHours();
			lblTotal.Text = Math.Round(hrs, 1).ToString();
		}

		
		//After work week modification, and change selected item
		private void UpdateWeekComboBox(WorkWeek workWeek)
		{
			//Set combo box for weeks
			cmbWeekSpan.Items.Clear();
			for (int i = 0; i < employeeWorkWeekList.Count; i++)
			{
				cmbWeekSpan.Items.Add(WorkWeek.DateToWeekSpan(employeeWorkWeekList[i].Week));		//Call convert date before entering val
			}
			cmbWeekSpan.SelectedItem = cmbWeekSpan.Items[employeeWorkWeekList.IndexOf(workWeek)];
		}



		//Click about menu
		private void about_Click(object sender, EventArgs e)
		{
			Form aboutForm = new Form();
			aboutForm.SuspendLayout();
			aboutForm.StartPosition = FormStartPosition.CenterScreen;
			aboutForm.Size = new Size(600, 400);
			aboutForm.FormBorderStyle = FormBorderStyle.FixedSingle;
			
			//Change icon here

			try
			{
				
				

			}
			catch (Exception exc)
			{

				throw exc;
			}
			


			RichTextBox lblItem = new RichTextBox();
			lblItem.ReadOnly = true;
			lblItem.BorderStyle = BorderStyle.None;

			lblItem.Location = new Point(10, 50);
			lblItem.Size = new Size(550, 300);
			lblItem.Font = new Font("Serif", 12, FontStyle.Regular);
			//lblItem.Dock = DockStyle.Fill;
			//lblItem.AutoSize = true;
			lblItem.Text = "Portfolio - TimeSheet Serverless Project\nAuthor: Alfredo Almenares\n" +
				"Description: Winforms, .Net, SQLite Database with 2 tables managed by 2 applications:\n\n" +
				"1st. TimeSheet application to load employees' information on their total of hours per week, and days worked.\n\n" +
				"2nd. Employee List application to add employees' and their personal information (Names, Emails, Phone, and Starting Date)\n" +
				"or remove them.\n\n" +
				"Note: TimesheetPortfolio Serverless version";

			aboutForm.Controls.Add(new Label() { Text = "About TimeSheet Application", AutoSize = true, Location = new Point(10, 10), Font = new Font("Serif", 12, FontStyle.Bold), BorderStyle = BorderStyle.None });
			aboutForm.Controls.Add(lblItem);
			lblItem.Enabled = false;
			lblItem.SelectAll();
			lblItem.SelectionColor = Color.Black;
			aboutForm.ResumeLayout();
			aboutForm.Show();
		}


		/* ==================================================================
		 * Parse cell name into prefix, stem, and suffix - used for reflection
		 * INPUT: cell name
		 * OUTPUT: corresponding array value
		 */
		private string[] ParseCellName(string name)
		{
			return new string[] { name.Substring(0, 3), name.Substring(3, 3), name.Substring(6) };			
		}


		/*
		 * Provides translation from textboxes (list allcells) to grid representation (sort of )
		 */
		//Get row index
		private int GetRow(TextBox cell)
		{
			string rowStr = ParseCellName(cell.Name)[1];
			return (int)WorkWeek.daysDict[rowStr];		
		}

		//Get col index
		private int GetCol(TextBox cell)
		{
			string name = ParseCellName(cell.Name)[2];
			return (int)WorkWeek.recsDict[name];
		}


		//Get grid position given cell
		private Point GridPos(TextBox cell)
		{
			int row = GetRow(cell);
			int col = GetCol(cell);
			return new Point(row, col);
		}
		

		//Cell position, grid equivalent
		private string GridTranslate(TextBox cell)
		{
			int row = GetRow(cell);
			int col = GetCol(cell);
			return grid[row][col];
		}
		/////////////////////////////////////////////////////////////////////////////////////////



		//Update Grid
		private void GridCellUpdate(TextBox cell)
		{
			Point p = GridPos(cell);
			grid[p.X][p.Y] = cell.Text;
		}



		/*	==============================================================
		 *	CalculateTime
		 *  Description: Loops through all texboxes (cells); calculate difference as dates: (delta time).TotalHours
		 *  
		 *	Notes:
		 *	- Uses grid, updated in UpdateGrid function
		 *	- Row Major Order
		 *	> Outer loop for rows, intermed loop for col, inner loop for offsets
		 *  ==============================================================
		 */
		private double CalculateHours()
		{			
			double[] total = new double[grid.Length];						//Totals per row
			double sum = 0;													//Get sum of each int elem in array
			for (int i = 0; i < grid.Length; i++)
			{
				DateTime oldDate;
				DateTime currentDate;
				for (int j = 0; j < grid[i].Length; j++)					//Find first date down each row
				{
					if (DateTime.TryParseExact(grid[i][j], "HHmm", CultureInfo.InvariantCulture, DateTimeStyles.None, out oldDate)
						&& grid[i][j] != "0000")
					{						
						int counter = j + 1;
						for (; counter < grid[i].Length; counter++)				//Find following date to the right
						{
							if (DateTime.TryParseExact(grid[i][counter], "HHmm", CultureInfo.InvariantCulture, DateTimeStyles.None, out currentDate)
								&& grid[i][counter] != "0000")
							{
								//Date calculation
								total[i] += (currentDate - oldDate).TotalHours;			
								oldDate = currentDate;						//oldDate is now currentDate								
							}	
						}
						break;
					}					
				}
				sum += total[i];
			}
			return sum;
		}


		//Set Default dummies - 0000 ... --
		private void CheckForDummies(TextBox cell)
		{
			//Cell text has any dummy
			if (dummies.IndexOf(cell.Text) != -1)
			{
				int col = GetCol(cell);
				if (col < 4)
					cell.Text = "0000";
				else if (col == 4)
					cell.Text = "...";
				else
					cell.Text = "--";

				//Remember: Windows Text is the default color for text boxes
				cell.ForeColor = Color.DarkGray;
			}
			else
				cell.ForeColor = SystemColors.WindowText;
		}


		//Check if there is only dummies - ie the workweek sheet is empty
		private bool IsEmptyWorkWeek()
		{
			bool hasChange = false;
			foreach (TextBox e in allCells)
			{	
				if (dummies.IndexOf(e.Text) != -1)				//Dummy ...
				{
					hasChange = true;		
					break;										//... on at least 1 cell
				}	
			}
			return hasChange;
		}


		/*
		 * ===================================================================
		 * Logic Error Catcher 
		 * ---------------------------------------------------------
		 * CASE: Saving, Back, and Forth button presses
		 * with name display different to currentWorkWeek		 
		 * 
		 * ===================================================================
		 */
		private bool CheckValidTransition()
		{
			//Both first and last names are the same for the current work week, and the displayed texts
			if (currentWorkWeek.FirstName == txtFirstName.Text && currentWorkWeek.LastName == txtLastName.Text)
			{
				return true;			
			}
			else
				return false;
		}


		//Printer event
		private void lblPrint_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			PrinterForm printer = new PrinterForm(this);
		}


		//Handles menu items clicks
		private void DropDownItemClicked(object sender, EventArgs e)
		{
			ToolStripMenuItem  item = (ToolStripMenuItem)sender;
			foreach (ToolStripMenuItem elem in menu.DropDownItems)
			{
				if (elem == item)
				{
					item.Checked = true;

					//Pop form 2
					if (menuItem2.Checked)
						PopForm2();

					//if (menuItem3.Checked)
					//	PopScheduleForm();
				}
				else
					elem.Checked = false;
			}
		}

	}
}
 