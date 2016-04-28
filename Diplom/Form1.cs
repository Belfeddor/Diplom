using System;
using System.Diagnostics;
using System.Windows.Forms;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;



namespace Diplom
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		private void OpenImage_Click(object sender, EventArgs e)
		{
			if (OpenImageDialog.ShowDialog() == DialogResult.OK)
			{
				//Show Source Image
				SourceImage src = new SourceImage();
				src.ImagePath = OpenImageDialog.FileName;
				Method1.Visible = true;
			}
		}

		private void Method1_Click(object sender, EventArgs e)
		{
			if(OpenImageDialog.FileName != null)
			{
				//A timer for determing the duration of the algorithm 
				Stopwatch timer = new Stopwatch();
				timer.Start();

				Method1 method1 = new Method1();
				// Execute the first algorithm
				method1.ImagePath = OpenImageDialog.FileName;

				//method1.ImagePath = "E:\\Роботы\\6.jpg";
				timer.Stop();
				ResultTextBox.Text += method1.ImagePath + "Время выполнения первого алгоритма: "+ timer.ElapsedMilliseconds + " мс\r\n";
				ResultTextBox.SelectionStart = ResultTextBox.Text.Length;
				ResultTextBox.ScrollToCaret();
			}
		}

		private void Method2_Click(object sender, EventArgs e)
		{

		}
	}

	public struct RobodromProperties
	{
		public int height;
		public int length;
		public int width;
	}

	public struct NXTProperties
	{
		public int length;
		public int width;
		public int LCDlength;
		public int LCDwidth;
		public int toLCDdistance;
	}
}
