using SmartBot.Service.Api.Users;

namespace SmartBot
{
	partial class fLogin
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
			txtUsername = new System.Windows.Forms.TextBox();
			txtPassword = new System.Windows.Forms.TextBox();
			btnLogin = new System.Windows.Forms.Button();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			button1 = new System.Windows.Forms.Button();
			SuspendLayout();
			// 
			// txtUsername
			// 
			txtUsername.Location = new System.Drawing.Point(182, 63);
			txtUsername.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			txtUsername.Name = "txtUsername";
			txtUsername.Size = new System.Drawing.Size(217, 27);
			txtUsername.TabIndex = 0;
			// 
			// txtPassword
			// 
			txtPassword.Location = new System.Drawing.Point(182, 144);
			txtPassword.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			txtPassword.Name = "txtPassword";
			txtPassword.Size = new System.Drawing.Size(217, 27);
			txtPassword.TabIndex = 1;
			// 
			// btnLogin
			// 
			btnLogin.Location = new System.Drawing.Point(95, 248);
			btnLogin.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			btnLogin.Name = "btnLogin";
			btnLogin.Size = new System.Drawing.Size(99, 45);
			btnLogin.TabIndex = 2;
			btnLogin.Text = "Đăng nhập";
			btnLogin.UseVisualStyleBackColor = true;
			btnLogin.Click += btnLogin_Click;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(46, 67);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(107, 20);
			label1.TabIndex = 3;
			label1.Text = "Tên đăng nhập";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(46, 148);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(70, 20);
			label2.TabIndex = 4;
			label2.Text = "Mật khẩu";
			// 
			// button1
			// 
			button1.Location = new System.Drawing.Point(233, 248);
			button1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			button1.Name = "button1";
			button1.Size = new System.Drawing.Size(94, 45);
			button1.TabIndex = 5;
			button1.Text = "Hủy";
			button1.UseVisualStyleBackColor = true;
			button1.Click += button1_Click;
			// 
			// fLogin
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(447, 324);
			Controls.Add(button1);
			Controls.Add(label2);
			Controls.Add(label1);
			Controls.Add(btnLogin);
			Controls.Add(txtPassword);
			Controls.Add(txtUsername);
			Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			Name = "fLogin";
			Text = "Đăng nhập";
			Load += fLogin_Load;
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.TextBox txtUsername;
		private System.Windows.Forms.TextBox txtPassword;
		private System.Windows.Forms.Button btnLogin;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button button1;
	}
}