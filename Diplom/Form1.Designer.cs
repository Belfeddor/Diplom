namespace Diplom
{
	partial class MainForm
	{
		/// <summary>
		/// Обязательная переменная конструктора.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Освободить все используемые ресурсы.
		/// </summary>
		/// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Код, автоматически созданный конструктором форм Windows

		/// <summary>
		/// Требуемый метод для поддержки конструктора — не изменяйте 
		/// содержимое этого метода с помощью редактора кода.
		/// </summary>
		private void InitializeComponent()
		{
			this.OpenImage = new System.Windows.Forms.Button();
			this.OpenImageDialog = new System.Windows.Forms.OpenFileDialog();
			this.Method1 = new System.Windows.Forms.Button();
			this.ResultTextBox = new System.Windows.Forms.TextBox();
			this.Method2 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// OpenImage
			// 
			this.OpenImage.Location = new System.Drawing.Point(100, 25);
			this.OpenImage.Name = "OpenImage";
			this.OpenImage.Size = new System.Drawing.Size(100, 50);
			this.OpenImage.TabIndex = 0;
			this.OpenImage.Text = "Выберите кадр";
			this.OpenImage.UseVisualStyleBackColor = true;
			this.OpenImage.Click += new System.EventHandler(this.OpenImage_Click);
			// 
			// OpenImageDialog
			// 
			this.OpenImageDialog.FileName = "Выберите кадр";
			// 
			// Method1
			// 
			this.Method1.Location = new System.Drawing.Point(100, 100);
			this.Method1.Name = "Method1";
			this.Method1.Size = new System.Drawing.Size(100, 50);
			this.Method1.TabIndex = 1;
			this.Method1.Text = "Первый метод";
			this.Method1.UseVisualStyleBackColor = true;
			this.Method1.Visible = false;
			this.Method1.Click += new System.EventHandler(this.Method1_Click);
			// 
			// ResultTextBox
			// 
			this.ResultTextBox.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.ResultTextBox.Location = new System.Drawing.Point(0, 261);
			this.ResultTextBox.Multiline = true;
			this.ResultTextBox.Name = "ResultTextBox";
			this.ResultTextBox.ReadOnly = true;
			this.ResultTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.ResultTextBox.Size = new System.Drawing.Size(284, 150);
			this.ResultTextBox.TabIndex = 2;
			// 
			// Method2
			// 
			this.Method2.Location = new System.Drawing.Point(100, 175);
			this.Method2.Name = "Method2";
			this.Method2.Size = new System.Drawing.Size(100, 50);
			this.Method2.TabIndex = 3;
			this.Method2.Text = "Второй метод";
			this.Method2.UseVisualStyleBackColor = true;
			this.Method2.Visible = false;
			this.Method2.Click += new System.EventHandler(this.Method2_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 411);
			this.Controls.Add(this.Method2);
			this.Controls.Add(this.ResultTextBox);
			this.Controls.Add(this.Method1);
			this.Controls.Add(this.OpenImage);
			this.Icon = global::Diplom.Properties.Resources.icon;
			this.Name = "MainForm";
			this.Text = "Локализация роботов с помощью ТЗ";
			this.TopMost = true;
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button OpenImage;
		private System.Windows.Forms.OpenFileDialog OpenImageDialog;
		private System.Windows.Forms.Button Method1;
		private System.Windows.Forms.TextBox ResultTextBox;
		private System.Windows.Forms.Button Method2;
	}
}

