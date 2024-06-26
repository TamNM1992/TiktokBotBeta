namespace SmartBot
{
	partial class fListJoinedGroup
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			checkedListBox1 = new System.Windows.Forms.CheckedListBox();
			button1 = new System.Windows.Forms.Button();
			SuspendLayout();
			// 
			// checkedListBox1
			// 
			checkedListBox1.FormattingEnabled = true;
			checkedListBox1.Location = new System.Drawing.Point(12, 9);
			checkedListBox1.Name = "checkedListBox1";
			checkedListBox1.Size = new System.Drawing.Size(534, 490);
			checkedListBox1.TabIndex = 0;
			// 
			// button1
			// 
			button1.Location = new System.Drawing.Point(243, 518);
			button1.Name = "button1";
			button1.Size = new System.Drawing.Size(75, 23);
			button1.TabIndex = 1;
			button1.Text = "Đồng ý";
			button1.UseVisualStyleBackColor = true;
			button1.Click += button1_Click;
			// 
			// fListJoinedGroup
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(558, 553);
			Controls.Add(button1);
			Controls.Add(checkedListBox1);
			Name = "fListJoinedGroup";
			Text = "Chọn Group để đăng bài";
			Load += fListJoinedGroup_Load;
			ResumeLayout(false);
		}

		#endregion

		public System.Windows.Forms.CheckedListBox checkedListBox1;
		private System.Windows.Forms.Button button1;
	}
}