using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace TimesheetServerless
{
	public class BaseForm : Form
	{
		public BaseForm()
		{
			this.Width = 900;
			this.Height = 500;
			this.StartPosition = FormStartPosition.CenterScreen;

			this.FormBorderStyle = FormBorderStyle.FixedSingle;		//NO resizeable
			this.MaximizeBox = false;

			this.AutoScroll = true;

			
		}
	}
}
