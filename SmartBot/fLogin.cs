using RestSharp.Authenticators;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using SmartBot.Service.Api.Users;
using SmartBot.Common;
using SmartBot.DataDto.User;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using Microsoft.Extensions.DependencyInjection;
using Quickwire.Attributes;

namespace SmartBot
{
	[RegisterService(ServiceLifetime.Singleton)]
	[RegisterService(ServiceLifetime.Scoped)]
	public partial class fLogin : Form
	{
		private IUsersApiServices usersApiServices;
		public IUsersApiServices _usersApiServices
		{
			set
			{
				this.usersApiServices = value;
			}
			get
			{

				return new UsersApiServices();
			}
		}



		//[InjectService]
		//public UsersApiServices _usersApiServices { get; set; }
		public fLogin()
		{
			InitializeComponent();
		}
		public bool _stop = true;
		protected string message { get; set; }

		private async void btnLogin_Click(object sender, EventArgs e)
		{
			string username = txtUsername.Text;
			string password = txtPassword.Text;
			//var options = new RestClientOptions("http://127.0.0.1:5000");
			//{
			//    Authenticator = new HttpBasicAuthenticator("username", "password")
			//};
			//var client = new RestClient(options);
			//var request = new RestRequest("/login");nh
			//string hwID = HardwareID.Value();
			//request.AddParameter("username", username);
			//request.AddParameter("password", password);
			//request.AddParameter("hwID", hwID);
			//request.AddFile("file", path);
			//var response = client.Post(request);
			//var t = _usersApiServices;
			var response = _usersApiServices.CheckUserByAccount(username, password);
			if (response.IsSuccessful)
			{
				//var content = response.Content; // Raw content as string
				//var jResponse = JsonConvert.DeserializeObject<dynamic>(content);
				var status = response.Data.Status;
				if (status == "success")
				{
					_stop = false;
					string token = response.Data.Token;
					string configPath = Environment.CurrentDirectory + "/config.json";
					string strToken = "";
					if (File.Exists(configPath))
					{
						strToken = File.ReadAllText(configPath);
					}
					var jToken = JsonConvert.DeserializeObject<GeneralConfig>(strToken);
					if (jToken == null || strToken == "")
					{
						jToken = new GeneralConfig();
					}
					jToken.access_token = token;
					using (StreamWriter file = File.CreateText("config.json"))
					{
						JsonSerializer serializer = new JsonSerializer();
						serializer.Serialize(file, jToken);
					}
					message = "Đăng nhập thành công";
					this.Close();
				}
				else
				{
					_stop = true;
					message = response.Message;
					MessageBox.Show(message, "Lỗi!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			else
			{
				_stop = true;
				MessageBox.Show("Lỗi kết nối đến server!", "Lỗi!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

		}

		private void button1_Click(object sender, EventArgs e)
		{
			this._stop = true;
			this.Close();
		}

		private void fLogin_Load(object sender, EventArgs e)
		{

		}
	}
}
