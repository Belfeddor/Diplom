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
			this.MethodCanny = new System.Windows.Forms.Button();
			this.ResultTextBox = new System.Windows.Forms.TextBox();
			this.OpenModelDialog = new System.Windows.Forms.OpenFileDialog();
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
			// MethodCanny
			// 
			this.MethodCanny.Location = new System.Drawing.Point(100, 100);
			this.MethodCanny.Name = "MethodCanny";
			this.MethodCanny.Size = new System.Drawing.Size(100, 50);
			this.MethodCanny.TabIndex = 1;
			this.MethodCanny.Text = "Метод Кэнни";
			this.MethodCanny.UseVisualStyleBackColor = true;
			this.MethodCanny.Visible = false;
			this.MethodCanny.Click += new System.EventHandler(this.MethodCanny_Click);
			// 
			// ResultTextBox
			// 
			this.ResultTextBox.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.ResultTextBox.Location = new System.Drawing.Point(0, 211);
			this.ResultTextBox.Multiline = true;
			this.ResultTextBox.Name = "ResultTextBox";
			this.ResultTextBox.ReadOnly = true;
			this.ResultTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.ResultTextBox.Size = new System.Drawing.Size(284, 150);
			this.ResultTextBox.TabIndex = 2;
			// 
			// OpenModelDialog
			// 
			this.OpenModelDialog.FileName = "Выберите модель NXT блока";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 361);
			this.Controls.Add(this.ResultTextBox);
			this.Controls.Add(this.MethodCanny);
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
		private System.Windows.Forms.Button MethodCanny;
		private System.Windows.Forms.TextBox ResultTextBox;
		private System.Windows.Forms.OpenFileDialog OpenModelDialog;
	}
}

