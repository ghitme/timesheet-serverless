using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;


namespace TimesheetServerless
{
	public class ScheduleForm: BaseForm
	{
		readonly int TOTAL_ROWS;					//Number of employees in weekly schedule
		readonly int TOTAL_COLS = 7;

		int oldTextSize = 0;						//Hold measure of text in textbox, resize purposes

		List<Employee> allEmployees;

		//Form layout
		public ScheduleForm()
		{
			allEmployees = EmployeeDatabase.GetAllEmployees();
			
			#region table layout calculations
			TableLayoutPanel table = new TableLayoutPanel();
			table.SuspendLayout();

			table.Location = new Point(40, 140);
			table.Size = new Size(800, 300);
			table.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
			

			//Creating cells - TextBoxes
			TOTAL_ROWS = EmployeeDatabase.GetTotalEmployees();	//Get total number of employees - rows
			for (int i = 0; i < TOTAL_ROWS; i++)
			{
				table.RowStyles.Add(new RowStyle(SizeType.Percent, 50.0f));
				for (int j = 0; j < TOTAL_COLS + 1; j++)
				{
					table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50.0f));

					table.Controls.Add(new TextBox() { 
						Dock = DockStyle.Fill,
						Anchor = AnchorStyles.Top | AnchorStyles.Left,
						Font = new Font("Arial", 12, FontStyle.Regular),
						Name = string.Format("{0},{1}", j, i)						
					},
										j, i);

					//Setting specific textboxes 
					Control c = table.GetControlFromPosition(j, i);
					if (j == 0)
					{						
						if (i > 0 && allEmployees[i] != null)
						{

							c.Text = allEmployees[i].FirstName + " " + allEmployees[i].LastName;

							//Change column width only if text is greater that last greater text
							int newTextSize = TextRenderer.MeasureText(c.Text, c.Font).Width - 70;
							if (newTextSize > oldTextSize)
							{
								table.ColumnStyles[0].Width = newTextSize;
								oldTextSize = newTextSize;
							}
							
						}
						else
							c.Text = "Employee";
					}
					else if(j > 0 && i == 0)		//Days Labels
					{
						c.Text = (Day)(j - 1) + "";

					}
						
				}
			}

			table.ResumeLayout();
			table.Show();

			this.Controls.Add(table);
			#endregion		


			#region Print linklabel control
			LinkLabel printPage = new LinkLabel();
			printPage.Text = "Print";
			printPage.Font = new Font("Arial", 10, FontStyle.Regular);

			printPage.Location = new Point(table.Bounds.Right - 50, 20);
			printPage.Bounds = new Rectangle(printPage.Location,
				new Size(50, 20));			

			//Assign event print
			printPage.LinkClicked += new LinkLabelLinkClickedEventHandler(lnkPrint_LinkClicked);

			this.Controls.Add(printPage);
			#endregion

		}

		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// ScheduleForm
			// 
			this.ClientSize = new System.Drawing.Size(282, 257);
			this.Name = "ScheduleForm";
			this.ResumeLayout(false);

		}

		//Printer event
		private void lnkPrint_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			PrinterForm printer = new PrinterForm(this);
		}
	}
}
