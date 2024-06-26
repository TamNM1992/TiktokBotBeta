using Newtonsoft.Json;
using SmartBot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using File = System.IO.File;
using System.Net.NetworkInformation;
using ChromeAuto;
using RestSharp;
using Newtonsoft.Json.Linq;
using RestSharp.Authenticators;
using static System.Windows.Forms.Design.AxImporter;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using SmartBot.Service.Api.Users;
using SmartBot.Common.Configuration;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Collections;
using SmartBot.Common.Helpers;
using SmartBot.DataDto.Group;
using SmartBot.Common.Enums;
using System.Timers;
using System.Security.Policy;

namespace SmartBot
{
	public partial class fMain : Form
	{
		private bool _stop = true;
		private int timeLoad { get; set; }
		private FbAction FB { get; set; }
		private fCaiDatTuongTac_NewFeeds formNewfeeds { get; set; }
		private fCaiDatTuongTac_Nhom formNhom { get; set; }
		private fCaiDatTuongTac_Reels formReels { get; set; }
		private fCaiDatTuongTac_Stories formStories { get; set; }
		private fCaiDatTuongTac_BinhLuan formBinhLuan { get; set; }
		private GeneralConfig jConfig { get; set; }
		private string Grfile = null;
		private List<dynamic> link_List;
		private string linkPath = "";
		private string pathUD = Environment.CurrentDirectory + "/UserData";
		private string pathBrowser = Environment.CurrentDirectory + "/Browser";
		private string[] listFolder = { };
		private string accPath = null;
		private PauseTokenSource pts;
		private CancellationTokenSource cts;
		private List<dynamic> acc_List;
		List<string> ListActivateSession = new List<string>();
		List<SessionChrome> sessionChromes = new List<SessionChrome>();
		string[] arr_Anh = null;
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
		public fMain()
		{
			InitializeComponent();
			this.timeLoad = Convert.ToInt16(num_Delay.Value) * 1000;
			readToken();
			loadConfig();
			LoadTinh();
			Init();
			//radioButton1.Checked = true;
		}
		private async void readToken()
		{
			string configPath = Environment.CurrentDirectory + "/config.json";
			string strToken = "";
			if (File.Exists(configPath))
			{
				strToken = File.ReadAllText(configPath);
			}
			var jToken = JsonConvert.DeserializeObject<GeneralConfig>(strToken);

			if (jToken == null ||
				strToken == "" ||
				jToken.access_token == "")
			{
				Console.WriteLine("Không có token, chạy hàm login");
				fLogin login = new fLogin(); // thiếu parram => lỗi ngay
				login.ShowDialog();
				_stop = login._stop;
			}
			else
			{
				//var authenticator = new JwtAuthenticator(jToken.access_token);
				//var options = new RestClientOptions("http://127.0.0.1:5000");
				//var client = new RestClient(options);
				//var request = new RestRequest("/protected");
				////{
				////    Authenticator = authenticator
				////};
				//request.AddHeader("Authorization", $"Bearer {jToken.access_token}");
				//string hwID = HardwareID.Value();
				//request.AddParameter("hwID", hwID);
				//request.AddFile("file", path);
				//var response = client.Post(request);
				var response = _usersApiServices.CheckUserByToken(jToken.access_token);
				_stop = false;
				return;
				if (response.IsSuccessful)
				{
					//var content = response.Content; // Raw content as string
					//var jResponse = JsonConvert.DeserializeObject<dynamic>(content);
					var status = response.Data.Status;
					//var status = "success";

					string mess = response.Message;
					if (status == "success")
					{
						_stop = false;
					}
					else
					{
						_stop = true;
						if (mess.Contains("expired"))
						{
							/*
                             * Check xem token hết hạn không.
                             * Hết hạn thì xóa token trong file config
                             * Mở form login lên để người dùng đăng nhập lại
                             * Và tạo token mới
                             */
							jToken.access_token = "";
							using (StreamWriter file = File.CreateText("config.json"))
							{
								JsonSerializer serializer = new JsonSerializer();
								serializer.Serialize(file, jToken);
							}
							var resultBox = MessageBox.Show("Phiên đăng nhập đã hết hạn, hãy đăng nhập lại!", "Thông tin", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
							if (resultBox == DialogResult.OK)
							{
								fLogin login = new fLogin();
								login.ShowDialog();
								_stop = login._stop;
							}
							else
							{
								this.Close();
							}
						}
						//MessageBox.Show(jResponse.message);
						//Console.WriteLine(jResponse.message);
					}
				}
				else
				{
					_stop = true;
					MessageBox.Show("Lỗi kết nối đến server!", "Lỗi!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}
		public ContentConfig ContentConfig { get; set; }
		public List<string> _userLogins = new List<string>();
		private void loadConfig()
		{
			/*
             * tạo ra 1 hàm thực hiện tải vào cấu hình của ứng dụng từ file json
             * các cấu hình từ file bao gồm các biến trong chương trình
             */
			try
			{
				var configPath = "config.json";
				var strConfig = File.ReadAllText(configPath);
				jConfig = JsonConvert.DeserializeObject<GeneralConfig>(strConfig);
				if (jConfig.pathUD == null || jConfig.pathUD == "") { jConfig.pathUD = Environment.CurrentDirectory + "/UserData"; }
				if (jConfig.pathLog == null || jConfig.pathLog == "") { jConfig.pathLog = Environment.CurrentDirectory + "/log.txt"; }
				if (jConfig.delayBeforeAction == null || jConfig.delayBeforeAction == 0) { jConfig.delayBeforeAction = 2000; }
				if (jConfig.delayAfterAction == null || jConfig.delayAfterAction == 0) { jConfig.delayAfterAction = 2000; }
				var folders = Directory.EnumerateDirectories(jConfig.pathUD = Environment.CurrentDirectory + "/UserData").ToList();
				foreach (var f in folders)
				{
					if (f.Contains("Profile "))
					{
						var arr = f.Split("Profile ");
						_userLogins.Add(arr[1]);
						cb_Account.Items.Add(arr[1]);
					}
				}
				//var contentPost = File.ReadAllText("contentpost.txt");
				//var contentComment = File.ReadAllText("contentcomment.txt");
				//ContentConfig = new ContentConfig { ContentComment = contentComment, ContentPost = contentPost, ImgComment = jConfig.ImgComment, ImgPost = jConfig.ImgPost };
			}
			catch { }

		}
		private void LoadTinh()
		{
			/*
             * Load đơn vị hành chính các tỉnh thành phố
             */
			try
			{
				var donviHanhChinh = File.ReadAllText("Data/DonViHanhChinh.json");
				dynamic donvi = JsonConvert.DeserializeObject<dynamic>(donviHanhChinh);
				foreach (dynamic item in donvi)
				{
				}
			}
			catch { }
		}

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr hWndChildAfter, string className, string windowTitle);
		private static IntPtr FindHandle(IntPtr parentHandle, string className, string title)
		{
			IntPtr handle = IntPtr.Zero;

			for (var i = 0; i < 50; i++)
			{
				handle = FindWindowEx(parentHandle, IntPtr.Zero, className, title);

				if (handle == IntPtr.Zero)
				{
					Thread.Sleep(100);
				}
				else
				{
					break;
				}
			}
			return handle;
		}

		private static Random random = new Random();

		private async void btn_Create_Click(object sender, EventArgs e)
		{
			string userName = txt_Username.Text;
			string passWord = txt_Pass.Text;
			if ((userName == "") && (passWord == ""))
			{
				MessageBox.Show("Tài khoản và mật khẩu không được để trống", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else
			{
				string nameProfile;
				if (userName.Contains("@"))
				{
					nameProfile = Regex.Replace(userName.Split("@")[0], "[^0-9A-Za-z _-]", "");
				}
				else
				{
					nameProfile = Regex.Replace(userName, "[^0-9A-Za-z _-]", "");
				}
				FB = new FbAction("Profile " + nameProfile, txt_UA.Text, txt_Proxy.Text);
				await FB.LoginTiktok(userName, passWord);
			}
		}
		private async void file_Acc_Click(object sender, EventArgs e)
		{
			if (cb_Account.SelectedItem != null)
			{
				var nameProfile = cb_Account.SelectedItem.ToString();
				FB = new FbAction("Profile " + nameProfile, txt_UA.Text, txt_Proxy.Text);
				//var result = await FB.LoginTiktok("", "");
				//if(result)
				//	MessageBox.Show("Bạn đã đăng xuất tài khoản này. Hãy đăng nhập lại");
			}
			else
			{
				MessageBox.Show("Bạn cần chọn tài khoản để đăng nhập");
			}
		}
		[DllImport("User32.dll")]
		static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
		[Flags]
		public enum MouseEventFlags
		{
			LEFTDOWN = 0x00000002,
			LEFTUP = 0x00000004,
			MIDDLEDOWN = 0x00000020,
			MIDDLEUP = 0x00000040,
			MOVE = 0x00000001,
			ABSOLUTE = 0x00008000,
			RIGHTDOWN = 0x00000008,
			RIGHTUP = 0x00000010
		}
		public static void LeftClick(int x, int y)
		{
			Cursor.Position = new System.Drawing.Point(x, y);
			mouse_event((int)(MouseEventFlags.LEFTDOWN), 0, 0, 0, 0);
			mouse_event((int)(MouseEventFlags.LEFTUP), 0, 0, 0, 0);
		}
		private void btn_IMG_Click(object sender, EventArgs e)
		{
			if (open_Folder_IMG.ShowDialog() == DialogResult.OK)
			{
				try
				{
					var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".mp4" };
					var path_IMGs = open_Folder_IMG.SelectedPath;
					arr_Anh = Directory.GetFiles(path_IMGs)
										.Where(file => allowedExtensions
										.Any(file.ToLower().EndsWith))
										.ToArray();
				}
				catch (SecurityException ex)
				{
					MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
					$"Details:\n\n{ex.StackTrace}");
				}
			}
		}
		private void cb_UA_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				string listUA = File.ReadAllText("Brave\\ua.json");
				var json_Data = JsonConvert.DeserializeObject<dynamic>(listUA);
				var ua_Desktop = json_Data["Desktop"];
				var ua_Mobile = json_Data.Mobile;
				var rd = random.Next(ua_Desktop.Count);

				if (cb_UA.Checked)
				{
					txt_UA.Enabled = false;
					txt_UA.Text = ua_Desktop[rd]["ua"].ToString();
				}
				else { txt_UA.Enabled = true; }
			}
			catch
			{
				MessageBox.Show("Không tìm thấy File chưa User-Agent", "Lỗi!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		private void cb_Account_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cb_Account.Items.Count > 0)
			{
				var user = cb_Account.SelectedItem;
			}
		}
		private void KillBrave()
		{
			/*
             * Đóng trình duyệt
             */
			Process[] AllProcesses = Process.GetProcesses();
			foreach (var process in AllProcesses)
			{
				string s = process.ProcessName.ToLower();
				if (s == "brave")
				{
					process.Kill();
				}
				//process.CloseMainWindow();
			}
		}

		private void fMain_Load(object sender, EventArgs e)
		{
			label_status.Text = string.Empty;
			if (this._stop == true)
			{
				this.Close();

			}
		}
		bool checkPostGroup = false;

		private void InitFb()
		{
			List<Task> TaskList = new List<Task>();
			if (jConfig.pathUD == null || jConfig.pathUD == "") { jConfig.pathUD = Environment.CurrentDirectory + "/UserData"; }
			listFolder = Directory.GetDirectories(jConfig.pathUD, "Profile *");
			bool flag = true;
			if (listFolder.Length == 0)
			{
				MessageBox.Show("Chưa đăng nhập. Hãy đăng nhập trước khi chạy!", "Lỗi!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				flag = false;
			}

			if (flag)
			{
				for (int pro5 = 0; pro5 < listFolder.Length; pro5 += 2)
				{
					TaskList.Clear();
					for (int pro6 = 0; pro6 < 2; pro6++)
					{
						string[] Profile_ = listFolder[pro5 + pro6].Split('\\');
						int idxFolderProfile = Profile_.Length - 1;
						string Profile = Profile_[idxFolderProfile];

						if (FB == null)
							FB = new FbAction(Profile, txt_UA.Text, txt_Proxy.Text);
						Task.WaitAll(TaskList.ToArray());
						return;
					}
					Task.WaitAll(TaskList.ToArray());

				}
			}
		}
		private void button1_Click(object sender, EventArgs e)
		{
			var content = ContentConfig.ContentPost;
			var url = ContentConfig.ImgPost;
			foreach (var link in Constants.GroupList)
				FB.PostGroupAsyncNew(link.Link, content, url).Wait();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			var content = ContentConfig.ContentComment;
			var url = ContentConfig.ImgComment;
			foreach (var link in Constants.GroupList)
				FB.CommentMultiPostInGroup(content, link.Link, url).Wait();
		}

		static int minute = 10;
		static System.Timers.Timer aTimer = new System.Timers.Timer();
		static double _time = 60000 * minute;
		public void Init()
		{
			try
			{
				//FileHelper.GeneratorFileByDay(FileStype.Log, $"Khởi động hệ thống lúc {DateTime.Now.ToString("HH:mm:ss")}.", "Init");
				//aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
				//aTimer.Interval = _time;
				//aTimer.Enabled = true;
			}
			catch (Exception ex)
			{
				FileHelper.GeneratorFileByDay(FileStype.Error, ex.ToString(), "Init");
			}
		}
		private void OnTimedEvent(object source, ElapsedEventArgs e)
		{
			var now = DateTime.Now.TimeOfDay;

			if (now > new TimeSpan(6, 0, 0) && now <= new TimeSpan(6, minute, 0))
			{
				// post
				//var content = ContentConfig.ContentPost;
				//var url = ContentConfig.ImgPost;
				//foreach (var link in Constants.GroupList)
				//	FB.PostGroupAsyncNew(link.Link, content, url).Wait();
			}
			else if (now > new TimeSpan(8, 0, 0) && now <= new TimeSpan(8, minute, 0))
			{
				// comment
				var content = ContentConfig.ContentComment;
				var url = ContentConfig.ImgComment;
				foreach (var link in Constants.GroupList)
					FB.CommentMultiPostInGroup(content, link.Link, url).Wait();
			}
			else if (now > new TimeSpan(16, 0, 0) && now <= new TimeSpan(16, minute, 0))
			{
				// post
				var content = ContentConfig.ContentPost;
				var url = ContentConfig.ImgPost;
				foreach (var link in Constants.GroupList)
					FB.PostGroupAsyncNew(link.Link, content, url).Wait();
			}
			else if (now > new TimeSpan(18, 0, 0) && now <= new TimeSpan(18, minute, 0))
			{
				// comment
				var content = ContentConfig.ContentComment;
				var url = ContentConfig.ImgComment;
				foreach (var link in Constants.GroupList)
					FB.CommentMultiPostInGroup(content, link.Link, url).Wait();
			}
		}
		public string urlTiktok = "https://www.tiktok.com/";
		private async void radioButton1_CheckedChanged(object sender, EventArgs e)
		{
			if (radioButton1.Checked)
			{
				if (FB == null)
				{
					MessageBox.Show("Bạn cần đăng nhập", "Thông báo");
				}
				else
				{
					await FB.TiktokForyou();
				}
			}
		}

		private async void radioButton2_CheckedChanged(object sender, EventArgs e)
		{
			if (radioButton2.Checked)
			{
				if (FB == null)
				{
					MessageBox.Show("Bạn cần đăng nhập", "Thông báo");
				}
				else
				{
					await FB.TiktokFollowing();
				}
			}
		}

		private async void radioButton3_CheckedChanged(object sender, EventArgs e)
		{
			if (radioButton3.Checked)
			{
				if (FB == null)
				{
					MessageBox.Show("Bạn cần đăng nhập", "Thông báo");
				}
				else
				{
					await FB.TiktokFriends();
				}
			}
		}

		private async void radioButton4_CheckedChanged(object sender, EventArgs e)
		{
			if (radioButton4.Checked)
			{
				if (FB == null)
				{
					MessageBox.Show("Bạn cần đăng nhập", "Thông báo");
				}
				else
				{
					await FB.TiktokDefault();
				}
			}
		}

		private async void radioButton5_CheckedChanged(object sender, EventArgs e)
		{
			if (radioButton5.Checked)
			{
				if (FB == null)
				{
					MessageBox.Show("Bạn cần đăng nhập", "Thông báo");
				}
				else
				{
					await FB.TiktokExplore();
				}
			}
		}

		private async void button3_Click(object sender, EventArgs e)
		{
			await FB.TiktokNext();
		}

		private async void button4_Click(object sender, EventArgs e)
		{
			await FB.TiktokLike();
		}

		private async void button5_Click(object sender, EventArgs e)
		{
			await FB.LoginComment("hay quá");
		}
		Task task = null;
		CancellationTokenSource ctsTiktok = new();
		private void button_start_Click(object sender, EventArgs e)
		{
			int second = 0;
			bool check = int.TryParse(textBox_second.Text, out second);
			if (check || second > 0)
			{
				Task.Run(() => RunHanhDong(second, ctsTiktok));
			}
			else
			{
				MessageBox.Show("Thời gian không hợp lệ", "Thông báo");
			}
		}

		int countTim = 0;
		int countView = 0;
		int countFollow = 0;
		int countComment = 0;
		private async void RunHanhDong(int second, CancellationTokenSource token)
		{
			SetTextLabel(label_status, "Đang chạy...", Color.Blue);
			string bl = "Tuyệt vời";

			var text = (string)richTextBox_bl.Invoke(new Func<string>(() => richTextBox_bl.Text));
			if (!string.IsNullOrEmpty(text))
				bl = text;
			if (FB == null)
			{
				MessageBox.Show("Bạn cần đăng nhập", "Thông báo");
				return;
			}
			//await FB.TiktokForyou();
			try
			{
				int time = 15;
				int.TryParse(textBox_second.Text, out time);

				while (true)
				{
					bool check = false;
					if (checkBox_thatim.Checked)
					{
						check = await FB.TiktokLike();
						if (check)
						{
							countTim++;
							SetTextLabel(label_countTim, countTim.ToString(), null);
						}
					}
					if (checkBox_follow.Checked)
					{
						var ccheckFollow = FB.FollowClick();
						if (ccheckFollow)
						{
							countFollow++;
							SetTextLabel(label_countFollow, countFollow.ToString(), null);
						}
					}
					countView++;
					SetTextLabel(label_countView, countView.ToString(), null);
					if (checkBox_thatim.Checked && !check)
					{
						await FB.TiktokNext();
						continue;
					}
					if (checkBox_bl.Checked)
					{
						await FB.LoginComment(bl);
						countComment++;
						SetTextLabel(label_countComment, countComment.ToString(), null);
					}
					Thread.Sleep(time * 1000);
					await FB.TiktokNext();
					token.Token.ThrowIfCancellationRequested();
				}
			}
			catch (Exception ex)
			{
				SetTextLabel(label_status, "Lỗi rồi.", Color.Red);
				return;
			}
		}
		delegate void SetTextCallback(System.Windows.Forms.Label label, string text, Color? color);
		private void SetTextLabel(Label label, string text, Color? color)
		{
			if (label.InvokeRequired)
			{
				SetTextCallback d = new SetTextCallback(SetTextLabel);
				this.Invoke(d, new object[] { label, text, color });
			}
			else
			{
				label.Text = text;
				if (color != null)
					label.ForeColor = (Color)color;
			}
		}

		private void button_stop_Click(object sender, EventArgs e)
		{
			ctsTiktok.Cancel();
		}

		private async void button1_Click_1(object sender, EventArgs e)
		{
			await FB.TiktokBack();
		}

		private void label_countFollow_Click(object sender, EventArgs e)
		{

		}

		private void label_countTim_Click(object sender, EventArgs e)
		{

		}

		private async void button2_Click_1(object sender, EventArgs e)
		{
			await FB.PostViveo();
		}
	}
}