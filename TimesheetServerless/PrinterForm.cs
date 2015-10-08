using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Imaging;


namespace TimesheetServerless
{
	class PrinterForm : Form
	{
		private PrintDocument printDoc = new PrintDocument();		//Printable object
		Bitmap bitmap;												//Bitmap image
		Size formSize;

		public PrinterForm(Form parent)
		{
			CaptureScreen(parent);
			printDoc.PrintPage += new PrintPageEventHandler(printDoc_PrintPage);

			//Dialog to choose printer
			PrintDialog printDialog = new PrintDialog();
			printDialog.AllowSomePages = true;
			printDialog.ShowHelp = true;
			printDialog.Document = printDoc;
			DialogResult result = printDialog.ShowDialog();

			//If click OK button
			if (result == DialogResult.OK)
				printDoc.Print();
		}

		//Prepare for print and save image
		private void CaptureScreen(Form parent)
		{
			Graphics myGraphics = parent.CreateGraphics();
			formSize = parent.Size;
			bitmap = new Bitmap(formSize.Width, formSize.Height, myGraphics);
			myGraphics.Dispose();
						
			Graphics graphics = Graphics.FromImage(bitmap);
			graphics.CopyFromScreen(parent.Location.X, parent.Location.Y, 0, 0, formSize);
			
		}


		//Print event
		private void printDoc_PrintPage(System.Object sender,  
			   System.Drawing.Printing.PrintPageEventArgs e)
		{
			//Calculate width and height
			var wScale = e.PageBounds.Width / (float)bitmap.Width;
			var hScale = e.PageBounds.Height / (float)bitmap.Height;

			//Choose the smaller of the two scales
			var scale = wScale < hScale ? wScale : hScale;

			//Apply scaling to the image
			e.Graphics.ScaleTransform(scale, scale);

			e.Graphics.DrawImage(bitmap, 0, 0);
		}
	}
}
