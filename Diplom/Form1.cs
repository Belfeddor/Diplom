using System;
using System.Diagnostics;
using System.Windows.Forms;

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
				//Открываем исходное изображение
				SourceImage src = new SourceImage();
				src.ImagePath = OpenImageDialog.FileName;
				Method1.Visible = true;
				Method2.Visible = true;
			}
		}

		private void Method1_Click(object sender, EventArgs e)
		{
			if(OpenImageDialog.FileName != null)
			{
				//Инициализируем таймер, определяющий время исполнения алгоритма 
				Stopwatch timer = new Stopwatch();
				timer.Start();

				Method1 method1 = new Method1();
				//Выполняем первый алгоритм
				method1.ImagePath = OpenImageDialog.FileName;

				//Останавливаем таймер
				timer.Stop();
				//Выводим результат выполнения программы в ResultTextBox на MainForm
				ResultTextBox.Text = "";
				//ResultTextBox.Text += method1.ImagePath + "Время выполнения первого алгоритма: 27"+ timer.ElapsedMilliseconds + " мс\r\n";
				ResultTextBox.Text += method1.ImagePath + "Время выполнения первого алгоритма: " + timer.ElapsedMilliseconds + " мс\r\n";
				ResultTextBox.SelectionStart = ResultTextBox.Text.Length;
				ResultTextBox.ScrollToCaret();
			}
		}

		private void Method2_Click(object sender, EventArgs e)
		{
			if (OpenImageDialog.FileName != null)
			{
				//Инициализируем таймер, определяющий время исполнения алгоритма 
				Stopwatch timer = new Stopwatch();
				timer.Start();

				Method1 method2 = new Method1();
				//Выполняем второй алгоритм
				method2.ImagePath = OpenImageDialog.FileName;

				//Останавливаем таймер
				timer.Stop();

				//Выводим результат выполнения программы в ResultTextBox на MainForm
				ResultTextBox.Text = "";
				ResultTextBox.Text += method2.ImagePath + "Время выполнения первого алгоритма: " + timer.ElapsedMilliseconds + " мс\r\n";
				ResultTextBox.SelectionStart = ResultTextBox.Text.Length;
				ResultTextBox.ScrollToCaret();
			}
		}
	}

	//Используем для подгрузки параметров робота из xml-файла 
	public struct NXTProperties
	{
		public int length;
		public int width;
		public int NXTlength;
		public int NXTwidth;
		public int LCDlength;
		public int LCDwidth;
		public int fromFrontToNXT;
		public int fromLeftToNXT;
		public int fromFrontNXTtoLCD;
		public int fromLeftNXTtoLCD;
	}
}
