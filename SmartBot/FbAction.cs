using ChromeAuto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SmartBot
{
    public class FbAction
    {
        private string profileName { get; set; }
        private string pathUD { get; set; }
        private Chrome chrome { get; set; }
        private Process process { get; set; }
        public int timeLoad { get; set; }
        private Random random { get; set; }
        private string WindownTitle { get; set; }
        //List<string> ListActivateSession = new List<string>();
        public string ActivateSession = "";
        public SessionChrome gSessionChrome { get; set; }
        public List<string> JoinedGroups { get; set; } = new List<string>();
		//private ChromeAuto.Chrome Chrome { get; set; }
		//private int heightScreen System.Windows.SystemParameters.PrimaryScreenWidth;
		//public FbAction() {

		//}
		public FbAction()
        {
            chrome = new Chrome("http://localhost:9222");
        }
        public FbAction(string profileName, string UA, string Proxy, int timeLoad = 5, List<SessionChrome> sessionChromes = null)
        {
            /* Hàm khởi tạo */
            if (sessionChromes == null) sessionChromes = new List<SessionChrome>();
            pathUD = Environment.CurrentDirectory + "/UserData";
            this.profileName = profileName;
            this.timeLoad = timeLoad * 1000;
            this.process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.FileName = @"C:\Program Files\BraveSoftware\Brave-Browser\Application\brave.exe";
			//startInfo.FileName = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
			if (UA == "")
            {
                startInfo.Arguments = $"--remote-debugging-port=9222 --disable-popup-blocking --remote-allow-origins=*" +
                $" --proxy-server=\"{Proxy}\" --disable-features=Translate" +
                $" --user-data-dir=\"{pathUD}\" --profile-directory=\"{profileName}\" --start-maximized";
            }
            else
            {
                startInfo.Arguments = $"--remote-debugging-port=9222 --disable-popup-blocking --remote-allow-origins=*" +
            $" --proxy-server=\"{Proxy}\" --disable-features=Translate --user-agent=\"{UA}\"" +
            $" --user-data-dir=\"{pathUD}\" --profile-directory=\"{profileName}\" --start-maximized";
            }
            process.StartInfo = startInfo;
            process.Start();
            Delay(3);
            chrome = new Chrome("http://localhost:9222");
            var sessions = chrome.GetAvailableSessions();
            string sessionWSEndpoint = "";
            SessionChrome sessionChrome = new SessionChrome();

            foreach (var ss in sessions)
            {
                sessionChrome.Profile = profileName;
                sessionChrome.session = ss.webSocketDebuggerUrl;
                if ((!sessionChromes.Contains(sessionChrome)) && (ss.type == "page"))
                {
                    if (ss.url.Contains("://welcome"))
                    {
                        sessionWSEndpoint = sessionChrome.session;
                        sessionChromes.Add(sessionChrome);
                        break;
                    }
                    else if (ss.url == "chrome://newtab/")
                    {
                        sessionWSEndpoint = sessionChrome.session;
                        sessionChromes.Add(sessionChrome);
                        break;
                    }
                    else if (ss.url.Contains("http"))
                    {
                        sessionWSEndpoint = sessionChrome.session;
                        sessionChromes.Add(sessionChrome);
                        break;
                    }
                }
            }
            ActivateSession = sessionWSEndpoint;
            gSessionChrome = sessionChrome;
            chrome.SetActiveSession(sessionWSEndpoint);
        }
        public void SetAcivateSession(string sessionWSEndpoint)
        {
            chrome.SetActiveSession(sessionWSEndpoint);
        }
        private void loadConfig()
        {
            /*
             * Hàm load file json config 
             * Hàm này thực hiện load cấu hình từ 1 file config json
             */
            if (File.Exists(Environment.CurrentDirectory+"/settings.json"))
            {
                string json = File.ReadAllText(Environment.CurrentDirectory + "/settings.json");
                var config = JsonConvert.DeserializeObject<dynamic>(json);
                this.timeLoad = config["TimeLoad"];
                this.profileName = config["ProfileName"];
                this.pathUD = config["PathUD"];
            }
            else
            {
                MessageBox.Show("Không tìm thấy file config. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public async Task LoginFB(string username, string password)
        {
            /*Hàm đăng nhập fb"*/
            chrome.NavigateTo("http://facebook.com");
            await Task.Delay(timeLoad);
            chrome.Eval("document.getElementsByName('email')[0].value = \'" + username + "\'");
            await Task.Delay(500);
            chrome.Eval("document.getElementsByName('pass')[0].value = \'" + password + "\'");
            await Task.Delay(1500);
            chrome.Eval("document.getElementsByName('login')[0].click()");
            await Task.Delay(timeLoad);
            chrome.NavigateTo("chrome://newtab/");
            await Task.Delay(1500);
            MessageBox.Show("Đã thực hiện hành động đăng nhập xong.\nHãy cài đặt và kiểm tra lại, sau đó bấm OK!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
		public bool FollowClick()
		{
			var xy_cor_click = chrome.GetCenterEle("document.getElementsByClassName('css-1a05ri3-SpanIconWrapper e1hk3hf91')[1].getBoundingClientRect().toJSON()");
			Thread.Sleep(1000);
            if(xy_cor_click != null)
			    chrome.MouseClick(xy_cor_click[0], xy_cor_click[1]);
			var text_ele = chrome.Eval("document.getElementsByClassName('css-r4iroe-DivBtnWrapper evv7pft7')[0].textContent");
			string textContent = JsonConvert.DeserializeObject<dynamic>(text_ele)["result"]["result"]["value"];
            if(textContent != "Following")
            {
				var xy_cor_click2 = chrome.GetCenterEle("document.getElementsByClassName('css-r4iroe-DivBtnWrapper evv7pft7')[0].getBoundingClientRect().toJSON()");
				Thread.Sleep(1000);
				if (xy_cor_click2 != null)
                {
					chrome.MouseClick(xy_cor_click2[0], xy_cor_click2[1]);
                    return true;
				}

			}
			Thread.Sleep(1000);
            return false;
		}
		public async Task TiktokForyou()
        {
			chrome.NavigateTo("https://www.tiktok.com/foryou");
			await Task.Delay(timeLoad);
		}
		public async Task TiktokFollowing()
		{
			chrome.NavigateTo("https://www.tiktok.com/following");
			await Task.Delay(timeLoad);
		}
		public async Task TiktokFriends()
		{
			chrome.NavigateTo("https://www.tiktok.com/friends");
			await Task.Delay(timeLoad);
		}
		public async Task TiktokDefault()
		{
			chrome.NavigateTo("https://www.tiktok.com");
			await Task.Delay(timeLoad);
		}
		public async Task TiktokExplore()
		{
			chrome.NavigateTo("https://www.tiktok.com/explore");
			await Task.Delay(timeLoad);
		}
		public async Task TiktokNext()
		{
			chrome.PressDown();
			await Task.Delay(1000);
		}
		public async Task TiktokBack()
		{
			chrome.PressUp();
			await Task.Delay(1000);
		}
		public async Task<bool> PostViveo()
		{
			//await TiktokForyou();
			//var xy_cor_click = chrome.GetCenterEle("document.getElementsByClassName('css-1qup28j-DivUpload e18d3d944')[0].getBoundingClientRect().toJSON()");
			//Thread.Sleep(1000);
   //         if(xy_cor_click == null)
   //             return false;
			//chrome.MouseClick(xy_cor_click[0], xy_cor_click[1]);
			chrome.NavigateTo("https://www.tiktok.com/tiktokstudio/upload?from=upload");
            Thread.Sleep(5000);
            var classwall = "jsx-3309220042";
            var xy_cor_click = chrome.GetCenterEle($"document.getElementsByClassName('{classwall}')[0].getBoundingClientRect().toJSON()");
            Thread.Sleep(1000);
            if (xy_cor_click!=null)
            {
                chrome.MouseClick(xy_cor_click[0], xy_cor_click[1]);
            }
            Thread.Sleep(3000);
            string path = "D:\\Tiktok\\Video\\tiktok-next- back- tha tim.mp4";
			if (path != null)
			{
				string pathAnh = path.Trim();
				if (pathAnh.Length > 0)
				{
                    chrome.PressEnter();
					await Task.Delay(1000);

					StringBuilder pathIMG = new StringBuilder(pathAnh);

					await Task.Delay(3000);
					await Task.Run(() => SendIMG(pathIMG));
                    await Task.Delay(3000);
                }
            }
			await Task.Delay(5000);
            var post = "jsx-2745626608 button-group";
            var svgcmtBox = chrome.GetCenterEle($"document.getElementsByClassName('{post}')[0]" +
                            ".getBoundingClientRect().toJSON()");
            chrome.PressDown();
            chrome.PressDown();
            chrome.PressDown();

            await Task.Delay(2000);

            var button = "TUXButton TUXButton--default TUXButton--large TUXButton--primary";
			var xy_cor_click2 = chrome.GetCenterEle($"document.getElementsByClassName('{button}')[0].getBoundingClientRect().toJSON()");

            Thread.Sleep(1000);
			if (xy_cor_click2 == null)
				return false;
			chrome.MouseClick(xy_cor_click2[0], xy_cor_click2[1]);
            Thread.Sleep(3000);

            var uploadC = "TUXButton TUXButton--default TUXButton--medium TUXButton--primary";
            var uploadB = chrome.GetCenterEle($"document.getElementsByClassName('{uploadC}')[0].getBoundingClientRect().toJSON()");
            Thread.Sleep(1000);
            chrome.MouseClick(uploadB[0], uploadB[1]);
            return true;
		}
		public async Task<bool> TiktokLike()
		{
            bool result = false;
			var text_ele1 = chrome.Eval($"document.getElementsByClassName('css-xakz2y-ButtonActionItem e1hk3hf90')[0].getAttribute('aria-pressed')");
            if (text_ele1 != null)
            {
                var textContent = JsonConvert.DeserializeObject<dynamic>(text_ele1)["result"]["result"]["value"];
                if(textContent != null && textContent.ToString() == "false")
                {
					chrome.PressL();
					result = true;
				}
            }
			await Task.Delay(1000);
            return result;
		}
		public async Task LoginComment(string content)
        {
			var xy_cor_click2 = chrome.GetCenterEle("document.getElementsByClassName('notranslate public-DraftEditor-content')[0].getBoundingClientRect().toJSON()");
			Thread.Sleep(1000);
			chrome.MouseClick(xy_cor_click2[0], xy_cor_click2[1]);
			Thread.Sleep(1000);
            chrome.SendText(content);
			Thread.Sleep(1000);
            chrome.PressEnter();
			Thread.Sleep(1000);
			chrome.MouseClick(xy_cor_click2[0] * 2, xy_cor_click2[1]);
			Thread.Sleep(3000);
		}
		public async Task<bool> LoginTiktok(string username, string password)
		{
            bool result = true;
			/*Hàm đăng nhập tiktok"*/
			chrome.NavigateTo("https://www.tiktok.com/");
			await Task.Delay(timeLoad);
			var xy_cor_click = chrome.GetCenterEle("document.getElementsByClassName('css-q8q040-DivHeaderRightContainer e13wiwn60')[0].getBoundingClientRect().toJSON()");
			Thread.Sleep(1000);
			chrome.MouseClick(xy_cor_click[0], xy_cor_click[1]);
			if (xy_cor_click == null)
				return false;
            else
			Thread.Sleep(1000);
			var xy_cor_click1 = chrome.GetCenterEle("document.getElementsByClassName('css-1cp64nz-DivTextContainer e1cgu1qo3')[1].getBoundingClientRect().toJSON()");
            if(xy_cor_click1 == null)
                return false;
			Thread.Sleep(1000);
			chrome.MouseClick(xy_cor_click1[0], xy_cor_click1[1]);
			var xy_cor_click2 = chrome.GetCenterEle("document.getElementsByClassName('ep888o80 css-1mgli76-ALink-StyledLink epl6mg0')[1].getBoundingClientRect().toJSON()");
			Thread.Sleep(1000);
			chrome.MouseClick(xy_cor_click2[0], xy_cor_click2[1]);

			// user + pass
			var xy_cor_click_u = chrome.GetCenterEle("document.getElementsByClassName('ewblsjs1 css-bl7zoi-DivInputContainer-StyledBaseInput etcs7ny0')[0].getBoundingClientRect().toJSON()");
			Thread.Sleep(100);
			chrome.MouseClick(xy_cor_click_u[0], xy_cor_click_u[1]);
			chrome.SendText(username);
			Thread.Sleep(500);

			var xy_cor_click_p = chrome.GetCenterEle("document.getElementsByClassName('css-q83gm2-DivInputContainer etcs7ny0')[0].getBoundingClientRect().toJSON()");
			Thread.Sleep(100);
			chrome.MouseClick(xy_cor_click_p[0], xy_cor_click_p[1]);
			chrome.SendText(password);
			Thread.Sleep(500);

            // login
			chrome.Eval("document.getElementsByClassName('e1w6iovg0 css-11sviba-Button-StyledButton ehk74z00')[0].click()");
			await Task.Delay(timeLoad);
			chrome.NavigateTo("chrome://newtab/");
			await Task.Delay(1500);
			MessageBox.Show("Đã thực hiện hành động đăng nhập xong.\nHãy cài đặt và kiểm tra lại, sau đó bấm OK!", "Thông báo",
				MessageBoxButtons.OK, MessageBoxIcon.Information);
            return result;
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
        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int uMsg, IntPtr wParam, StringBuilder lParam);
        void Delay(int delay)
        {
            Thread.Sleep(TimeSpan.FromSeconds(delay));
        }
        private void SendIMG(StringBuilder pathIMG)
        {
            /* Hàm click gửi ảnh*/
            //StringBuilder pathIMG = new StringBuilder("");
            //for (int i = 0; i < clb_Link_Anh.Items.Count; i++)
            //{
            //    if (clb_Link_Anh.GetItemChecked(i))
            //    {
            //        pathIMG.Append("\"");
            //        pathIMG.Append(clb_Link_Anh.Items[i]);
            //        pathIMG.Append("\" ");
            //    }
            //}
            IntPtr fileChooserHandle = FindHandle(IntPtr.Zero, null, "Open");
            var comboboxExHandle = FindHandle(fileChooserHandle, "ComboBoxEx32", null);
            var comboboxHandle = FindHandle(comboboxExHandle, "ComboBox", null);
            var editHandle = FindHandle(comboboxHandle, "Edit", null);
            var btnHandle = FindWindowEx(fileChooserHandle, IntPtr.Zero, "Button", null);
            SendMessage(editHandle, 0x000C, IntPtr.Zero, pathIMG);
            Thread.Sleep(500);
            SendMessage(btnHandle, 513, IntPtr.Zero, null);
            // LeftButtonUp
            SendMessage(btnHandle, 514, IntPtr.Zero, null);
			Thread.Sleep(500);
        }
        public async Task PostWallAsync(PhanHoi BaiDang)
        {
            /* Đăng bài lên tường nhà*/
            chrome.NavigateTo("https://facebook.com");

            await Task.Delay(timeLoad);
            chrome.Eval("document.getElementsByClassName('xe4j0kc x78zum5 x1a02dak x1vqgdyp x1l1ennw x14vqqas x6ikm8r x10wlt62 x1y1aw1k xh8yej3')[0]" +
                ".getElementsByTagName('div')[3].click()");
            await Task.Delay(2000);
            chrome.SendText(BaiDang.Content);
            if (BaiDang.Image != null)
            {
                string pathAnh = BaiDang.Image.Trim();
                if (pathAnh.Length > 0)
                {
                    await Task.Delay(1000);
                    var xy_cor = chrome.GetCenterEle("document.getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z x78zum5 xdt5ytf x2lah0s x193iq5w " +
                        "xurb0ha x1sxyh0 x1gslohp x12nagc xzboxd6 x14l7nz5')[0].getBoundingClientRect().toJSON()");
                    await Task.Delay(1000);
                    StringBuilder pathIMG = new StringBuilder(pathAnh);
                    //string[] arrAnh = BaiDang.Image
                    //foreach (var ele in )
                    //{
                    //    pathIMG.Append("\"");
                    //    pathIMG.Append(ele.ToString());
                    //    pathIMG.Append("\"");
                    //}
                    chrome.MouseClick(xy_cor[0], xy_cor[1]);
                    await Task.Delay(3000);
                    await Task.Run(() => SendIMG(pathIMG));
                    await Task.Delay(3000);
                }
            }
            await Task.Delay(2000);
            var btn_Submit = chrome.GetCenterEle("document.getElementsByClassName('x6s0dn4 x9f619 x78zum5 x1qughib x1pi30zi x1swvt13 xyamay9 xh8yej3')[0]" +
            ".getElementsByTagName('div')[0].getBoundingClientRect().toJSON()");
            await Task.Delay(500);
            chrome.MouseClick(btn_Submit[0], btn_Submit[1]);

            await Task.Delay(timeLoad);
        }
        public async Task PostGroupAsync(PhanHoi BaiDang)
        {
            foreach(var link in JoinedGroups)
            {
				/*Đăng bài vào group*/
				chrome.NavigateTo(link);
				await Task.Delay(timeLoad);
				//for (int i = 0; i < 9; i++)
				//{
				//    SendKeys.SendWait("{TAB}");
				//    await Task.Delay(100);
				//}
				chrome.Eval("document.getElementsByClassName('x6s0dn4 x78zum5 x1l90r2v x1pi30zi x1swvt13 xz9dl7a')[0]" +
					".getElementsByTagName('div')[3].click()");
				await Task.Delay(1000);

				//chrome.Eval("document.getElementsByClassName('x9f619 x1lliihq x5yr21d xh8yej3')[0].getElementsByTagName('div')[0].click()");
				var xy_cor_click = chrome.GetCenterEle("document.getElementsByClassName('x9f619 x1lliihq x5yr21d xh8yej3')[0].getBoundingClientRect().toJSON()");
				await Task.Delay(1000);
				chrome.MouseClick(xy_cor_click[0], xy_cor_click[1]);
				await Task.Delay(1000);
				chrome.SendText(BaiDang.Content);

				if (BaiDang.Image != null)
				{
					string pathAnh = BaiDang.Image.Trim();
					pathAnh = pathAnh.Replace(@"/", @"\");

					if (pathAnh.Length > 0)
					{
						await Task.Delay(1000);
						var xy_cor = chrome.GetCenterEle("document.getElementsByClassName('xr9ek0c xfs2ol5 xjpr12u x12mruv9')[0].getBoundingClientRect().toJSON()");
						await Task.Delay(1000);
						chrome.MouseClick(xy_cor[0], xy_cor[1]);
						await Task.Delay(1300);
						StringBuilder pathIMG = new StringBuilder(pathAnh);
						await Task.Run(() => SendIMG(pathIMG));
						await Task.Delay(2000);
					}
				}
				await Task.Delay(2000);

				var ele_int = chrome.Eval("document.getElementsByClassName('x78zum5 x1q0g3np xqui1pq x1pl0jk3 x1plvlek xryxfnj x14ocpvf x5oemz9 x1lck2f0 xlgs127')[0]" +
					".getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z x78zum5 xdt5ytf x193iq5w xeuugli x1r8uery x1iyjqo2 xs83m0k xsyo7zv x16hj40l x10b6aqq x1yrsyyn')" +
					".length-1");
				var ele_json = JsonConvert.DeserializeObject<dynamic>(ele_int)["result"]["result"]["value"];

				chrome.Eval("document.getElementsByClassName('x78zum5 x1q0g3np xqui1pq x1pl0jk3 x1plvlek xryxfnj x14ocpvf x5oemz9 x1lck2f0 xlgs127')[0]" +
					$".getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z x78zum5 xdt5ytf x193iq5w xeuugli x1r8uery x1iyjqo2 xs83m0k xsyo7zv x16hj40l x10b6aqq x1yrsyyn')[{ele_json.ToString()}]" +
					".getElementsByTagName('div')[0].click()");
				//Delay(4);
				await Task.Delay(timeLoad);
			}
        }

		public async Task PostGroupAsyncNew(string link, string content, string img)
		{
		    /*Đăng bài vào group*/
		    chrome.NavigateTo(link);
			Thread.Sleep(timeLoad);
		    //for (int i = 0; i < 9; i++)
		    //{
		    //    SendKeys.SendWait("{TAB}");
		    //    await Task.Delay(100);
		    //}
		 //   chrome.Eval("document.getElementsByClassName('x1i10hfl x1ejq31n xd10rxx x1sy0etr x17r0tee x972fbf xcfux6l x1qhh985 xm0m39n x9f619 x1ypdohk xe8uvvx xdj266r x11i5rnm xat24cr x1mh8g0r x16tdsg8 x1hl2dhg xggy1nq x87ps6o x1lku1pv x1a2a7pz x6s0dn4 xmjcpbm x107yiy2 xv8uw2v x1tfwpuw x2g32xy x78zum5 x1q0g3np x1iyjqo2 x1nhvcw1 x1n2onr6 xt7dq6l x1ba4aug x1y1aw1k xn6708d xwib8y2 x1ye3gou')[0]" +
		 //   	".getElementsByTagName('div')[3].click()");
			//Thread.Sleep(1000);

		    //chrome.Eval("document.getElementsByClassName('x9f619 x1lliihq x5yr21d xh8yej3')[0].getElementsByTagName('div')[0].click()");
		    var xy_cor_click = chrome.GetCenterEle("document.getElementsByClassName('x6s0dn4 x78zum5 x1l90r2v x1pi30zi x1swvt13 xz9dl7a')[0].getBoundingClientRect().toJSON()");
			Thread.Sleep(1000);
		    chrome.MouseClick(xy_cor_click[0], xy_cor_click[1]);
			Thread.Sleep(1000);

            var arr = content.Split("\r\n").ToArray();
            for(int i = 0; i<arr.Length;i++)
            {
                chrome.SendText(arr[i]);
				if (i < arr.Length - 1)
                {
                    chrome.SendText(" ");
					chrome.PressEnter();
				}
			}

		    if (!string.IsNullOrEmpty(img))
		    {
		    	string pathAnh = img.Trim();
		    	pathAnh = pathAnh.Replace(@"/", @"\");

		    	if (pathAnh.Length > 0)
		    	{
					Thread.Sleep(1000);
		    		var xy_cor = chrome.GetCenterEle("document.getElementsByClassName('xr9ek0c xfs2ol5 xjpr12u x12mruv9')[0].getBoundingClientRect().toJSON()");
					Thread.Sleep(1000);
		    		chrome.MouseClick(xy_cor[0], xy_cor[1]);
					Thread.Sleep(1300);
		    		StringBuilder pathIMG = new StringBuilder(pathAnh);
		    		SendIMG(pathIMG);
					Thread.Sleep(2000);
		    	}
		    }
			Thread.Sleep(2000);

		    var ele_int = chrome.Eval("document.getElementsByClassName('x78zum5 x1q0g3np xqui1pq x1pl0jk3 x1plvlek xryxfnj x14ocpvf x5oemz9 x1lck2f0 xlgs127')[0]" +
		    	".getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z x78zum5 xdt5ytf x193iq5w xeuugli x1r8uery x1iyjqo2 xs83m0k xsyo7zv x16hj40l x10b6aqq x1yrsyyn')" +
		    	".length-1");
		    var ele_json = JsonConvert.DeserializeObject<dynamic>(ele_int)["result"]["result"]["value"];

		    chrome.Eval("document.getElementsByClassName('x78zum5 x1q0g3np xqui1pq x1pl0jk3 x1plvlek xryxfnj x14ocpvf x5oemz9 x1lck2f0 xlgs127')[0]" +
		    	$".getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z x78zum5 xdt5ytf x193iq5w xeuugli x1r8uery x1iyjqo2 xs83m0k xsyo7zv x16hj40l x10b6aqq x1yrsyyn')[{ele_json.ToString()}]" +
		    	".getElementsByTagName('div')[0].click()");
			//Delay(4);
			Thread.Sleep(10000);
		}
		public async Task PostWall_KichBan(HanhDong BaiDang)
        {
            /* Đăng bài lên tường nhà theo kich ban*/
            chrome.NavigateTo("https://facebook.com");

            await Task.Delay(timeLoad);
            chrome.Eval("document.getElementsByClassName('xe4j0kc x78zum5 x1a02dak x1vqgdyp x1l1ennw x14vqqas x6ikm8r x10wlt62 x1y1aw1k xh8yej3')[0]" +
                ".getElementsByTagName('div')[3].click()");
            await Task.Delay(2000);
            chrome.SendText(BaiDang.content);
            if (BaiDang.attach != null)
            {
                string pathAnh = BaiDang.attach.Trim();
                if (pathAnh.Length > 0)
                {
                    await Task.Delay(1000);
                    var xy_cor = chrome.GetCenterEle("document.getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z x78zum5 xdt5ytf x2lah0s x193iq5w " +
                        "xurb0ha x1sxyh0 x1gslohp x12nagc xzboxd6 x14l7nz5')[0].getBoundingClientRect().toJSON()");
                    await Task.Delay(1000);
                    StringBuilder pathIMG = new StringBuilder(pathAnh);
                    //string[] arrAnh = BaiDang.Image
                    //foreach (var ele in )
                    //{
                    //    pathIMG.Append("\"");
                    //    pathIMG.Append(ele.ToString());
                    //    pathIMG.Append("\"");
                    //}
                    chrome.MouseClick(xy_cor[0], xy_cor[1]);
                    await Task.Delay(3000);
                    await Task.Run(() => SendIMG(pathIMG));
                    await Task.Delay(3000);
                }
            }
            await Task.Delay(2000);
            var btn_Submit = chrome.GetCenterEle("document.getElementsByClassName('x6s0dn4 x9f619 x78zum5 x1qughib x1pi30zi x1swvt13 xyamay9 xh8yej3')[0]" +
            ".getElementsByTagName('div')[0].getBoundingClientRect().toJSON()");
            await Task.Delay(500);
            chrome.MouseClick(btn_Submit[0], btn_Submit[1]);

            await Task.Delay(timeLoad);
        }
        public async Task PostGroup_KichBan(HanhDong BaiDang)
        {
            /*
             * Đăng bài vào group theo kịch bản
             */
            chrome.NavigateTo(BaiDang.link);
            await Task.Delay(timeLoad);
            //for (int i = 0; i < 9; i++)
            //{
            //    SendKeys.SendWait("{TAB}");
            //    await Task.Delay(100);
            //}
            chrome.Eval("document.getElementsByClassName('x6s0dn4 x78zum5 x1l90r2v x1pi30zi x1swvt13 xz9dl7a')[0]" +
                ".getElementsByTagName('div')[3].click()");
            await Task.Delay(2000);
            chrome.SendText(BaiDang.content);
            if (BaiDang.attach != null)
            {
                string pathAnh = BaiDang.attach.Trim();
                if (pathAnh.Length > 0)
                {
                    await Task.Delay(1000);
                    var xy_cor = chrome.GetCenterEle("document.getElementsByClassName('xr9ek0c xfs2ol5 xjpr12u x12mruv9')[0].getBoundingClientRect().toJSON()");
                    await Task.Delay(1000);
                    chrome.MouseClick(xy_cor[0], xy_cor[1]);
                    await Task.Delay(1300);
                    StringBuilder pathIMG = new StringBuilder(pathAnh);
                    await Task.Run(() => SendIMG(pathIMG));
                    await Task.Delay(2000);
                }
            }
            await Task.Delay(2000);
            var ele_int = chrome.Eval("document.getElementsByClassName('x78zum5 x1q0g3np xqui1pq x1pl0jk3 x1plvlek xryxfnj x14ocpvf x5oemz9 x1lck2f0 xlgs127')[0]" +
                ".getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z x78zum5 xdt5ytf x193iq5w xeuugli x1r8uery x1iyjqo2 xs83m0k xsyo7zv x16hj40l x10b6aqq x1yrsyyn')" +
                ".length-1");
            var ele_json = JsonConvert.DeserializeObject<dynamic>(ele_int)["result"]["result"]["value"];
            chrome.Eval("document.getElementsByClassName('x78zum5 x1q0g3np xqui1pq x1pl0jk3 x1plvlek xryxfnj x14ocpvf x5oemz9 x1lck2f0 xlgs127')[0]" +
                $".getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z x78zum5 xdt5ytf x193iq5w xeuugli x1r8uery x1iyjqo2 xs83m0k xsyo7zv x16hj40l x10b6aqq x1yrsyyn')[{ele_json.ToString()}]" +
                ".getElementsByTagName('div')[0].click()");
            //Delay(4);
            await Task.Delay(timeLoad);
        }
        public async Task CommentToID_KichBan(HanhDong binhluan)
        {
            /*
             * Đăng bình luận vào bài viết vào ID theo kịch bản
             */
            chrome.NavigateTo(binhluan.link);
            await Task.Delay(timeLoad);
            var svgcmtBox = chrome.GetCenterEle("document.getElementsByClassName('x1iorvi4 x1pi30zi xjkvuk6 x1swvt13')[0]" +
                ".getBoundingClientRect().toJSON()");
            chrome.Eval($"scrollBy(0, {svgcmtBox[1] - svgcmtBox[3] * 3})");
            await Task.Delay(1000);
            svgcmtBox = chrome.GetCenterEle("document.getElementsByClassName('x1iorvi4 x1pi30zi xjkvuk6 x1swvt13')[0]" +
                ".getBoundingClientRect().toJSON()");
            await Task.Delay(500);
            chrome.MouseClick(svgcmtBox[0], svgcmtBox[1]);
            await Task.Delay(1000);
            chrome.SendText(binhluan.content);
            await Task.Delay(1000);
            if (binhluan.attach != null)
            {
                string pathAnh = binhluan.attach.Trim();
                if (pathAnh.Length > 0)
                {
                    StringBuilder pathIMG = new StringBuilder(pathAnh);
                    svgcmtBox = chrome.GetCenterEle("document.getElementsByClassName('x1iorvi4 x1pi30zi xjkvuk6 x1swvt13')[0]" +
                        ".getElementsByTagName('ul')[1].getElementsByTagName('li')[2].getBoundingClientRect().toJSON()");
                    await Task.Delay(500);
                    if (svgcmtBox == null)
                    {
                        svgcmtBox = chrome.GetCenterEle("document.getElementsByClassName('x1iorvi4 x1pi30zi xjkvuk6 x1swvt13')[0]" +
                                                ".getElementsByTagName('ul')[0].getElementsByTagName('li')[2].getBoundingClientRect().toJSON()");
                        await Task.Delay(500);
                    }
                    chrome.MouseClick(svgcmtBox[0], svgcmtBox[1]);
                    await Task.Delay(1900);
                    await Task.Run(() => SendIMG(pathIMG));
                    await Task.Delay(2000);
                }
            }
            svgcmtBox = chrome.GetCenterEle("document.getElementsByClassName('x1iorvi4 x1pi30zi xjkvuk6 x1swvt13')[0]" +
                ".getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z x78zum5 x2lah0s x1qughib x6s0dn4 xozqiw3 x1q0g3np xcud41i x139jcc6 x4cne27 xifccgj')[0]" +
                ".getBoundingClientRect().toJSON()");
            await Task.Delay(500);
            chrome.MouseClick(svgcmtBox[0], svgcmtBox[1]);
            await Task.Delay(timeLoad);
        }
        public async Task CommentAsync(PhanHoi BinhLuan)
        {
            chrome.NavigateTo(BinhLuan.Link);
            await Task.Delay(timeLoad);
        }
        public async Task<List<string>> GetID(string linkgr = "")
        {
            /* Lấy ID bài viết */
            int oldLen = 0;
            await Task.Delay(2000);
            if (linkgr != "")
            {
                chrome.NavigateTo(linkgr);
                await Task.Delay(timeLoad);
            }
            var arLink = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                chrome.Eval("scrollBy(0, 600);");
                await Task.Delay(1000);
                var lenBaiViet = chrome.Eval("document.getElementsByClassName('x1yztbdb x1n2onr6 xh8yej3 x1ja2u2z').length");
                int lenBaiViet_int = JsonConvert.DeserializeObject<dynamic>(lenBaiViet)["result"]["result"]["value"];
                for (int j = oldLen; j < lenBaiViet_int; j++)
                {
                    var x_co = chrome.Eval($"document.getElementsByClassName('x1yztbdb x1n2onr6 xh8yej3 x1ja2u2z')[{j}]" +
                        $".getElementsByClassName('xu06os2 x1ok221b')[1]" +
                        $".getElementsByClassName('x4k7w5x x1h91t0o x1h9r5lt x1jfb8zj xv2umb2 x1beo9mf xaigb6o x12ejxvf x3igimt xarpa2k xedcshv x1lytzrv x1t2pt76 x7ja8zs x1qrby5j')[0]" +
                        $".getElementsByTagName('a')[0].getBoundingClientRect().x + 10;");
                    var y_co = chrome.Eval($"document.getElementsByClassName('x1yztbdb x1n2onr6 xh8yej3 x1ja2u2z')[{j}]" +
                        $".getElementsByClassName('xu06os2 x1ok221b')[1]" +
                        $".getElementsByClassName('x4k7w5x x1h91t0o x1h9r5lt x1jfb8zj xv2umb2 x1beo9mf xaigb6o x12ejxvf x3igimt xarpa2k xedcshv x1lytzrv x1t2pt76 x7ja8zs x1qrby5j')[0]" +
                        $".getElementsByTagName('a')[0].getBoundingClientRect().y + 10;");
                    try
                    {
                        float x_j = JsonConvert.DeserializeObject<dynamic>(x_co)["result"]["result"]["value"];
                        float y_j = JsonConvert.DeserializeObject<dynamic>(y_co)["result"]["result"]["value"];
                        //Delay(1);
                        await Task.Delay(100);
                        if (y_j < 127)
                        {
                            continue;
                        }
                        else if (y_j >= 127 && y_j < 950)
                        {
                            chrome.MouseMove(x_j, y_j);
                            await Task.Delay(1000);
                            var obLink = chrome.Eval($"document.getElementsByClassName('x1yztbdb x1n2onr6 xh8yej3 x1ja2u2z')[{j}]" +
                                $".getElementsByClassName('xu06os2 x1ok221b')[1]" +
                                $".getElementsByClassName('x4k7w5x x1h91t0o x1h9r5lt x1jfb8zj xv2umb2 x1beo9mf xaigb6o x12ejxvf x3igimt" +
                                $" xarpa2k xedcshv x1lytzrv x1t2pt76 x7ja8zs x1qrby5j')[0].getElementsByTagName('a')[0].getAttribute('href')");
                            string linkO = JsonConvert.DeserializeObject<dynamic>(obLink)["result"]["result"]["value"];
                            int vtHoiCham = linkO.IndexOf("?");
                            if (vtHoiCham == -1)
                            {
                                continue;
                            }
                            string linkOriginal = linkO.Substring(0, vtHoiCham);
                            if (!arLink.Contains(linkOriginal) && linkOriginal.Contains("posts")) { arLink.Add(linkOriginal); }
                        }
                        else
                        {
                            oldLen = j;
                            break;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            return arLink;
        }
		public async Task<Dictionary<string, string>> ListGroup(string link = "")
		{
            if (string.IsNullOrEmpty(link))
            {
				link = "https://www.facebook.com/groups/joins/?nav_source=tab&ordering=viewer_added";
            }
			chrome.NavigateTo(link);
			Thread.Sleep(5000);
			var btnAddFr = chrome.GetCenterEle("document.getElementsByClassName('x6s0dn4 x78zum5 xdt5ytf x193iq5w x1t2pt76 xh8yej3')[0].getBoundingClientRect().toJSON()");
            if (btnAddFr != null)
            {
                chrome.Eval($"scrollBy(0, {btnAddFr[3] * 2})");
            }
			Thread.Sleep(15000);
			Dictionary<string, string> dicGroupLink = new Dictionary<string, string>();
			List<string> lst = new List<string>();
			for (int i = 0; i <= 1000; i++)
			{
				string groupName = "";
				string groupLink = "";
				var text_ele1 = chrome.Eval($"document.getElementsByClassName('x9f619 x78zum5 x1r8uery xdt5ytf x1iyjqo2 xs83m0k x150jy0e x1e558r4 xjkvuk6 x1iorvi4 xnpuxes')[{i}].textContent");
				if (text_ele1 != null)
				{
					var textContent1 = JsonConvert.DeserializeObject<dynamic>(text_ele1)["result"]["result"]["value"];
					if (textContent1 == null)
						continue;
                    if (!textContent1.ToString().Contains("Lần truy cập"))
                        continue;
					groupName = textContent1.ToString().Split("Lần truy cập")[0];
				}
				else
				{
					continue;
				}

				var text_ele2 = chrome.Eval($"document.getElementsByClassName('x9f619 x78zum5 x1r8uery xdt5ytf x1iyjqo2 xs83m0k x150jy0e x1e558r4 xjkvuk6 x1iorvi4 xnpuxes')[{i}]" +
					$".getElementsByClassName('x1jx94hy x1lq5wgf xgqcy7u x30kzoy x9jhf4c x1olyfxc x9f619 x78zum5 xdt5ytf x1qughib xyamay9 x1pi30zi x1l90r2v x1swvt13')[0]" +
					$".getElementsByClassName('x6s0dn4 x78zum5 x1vqgdyp x1qughib xw7yly9 xvfhwrs')[0]" +
					$".getElementsByClassName('x1i10hfl xjbqb8w x1ejq31n xd10rxx x1sy0etr x17r0tee x972fbf xcfux6l x1qhh985 xm0m39n x1ypdohk xe8uvvx xdj266r x11i5rnm xat24cr x1mh8g0r xexx8yu x4uap5 x18d9i69 xkhd6sd x16tdsg8 x1hl2dhg xggy1nq x1o1ewxj x3x9cwd x1e5q0jg x13rtm0m x87ps6o x1lku1pv x1a2a7pz x9f619 x3nfvp2 xdt5ytf xl56j7k x1n2onr6 xh8yej3')[0].href");
				if (text_ele2 != null)
				{
					groupLink = JsonConvert.DeserializeObject<dynamic>(text_ele2)["result"]["result"]["value"];
				}
				if (!string.IsNullOrEmpty(groupName) && !string.IsNullOrEmpty(groupLink) && !dicGroupLink.ContainsKey(groupName))
				{
					dicGroupLink.Add(groupName, groupLink);
				}

				//var text_ele3 = chrome.Eval($"document.getElementsByClassName('x9f619 x78zum5 x1r8uery xdt5ytf x1iyjqo2 xs83m0k x150jy0e x1e558r4 xjkvuk6 x1iorvi4 xnpuxes')[{i}]" +
				//	$".getElementsByClassName('x1jx94hy x1lq5wgf xgqcy7u x30kzoy x9jhf4c x1olyfxc x9f619 x78zum5 xdt5ytf x1qughib xyamay9 x1pi30zi x1l90r2v x1swvt13')[0]" +
				//	$".getElementsByClassName('x6s0dn4 x78zum5 x1vqgdyp x1qughib xw7yly9 xvfhwrs')[0]" +
				//	$".getElementsByClassName('x1i10hfl xjbqb8w x1ejq31n xd10rxx x1sy0etr x17r0tee x972fbf xcfux6l x1qhh985 xm0m39n x1ypdohk xe8uvvx xdj266r x11i5rnm xat24cr x1mh8g0r xexx8yu x4uap5 x18d9i69 xkhd6sd x16tdsg8 x1hl2dhg xggy1nq x1o1ewxj x3x9cwd x1e5q0jg x13rtm0m x87ps6o x1lku1pv x1a2a7pz x9f619 x3nfvp2 xdt5ytf xl56j7k x1n2onr6 xh8yej3')[0]"
				//  + $".getElementsByClassName('x1rg5ohu x1n2onr6 x3ajldb x1ja2u2z')[0].src"
				//	//+ $".getElementsByClassName('x1rg5ohu x1n2onr6 x3ajldb x1ja2u2z')[0]"
				//	//+ $".getElementsByTagName('image')[x]" 
				//	);

				//if (text_ele3 != null)
				//{
				//	string groupImage = JsonConvert.DeserializeObject<dynamic>(text_ele3)["result"]["result"]["value"];
				//	if (!string.IsNullOrEmpty(groupImage))
				//		lst.Add(groupImage);

				//}
			}

            return dicGroupLink;
		}
		public async Task<string> GetPostFrID(string link = "")
        {
            /* Lấy nội dung từ ID bài viết */
            if (link != "")
            {
                chrome.NavigateTo(link);

                await Task.Delay(timeLoad);
            }
            link = chrome.url();
            var text_ele = chrome.Eval("document.getElementsByClassName('x9f619 x78zum5 x1r8uery xdt5ytf x1iyjqo2 xs83m0k x150jy0e x1e558r4 xjkvuk6 x1iorvi4 xnpuxes')[0].textContent");
            Dictionary<string, string> dicGroupLink = new Dictionary<string, string>();
            List<string> lst = new List<string>();
            for(int i = 0; i<= 100;i++)
            {
                string groupName = "";
                string groupLink = "";
				var text_ele1 = chrome.Eval($"document.getElementsByClassName('x9f619 x78zum5 x1r8uery xdt5ytf x1iyjqo2 xs83m0k x150jy0e x1e558r4 xjkvuk6 x1iorvi4 xnpuxes')[{i}].textContent");
                if (text_ele1 != null)
                {
					var textContent1 = JsonConvert.DeserializeObject<dynamic>(text_ele1)["result"]["result"]["value"];
                    if (textContent1 == null)
                        break;
                    groupName = textContent1.ToString().Split("Lần truy cập")[0];
				}
                else
                {
                    break;
                }

				var text_ele2 = chrome.Eval($"document.getElementsByClassName('x9f619 x78zum5 x1r8uery xdt5ytf x1iyjqo2 xs83m0k x150jy0e x1e558r4 xjkvuk6 x1iorvi4 xnpuxes')[{i}]" +
                    $".getElementsByClassName('x1jx94hy x1lq5wgf xgqcy7u x30kzoy x9jhf4c x1olyfxc x9f619 x78zum5 xdt5ytf x1qughib xyamay9 x1pi30zi x1l90r2v x1swvt13')[0]" +
				    $".getElementsByClassName('x6s0dn4 x78zum5 x1vqgdyp x1qughib xw7yly9 xvfhwrs')[0]" +
				    $".getElementsByClassName('x1i10hfl xjbqb8w x1ejq31n xd10rxx x1sy0etr x17r0tee x972fbf xcfux6l x1qhh985 xm0m39n x1ypdohk xe8uvvx xdj266r x11i5rnm xat24cr x1mh8g0r xexx8yu x4uap5 x18d9i69 xkhd6sd x16tdsg8 x1hl2dhg xggy1nq x1o1ewxj x3x9cwd x1e5q0jg x13rtm0m x87ps6o x1lku1pv x1a2a7pz x9f619 x3nfvp2 xdt5ytf xl56j7k x1n2onr6 xh8yej3')[0].href");
			    if (text_ele2 != null)
                {
					groupLink = JsonConvert.DeserializeObject<dynamic>(text_ele2)["result"]["result"]["value"];
				}
                if(!string.IsNullOrEmpty(groupName) && !string.IsNullOrEmpty(groupLink) && !dicGroupLink.ContainsKey(groupName))
                {
                    dicGroupLink.Add(groupName, groupLink);
				}

				var text_ele3 = chrome.Eval($"document.getElementsByClassName('x9f619 x78zum5 x1r8uery xdt5ytf x1iyjqo2 xs83m0k x150jy0e x1e558r4 xjkvuk6 x1iorvi4 xnpuxes')[{i}]" +
					$".getElementsByClassName('x1jx94hy x1lq5wgf xgqcy7u x30kzoy x9jhf4c x1olyfxc x9f619 x78zum5 xdt5ytf x1qughib xyamay9 x1pi30zi x1l90r2v x1swvt13')[0]" +
					$".getElementsByClassName('x6s0dn4 x78zum5 x1vqgdyp x1qughib xw7yly9 xvfhwrs')[0]" +
					$".getElementsByClassName('x1i10hfl xjbqb8w x1ejq31n xd10rxx x1sy0etr x17r0tee x972fbf xcfux6l x1qhh985 xm0m39n x1ypdohk xe8uvvx xdj266r x11i5rnm xat24cr x1mh8g0r xexx8yu x4uap5 x18d9i69 xkhd6sd x16tdsg8 x1hl2dhg xggy1nq x1o1ewxj x3x9cwd x1e5q0jg x13rtm0m x87ps6o x1lku1pv x1a2a7pz x9f619 x3nfvp2 xdt5ytf xl56j7k x1n2onr6 xh8yej3')[0]"
				  + $".getElementsByClassName('x1rg5ohu x1n2onr6 x3ajldb x1ja2u2z')[0].src"
					//+ $".getElementsByClassName('x1rg5ohu x1n2onr6 x3ajldb x1ja2u2z')[0]"
					//+ $".getElementsByTagName('image')[x]" 
					);

				if (text_ele3 != null)
                {
                    string groupImage = JsonConvert.DeserializeObject<dynamic>(text_ele3)["result"]["result"]["value"];
                    if(!string.IsNullOrEmpty(groupImage))
						lst.Add(groupImage);

				}
            }
            string textContent = JsonConvert.DeserializeObject<dynamic>(text_ele)["result"]["result"]["value"];
            if (textContent == null)
            {
                text_ele = chrome.Eval("document.getElementsByClassName('x1iorvi4 x1pi30zi x1swvt13 xjkvuk6')[0]" +
                    ".getElementsByClassName('x78zum5 xdt5ytf xz62fqu x16ldp7u')[0].textContent");
                textContent = JsonConvert.DeserializeObject<dynamic>(text_ele)["result"]["result"]["value"];
                if (textContent == null)
                {
                    text_ele = chrome.Eval("document.getElementsByClassName('x1cy8zhl x78zum5 x1nhvcw1 x1n2onr6 xh8yej3')[0].textContent");
                    textContent = JsonConvert.DeserializeObject<dynamic>(text_ele)["result"]["result"]["value"];
                }
            }
            await Task.Delay(500);
            var myData = new
            {
                Link = link,
                Noi_Dung = textContent,
            };
            await Task.Delay(500);
            using (StreamWriter file = File.AppendText("ListPosts.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, myData);
            }
            return textContent;
        }
        public async Task<List<string>> GetLog(string Link = "")
        {
            /* Lấy thông tin số lượng share, bình luận, cảm xúc */
            if (Link != "")
            {
                chrome.NavigateTo(Link);
            }
            var listCamXuc = new List<string>();
            await Task.Delay(timeLoad);
            var soLuongCMT_obj = chrome.Eval("document.getElementsByClassName('x168nmei x13lgxp2 x30kzoy x9jhf4c x6ikm8r x10wlt62')[0]" +
                ".getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z x78zum5 x2lah0s x1qughib x1qjc9v5 xozqiw3 x1q0g3np xykv574 xbmpl8g x4cne27 xifccgj')[0]" +
                ".getElementsByClassName('x4k7w5x x1h91t0o x1h9r5lt x1jfb8zj xv2umb2 x1beo9mf xaigb6o x12ejxvf x3igimt xarpa2k xedcshv x1lytzrv x1t2pt76 x7ja8zs x1qrby5j')[0].textContent");
            //var soLuongShare_obj = chrome.Eval("document.getElementsByClassName('x168nmei x13lgxp2 x30kzoy x9jhf4c x6ikm8r x10wlt62')[0]" +
            //    ".getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z x78zum5 x2lah0s x1qughib x1qjc9v5 xozqiw3 x1q0g3np xykv574 xbmpl8g x4cne27 xifccgj')[0]" +
            //    ".getElementsByClassName('x4k7w5x x1h91t0o x1h9r5lt x1jfb8zj xv2umb2 x1beo9mf xaigb6o x12ejxvf x3igimt xarpa2k xedcshv x1lytzrv x1t2pt76 x7ja8zs x1qrby5j')[0]");
            string soLuongCmt = JsonConvert.DeserializeObject<dynamic>(soLuongCMT_obj)["result"]["result"]["value"];
            var boxCamXuc = chrome.GetCenterEle("document.getElementsByClassName('x168nmei x13lgxp2 x30kzoy x9jhf4c x6ikm8r x10wlt62')[0]" +
                ".getElementsByClassName('xrbpyxo x6ikm8r x10wlt62 xlyipyv x1exxlbk')[0].getBoundingClientRect().toJSON()");
            if (boxCamXuc != null)
            {
                chrome.Eval($"scrollBy(0, {boxCamXuc[1] - boxCamXuc[3] * 5})");
                await Task.Delay(2000);
                boxCamXuc = chrome.GetCenterEle("document.getElementsByClassName('x168nmei x13lgxp2 x30kzoy x9jhf4c x6ikm8r x10wlt62')[0]" +
                ".getElementsByClassName('xrbpyxo x6ikm8r x10wlt62 xlyipyv x1exxlbk')[0].getBoundingClientRect().toJSON()");
                chrome.MouseClick(boxCamXuc[0], boxCamXuc[1]);
                await Task.Delay(2000);
                for (int i = 1; i < 8; i++)
                {
                    var valuezz = chrome.getValueEle("document.getElementsByClassName('x6s0dn4 x78zum5 x2lah0s x1qughib x879a55 x1n2onr6')[0]" +
                        $".getElementsByClassName('x1ey2m1c x9f619 xds687c x10l6tqk x17qophe x13vifvy')[0].children[{i}].getAttribute('aria-label')");
                    if (valuezz != null || valuezz != "null")
                    {
                        listCamXuc.Add(valuezz);
                    }
                }
            }
            listCamXuc.Add(soLuongCmt);
            return listCamXuc;
        }
        public async Task searchAndAction(string Profile, string typeFilter, string keyWord, string location, int Ngroup)
        {
            /* 
             * Tìm kiếm nhóm, trang và thực hiện theo dõi, tham gia tất cả các nhóm, trang tìm được.
             * Lưu dữ liệu vào trong CSDL
             */
            int heightMonitor = SystemInformation.PrimaryMonitorSize.Height;
            int oldLen = 0;
            string keyWordLocation = $"{keyWord} {location}";
            int clickFanpage = 0;
            chrome.NavigateTo($"https://www.facebook.com/search/{typeFilter}/?q={keyWordLocation}");
            await Task.Delay(timeLoad);
            int winHeight = Convert.ToInt16(chrome.getValueEle("window.innerHeight")) - 20;
            if (!Directory.Exists(Environment.CurrentDirectory + $"/Data/Data_Collect/"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + $"/Data/Data_Collect/");
            }
            string linkPath = Environment.CurrentDirectory + $"/Data/Data_Collect/Data_Collected_By_{Profile}.json";
            string link_string = "";
            if (File.Exists(linkPath))
            {
                link_string = await File.ReadAllTextAsync(linkPath);
            }
            List<dynamic> link_List = JsonConvert.DeserializeObject<List<dynamic>>(link_string);
            int countLinkList = link_List != null ? link_List.Count : 0;
            if (countLinkList > 0)
            {
                link_List.Clear();
            }
            //if ((typeFilter.Equals("pages") && !location.Equals("")) || (typeFilter.Equals("groups") && !location.Equals("")))
            //{
            //    var filter_ = chrome.GetCenterEle($"document.getElementsByClassName('xjpr12u x1emribx x14pwi92 x1k70j0n')[0]" +
            //        $".getElementsByClassName('x9f619 x78zum5 xdt5ytf xh8yej3')[0].getBoundingClientRect().toJSON()");
            //    chrome.MouseClick(filter_[0], filter_[1]);
            //    await Task.Delay(500);
            //    chrome.SendText(location);
            //    await Task.Delay(1000);
            //    filter_ = chrome.GetCenterEle("document.getElementsByClassName('x78zum5 x1q0g3np x1nhvcw1 xjwf9q1 x6ikm8r x10wlt62 x1nn3v0j x4uap5 x1120s5i xkhd6sd')[0]" +
            //        ".getBoundingClientRect().toJSON()");
            //    await Task.Delay(400);
            //    chrome.MouseClick(filter_[0], filter_[1]);
            //}
            //else if ((typeFilter.Equals("people") && !location.Equals("")) || (typeFilter.Equals("posts") && !location.Equals("")))
            //{
            //    var filter_ = chrome.GetCenterEle($"document.getElementsByClassName('xjpr12u x1emribx x14pwi92 x1k70j0n')[0]" +
            //        $".getElementsByClassName('x9f619 x78zum5 xdt5ytf xh8yej3')[1].getBoundingClientRect().toJSON()");
            //    chrome.MouseClick(filter_[0], filter_[1]);
            //    await Task.Delay(500);
            //    chrome.SendText(location);
            //    await Task.Delay(1000);
            //    filter_ = chrome.GetCenterEle("document.getElementsByClassName('x78zum5 x1q0g3np x1nhvcw1 xjwf9q1 x6ikm8r x10wlt62 x1nn3v0j x4uap5 x1120s5i xkhd6sd')[0]" +
            //        ".getBoundingClientRect().toJSON()");
            //    await Task.Delay(500);
            //    chrome.MouseClick(filter_[0], filter_[1]);
            //}
            if (typeFilter.Equals("posts"))
            {
                var listID = await GetID();
                string mes = "";
                foreach (var it in listID)
                {
                    mes += it + "\n";
                }
                //MessageBox.Show(mes);
            }
            else
            {
                int countLen = 0;
                //int nscroll = Ngroup / 6;
                //nscroll = nscroll < 1 ? 1 : nscroll;
                //int lenGr = 0;
                Random random = new Random();
                do
                {
                    await Task.Delay(timeLoad);
                    int rdScroll = random.Next(578, 783);
                    int rdDelay = random.Next(5500, 30600);
                    var lenGroup_obj = chrome.Eval("document.getElementsByClassName('x1yztbdb').length");
                    await Task.Delay(500);
                    int lenGroup = JsonConvert.DeserializeObject<dynamic>(lenGroup_obj)["result"]["result"]["value"];
                    await Task.Delay(500);
                    //lenGr = lenGroup;
                    if (clickFanpage >= Ngroup)
                    {
                        break;
                    }
                    if (clickFanpage >= lenGroup)
                    {
                        countLen++;
                        if (countLen >= 10)
                        {
                            break;
                        }
                        chrome.Eval($"scrollBy(0, {rdScroll});");
                        continue;

                    }
                    for (int j = oldLen; j < lenGroup; j++)
                    {
                        await Task.Delay(1000);
                        string lenJoin = chrome.getValueEle($"document.getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z x78zum5 xdt5ytf x193iq5w xeuugli x1r8uery x1iyjqo2 xs83m0k x150jy0e x1e558r4 xjkvuk6 x1iorvi4')[{j}]" +
                            $".getElementsByTagName('a').length");
                        //var lenA_obj = chrome.Eval($"document.getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z x78zum5 xdt5ytf x193iq5w xeuugli x1r8uery x1iyjqo2 xs83m0k x150jy0e x1e558r4 xjkvuk6 x1iorvi4')[{j}]" +
                        //    $".getElementsByTagName('a').length");
                        //int lenJoin = JsonConvert.DeserializeObject<dynamic>(lenA_obj)["result"]["result"]["value"];

                        if ((lenJoin != null) && (lenJoin == "0"))
                        {
                            var searchBox = chrome.GetCenterEle($"document.getElementsByClassName('x1yztbdb')[{j}].getBoundingClientRect().toJSON()");
                            await Task.Delay(300);
                            if (searchBox[1] < 80)
                            {
                                continue;
                            }
                            else if (searchBox[1] >= 80 && searchBox[1] < winHeight)
                            {
                                Random randomz = new Random();

                                await Task.Delay(500);
                                var namePage_obj = chrome.Eval($"document.getElementsByClassName('x1yztbdb')[{j}].getElementsByClassName('xu06os2 x1ok221b')[0].textContent");
                                string namePage = JsonConvert.DeserializeObject<dynamic>(namePage_obj)["result"]["result"]["value"];
                                var linkPage_obj = chrome.Eval($"document.getElementsByClassName('x1yztbdb')[{j}]" +
                                    $".getElementsByClassName('xu06os2 x1ok221b')[0].getElementsByTagName('a')[0].getAttribute('href')");
                                string linkPage = JsonConvert.DeserializeObject<dynamic>(linkPage_obj)["result"]["result"]["value"];
                                string infoPage = chrome.getValueEle($"document.getElementsByClassName('x1yztbdb')[{j}]" +
                                    $".getElementsByClassName('xu06os2 x1ok221b')[1].textContent");
                                var motaPage_obj = chrome.Eval($"document.getElementsByClassName('x1yztbdb')[{j}]" +
                                    $".getElementsByClassName('xu06os2 x1ok221b')[2].textContent");
                                string motaPage = JsonConvert.DeserializeObject<dynamic>(motaPage_obj)["result"]["result"]["value"];
                                await Task.Delay(500);
                                var searchBoxz = chrome.GetCenterEle($"document.getElementsByClassName('x1yztbdb')[{j}].getElementsByClassName('x6s0dn4 x78zum5 x1q0g3np')[2].getBoundingClientRect().toJSON()");
                                int rdDelayClick = randomz.Next(1500, 5600);
                                await Task.Delay(rdDelayClick);
                                //chrome.MouseClick(searchBoxz[0], searchBoxz[1]);
                                //await Task.Delay(3000);
                                var PheDuyet = chrome.GetCenterEle("document.getElementsByClassName('x1cy8zhl x9f619 x78zum5 xl56j7k x2lwn1j " +
                                    "xeuugli x47corl xurb0ha x1sxyh0 x1x97wu9 xbr3nou x3v4vwv x1dzdb2q')[0]" +
                                    ".getElementsByClassName('x92rtbv x10l6tqk x1tk7jg1 x1vjfegm')[0].getBoundingClientRect().toJSON");
                                if (PheDuyet != null)
                                {
                                    await Task.Delay(1000);
                                    chrome.MouseClick(PheDuyet[0], PheDuyet[1]);
                                    await Task.Delay(3000);
                                    PheDuyet = chrome.GetCenterEle("document.getElementsByClassName('x1cy8zhl x9f619 x78zum5 xl56j7k x2lwn1j " +
                                    "xeuugli x47corl xurb0ha x1sxyh0 x1x97wu9 xbr3nou x3v4vwv x1dzdb2q')[0]" +
                                    ".getElementsByClassName('x92rtbv x10l6tqk x1tk7jg1 x1vjfegm')[0].getBoundingClientRect().toJSON");
                                    //await Task.Delay(3000);
                                    chrome.MouseClick(PheDuyet[0], PheDuyet[1]);
                                    await Task.Delay(1000);
                                    continue;
                                }
                                clickFanpage++;
                                if (clickFanpage % 435 == 0)
                                {
                                    Random randomzz = new Random();

                                    await Task.Delay(randomzz.Next(60000 * 15, 60000 * 40));
                                }
                                //var myLink = new
                                //{
                                //    STT = clickFanpage,
                                //    Keyword = keyWord,
                                //    Location = "Hà Nội",
                                //    Category = typeFilter,
                                //    Name = namePage,
                                //    Link = linkPage,
                                //    Description = motaPage
                                //};
                                if (link_List != null)
                                {
                                    var myLink = new
                                    {
                                        STT = clickFanpage + countLinkList,
                                        Keyword = keyWord,
                                        Location = location,
                                        Category = typeFilter,
                                        Name = namePage,
                                        Link = linkPage,
                                        Info = infoPage,
                                        Description = motaPage,
                                        Profile = profileName.Split(" ")[1],
                                        Join = false
                                    };
                                    if (!link_List.Contains(myLink.Link))
                                    {
                                        //myLink.Profile = profileName.Split(" ")[1];
                                        link_List.Add(myLink);
                                    }
                                    else
                                    {
                                        clickFanpage--;
                                    }
                                }
                                else
                                {
                                    var myLink = new
                                    {
                                        STT = clickFanpage,
                                        Keyword = keyWord,
                                        Location = location,
                                        Category = typeFilter,
                                        Name = namePage,
                                        Link = linkPage,
                                        Info = infoPage,
                                        Description = motaPage,
                                        Profile = profileName.Split(" ")[1],
                                        Join = false
                                    };
                                    link_List = new List<dynamic> { myLink };
                                }
                                if (clickFanpage > Ngroup)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                oldLen = j;
                                break;
                            }
                        }
                        else { continue; }
                    }

                    chrome.Eval($"scrollBy(0, {rdScroll});");
                    await Task.Delay(rdDelay);
                } while (true);
                if (File.Exists(linkPath))
                {
                    link_string = await File.ReadAllTextAsync(linkPath);
                }
                List<dynamic> link_exists = JsonConvert.DeserializeObject<List<dynamic>>(link_string);
                int countLink_exist = link_exists != null ? link_exists.Count : 0;
                if (countLink_exist > 0)
                {
                    link_exists.AddRange(link_List);
                    //for (int i_ = countLink_exist; i_ < link_exists.Count; i_++)
                    //{
                    //    link_exists[i_].STT = i_;
                    //}
                }
                else
                {
                    link_exists = link_List;
                }
                //if (link_List != null)
                //{
                //    link_List.Clear();
                //}
                //await using FileStream createStream = await File.WriteAllTextAsync(linkPath);
                //await JsonSerializer.Serialize(createStream, link_List);
                using (StreamWriter file = File.CreateText(linkPath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, link_exists);
                }
            }
        }
        public async Task searchScrollSmooth(string Profile, string typeFilter, string keyWord, string location, int Ngroup)
        {
            /*
             * Tìm kiếm Nhóm, Trang theo từ khóa
             * Tham gia, theo dõi 3 nhóm, trang
             * Lưu dữ liệu vào CSDL.
             * Cuộn chuột mượt hơn
             */
            int oldLen = 0;
            int JoinFl = 3;
            bool boolHanChe = true;
            string keyWordLocation = $"{keyWord} {location}";
            int clickFanpage = 0;
            chrome.NavigateTo($"https://www.facebook.com/search/{typeFilter}/?q={keyWordLocation}");
            await Task.Delay(timeLoad);
            int winHeight = Convert.ToInt16(chrome.getValueEle("window.innerHeight")) - 20;
            chrome.Eval("document.visibilityState = 'visible'");
            await Task.Delay(500);
            chrome.Eval("document.hidden = false");
            string oldURL = chrome.url();
            if (!Directory.Exists(Environment.CurrentDirectory + $"/Data/Data_Collect/"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + $"/Data/Data_Collect/");
            }
            string linkPath = Environment.CurrentDirectory + $"/Data/Data_Collect/Data_Collected_By_{Profile}.json";
            string link_string = "";
            if (File.Exists(linkPath))
            {
                link_string = await File.ReadAllTextAsync(linkPath);
            }
            List<dynamic> link_List = JsonConvert.DeserializeObject<List<dynamic>>(link_string);
            int countLinkList = link_List != null ? link_List.Count : 0;
            if (countLinkList > 0)
            {
                link_List.Clear();
            }
            if (typeFilter.Equals("posts"))
            {
                var listID = await GetID();
                string mes = "";
                foreach (var it in listID)
                {
                    mes += it + "\n";
                }
            }
            else
            {
                Random random = new Random();
                do
                {
                    await Task.Delay(timeLoad);
                    int rdScroll = random.Next(378, 683);
                    int rdDelay = random.Next(5000, 15600);
                    string lenGroup_obj = chrome.getValueEle("document.getElementsByClassName('x1yztbdb').length");
                    await Task.Delay(500);
                    if (lenGroup_obj != null)
                    {
                        int lenGroup = Convert.ToInt16(lenGroup_obj);
                        await Task.Delay(500);
                        if (clickFanpage >= Ngroup)
                        {
                            break;
                        }
                        //if (clickFanpage >= lenGroup - 1)
                        //{
                        //    countLen++;
                        //    string EndOfResults = chrome.getValueEle("document.getElementsByClassName('x1n2onr6 x1ja2u2z x9f619 x78zum5 xdt5ytf x2lah0s x193iq5w xz9dl7a')[0].textContent");
                        //    if (EndOfResults != null)
                        //    {
                        //        chrome.Eval("document.getElementsByClassName('x1n2onr6 x1ja2u2z x9f619 x78zum5 xdt5ytf x2lah0s x193iq5w xz9dl7a')[0]" +
                        //            ".scrollIntoView({ behavior: 'smooth', block: 'center', inline: 'nearest' });");
                        //        await Task.Delay(2000);
                        //        break;
                        //    }
                        //}
                        for (int j = oldLen; j < lenGroup; j++)
                        {
                            oldLen++;
                            bool action_ = false;
                            await Task.Delay(1000);
                            string lenJoin = "1";
                            lenJoin = chrome.getValueEle($"document.getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z x78zum5 xdt5ytf x193iq5w xeuugli x1r8uery x1iyjqo2 xs83m0k x150jy0e x1e558r4 xjkvuk6 x1iorvi4')[{j}]" +
                                $".getElementsByTagName('a').length");
                            if (lenJoin != null)
                            {
                                if (j % 3 == 0)
                                {
                                    chrome.Eval($"document.getElementsByClassName('x1yztbdb')[{j}].getElementsByClassName('x6s0dn4 x78zum5 x1q0g3np')[2]" +
                                        ".scrollIntoView({ behavior: 'smooth', block: 'center', inline: 'nearest' });");
                                }
                                if (lenJoin == "1")
                                {
                                    action_ = true;
                                }
                                //float[] PheDuyet1 = chrome.GetCenterEle("document.getElementsByClassName('x92rtbv x10l6tqk x1tk7jg1 x1vjfegm')[0].getBoundingClientRect().toJSON()");
                                //if (PheDuyet1 != null)
                                //{
                                //    await Task.Delay(1000);
                                //    chrome.MouseClick(PheDuyet1[0], PheDuyet1[1]);
                                //    await Task.Delay(3000);
                                //    float[] PheDuyet2 = chrome.GetCenterEle("document.getElementsByClassName('x92rtbv x10l6tqk x1tk7jg1 x1vjfegm')[0].getBoundingClientRect().toJSON()");
                                //    await Task.Delay(500);
                                //    if (PheDuyet2 != null)
                                //    {
                                //        chrome.MouseClick(PheDuyet2[0], PheDuyet2[1]);
                                //        await Task.Delay(1000);
                                //    }
                                //}
                                Random randomz = new Random();
                                await Task.Delay(500);
                                string namePage_obj = chrome.Eval($"document.getElementsByClassName('x1yztbdb')[{j}].getElementsByClassName('xu06os2 x1ok221b')[0].textContent");
                                string namePage = JsonConvert.DeserializeObject<dynamic>(namePage_obj)["result"]["result"]["value"];
                                string linkPage_obj = chrome.Eval($"document.getElementsByClassName('x1yztbdb')[{j}]" +
                                    $".getElementsByClassName('xu06os2 x1ok221b')[0].getElementsByTagName('a')[0].getAttribute('href')");
                                string linkPage = JsonConvert.DeserializeObject<dynamic>(linkPage_obj)["result"]["result"]["value"];
                                string infoPage = chrome.getValueEle($"document.getElementsByClassName('x1yztbdb')[{j}]" +
                                    $".getElementsByClassName('xu06os2 x1ok221b')[1].textContent");
                                string motaPage_obj = chrome.Eval($"document.getElementsByClassName('x1yztbdb')[{j}]" +
                                    $".getElementsByClassName('xu06os2 x1ok221b')[2].textContent");
                                string motaPage = JsonConvert.DeserializeObject<dynamic>(motaPage_obj)["result"]["result"]["value"];
                                await Task.Delay(500);
                                int rdDelayClick = randomz.Next(1500, 5600);

                                if ((lenJoin == "0") && (JoinFl > 0) && (rdDelayClick % 2 == 0) && (boolHanChe == true))
                                {
                                    chrome.Eval($"document.getElementsByClassName('x1yztbdb')[{j}].getElementsByClassName('x6s0dn4 x78zum5 x1q0g3np')[2]" +
                                        ".scrollIntoView({ behavior: 'smooth', block: 'center', inline: 'nearest' });");
                                    await Task.Delay(1000);
                                    float[] searchBoxz = chrome.GetCenterEle($"document.getElementsByClassName('x1yztbdb')[{j}].getElementsByClassName('x6s0dn4 x78zum5 x1q0g3np')[2].getBoundingClientRect().toJSON()");
                                    await Task.Delay(500);
                                    try
                                    {
                                        chrome.MouseClick(searchBoxz[0], searchBoxz[1]);
                                    }
                                    catch { }
                                    await Task.Delay(timeLoad);
                                    float[] PheDuyet3 = chrome.GetCenterEle("document.getElementsByClassName('x1cy8zhl x9f619 x78zum5 xl56j7k x2lwn1j " +
                                        "xeuugli x47corl xurb0ha x1sxyh0 x1x97wu9 xbr3nou x3v4vwv x1dzdb2q')[0]" +
                                        ".getElementsByClassName('x92rtbv x10l6tqk x1tk7jg1 x1vjfegm')[0].getBoundingClientRect().toJSON()");
                                    if (PheDuyet3 != null)
                                    {
                                        await Task.Delay(1000);
                                        try
                                        {
                                            chrome.MouseClick(PheDuyet3[0], PheDuyet3[1]);
                                            await Task.Delay(3000);
                                            float[] PheDuyet4 = chrome.GetCenterEle("document.getElementsByClassName('x1cy8zhl x9f619 x78zum5 xl56j7k x2lwn1j " +
                                            "xeuugli x47corl xurb0ha x1sxyh0 x1x97wu9 xbr3nou x3v4vwv x1dzdb2q')[0]" +
                                            ".getElementsByClassName('x92rtbv x10l6tqk x1tk7jg1 x1vjfegm')[0].getBoundingClientRect().toJSON()");
                                            await Task.Delay(2000);
                                            chrome.MouseClick(PheDuyet4[0], PheDuyet4[1]);

                                            //await Task.Delay(1000);
                                            action_ = false;
                                        }
                                        catch
                                        {

                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        action_ = true;
                                        JoinFl--;
                                    }
                                    await Task.Delay(1000);
                                    float[] hanChe = chrome.GetCenterEle("document.getElementsByClassName('x78zum5 xdt5ytf x1qughib x17v04x3 x14rvwrp')[0]" +
                                        ".getElementsByClassName('x16n37ib')[0].getBoundingClientRect().toJSON()");
                                    if (hanChe != null)
                                    {
                                        try
                                        {
                                            await Task.Delay(2000);
                                            chrome.MouseClick(hanChe[0], hanChe[1]);
                                            await Task.Delay(1000);
                                            boolHanChe = false;
                                        }
                                        catch
                                        {
                                        }
                                    }

                                }
                                await Task.Delay(1000);
                                string newURL = chrome.url();
                                if (oldURL != newURL)
                                {
                                    chrome.NavigateTo(oldURL);
                                    await Task.Delay(timeLoad);
                                }
                                await Task.Delay(rdDelayClick);
                                clickFanpage++;
                                if (clickFanpage % 435 == 0)
                                {
                                    Random randomzz = new Random();

                                    await Task.Delay(randomzz.Next(60000 * 5, 60000 * 15));
                                }
                                if (link_List != null)
                                {
                                    var myLink = new
                                    {
                                        STT = clickFanpage + countLinkList,
                                        Keyword = keyWord,
                                        Location = location,
                                        Category = typeFilter,
                                        Name = namePage,
                                        Link = linkPage,
                                        Info = infoPage,
                                        Description = motaPage,
                                        Profile = profileName.Split(" ")[1],
                                        Join = action_
                                    };
                                    if (!link_List.Contains(myLink.Link))
                                    {
                                        //myLink.Profile = profileName.Split(" ")[1];
                                        link_List.Add(myLink);
                                    }
                                    else
                                    {
                                        clickFanpage--;
                                    }
                                }
                                else
                                {
                                    var myLink = new
                                    {
                                        STT = clickFanpage,
                                        Keyword = keyWord,
                                        Location = location,
                                        Category = typeFilter,
                                        Name = namePage,
                                        Link = linkPage,
                                        Info = infoPage,
                                        Description = motaPage,
                                        Profile = profileName.Split(" ")[1],
                                        Join = action_
                                    };
                                    link_List = new List<dynamic> { myLink };
                                }

                            }
                            else
                            {
                                clickFanpage--;
                                continue;
                            }
                        }
                        await Task.Delay(1000);
                        string EndOfResults = chrome.getValueEle("document.getElementsByClassName('x1n2onr6 x1ja2u2z x9f619 x78zum5 xdt5ytf x2lah0s x193iq5w xz9dl7a')[0].textContent");
                        if (EndOfResults != null)
                        {
                            chrome.Eval("document.getElementsByClassName('x1n2onr6 x1ja2u2z x9f619 x78zum5 xdt5ytf x2lah0s x193iq5w xz9dl7a')[0]" +
                                ".scrollIntoView({ behavior: 'smooth', block: 'center', inline: 'nearest' });");
                            await Task.Delay(2000);
                            break;
                        }
                        chrome.Eval($"scrollBy(0, {rdScroll});");
                        await Task.Delay(rdDelay);
                    }


                } while (true);
                if (File.Exists(linkPath))
                {
                    link_string = await File.ReadAllTextAsync(linkPath);
                }
                List<dynamic> link_exists = JsonConvert.DeserializeObject<List<dynamic>>(link_string);
                int countLink_exist = link_exists != null ? link_exists.Count : 0;
                if (countLink_exist > 0)
                {
                    link_exists.AddRange(link_List);

                }
                else
                {
                    link_exists = link_List;
                }
                using (StreamWriter file = File.CreateText(linkPath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, link_exists);
                }
            }
        }
        public async Task searchForKeyWord(string Profile, string typeFilter, string keyWord, string location, int Ngroup)
        {
            /*
             * Tìm kiếm trang, nhóm theo từ khóa không thực hiện theo dõi, tham gia bất cứ trang nhóm nào.
             * Lưu dữ liệu vào CSDL
             */
            int heightMonitor = SystemInformation.PrimaryMonitorSize.Height;
            int oldLen = 0;
            int JoinFl = 3;
            bool boolHanChe = true;
            string keyWordLocation = $"{keyWord} {location}";
            int clickFanpage = 0;
            chrome.NavigateTo($"https://www.facebook.com/search/{typeFilter}/?q={keyWordLocation}");
            await Task.Delay(timeLoad);
            int winHeight = Convert.ToInt16(chrome.getValueEle("window.innerHeight")) - 20;
            chrome.Eval("document.visibilityState = 'visible'");
            await Task.Delay(500);
            chrome.Eval("document.hidden = false");
            if (!Directory.Exists(Environment.CurrentDirectory + $"/Data/Data_Collect/"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + $"/Data/Data_Collect/");
            }
            string linkPath = Environment.CurrentDirectory + $"/Data/Data_Collect/Data_Collected_By_{Profile}.json";
            string link_string = "";
            if (File.Exists(linkPath))
            {
                link_string = await File.ReadAllTextAsync(linkPath);
            }
            List<dynamic> link_List = JsonConvert.DeserializeObject<List<dynamic>>(link_string);
            int countLinkList = link_List != null ? link_List.Count : 0;
            if (countLinkList > 0)
            {
                link_List.Clear();
            }
            if (typeFilter.Equals("posts"))
            {
                var listID = await GetID();
                string mes = "";
                foreach (var it in listID)
                {
                    mes += it + "\n";
                }
            }
            else
            {
                int countLen = 0;
                Random random = new Random();
                do
                {
                    await Task.Delay(timeLoad);
                    int rdScroll = random.Next(578, 783);
                    int rdDelay = random.Next(5000, 15600);
                    string lenGroup_obj = chrome.getValueEle("document.getElementsByClassName('x1yztbdb').length");
                    await Task.Delay(500);
                    if (lenGroup_obj != null)
                    {
                        int lenGroup = Convert.ToInt16(lenGroup_obj);
                        await Task.Delay(500);
                        if (clickFanpage >= Ngroup)
                        {
                            break;
                        }

                        if (clickFanpage >= lenGroup - 1)
                        {
                            countLen++;
                            string EndOfResults = chrome.getValueEle("document.getElementsByClassName('x1n2onr6 x1ja2u2z x9f619 x78zum5 xdt5ytf x2lah0s x193iq5w xz9dl7a')[0].textContent");
                            if (EndOfResults != null)
                            {
                                break;
                            }
                        }
                        for (int j = oldLen; j < lenGroup; j++)
                        {
                            bool action_ = false;
                            await Task.Delay(1000);
                            string lenJoin = chrome.getValueEle($"document.getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z x78zum5 xdt5ytf x193iq5w xeuugli x1r8uery x1iyjqo2 xs83m0k x150jy0e x1e558r4 xjkvuk6 x1iorvi4')[{j}]" +
                                $".getElementsByTagName('a').length");
                            if (lenJoin != null)
                            {
                                if (lenJoin == "1")
                                {
                                    action_ = true;
                                }
                                var searchBox = chrome.GetCenterEle($"document.getElementsByClassName('x1yztbdb')[{j}].getBoundingClientRect().toJSON()");
                                await Task.Delay(300);
                                if (searchBox[1] < 80)
                                {
                                    continue;
                                }
                                else if (searchBox[1] >= 80 && searchBox[1] < winHeight)
                                {
                                    Random randomz = new Random();

                                    await Task.Delay(500);
                                    var namePage_obj = chrome.Eval($"document.getElementsByClassName('x1yztbdb')[{j}].getElementsByClassName('xu06os2 x1ok221b')[0].textContent");
                                    string namePage = JsonConvert.DeserializeObject<dynamic>(namePage_obj)["result"]["result"]["value"];
                                    var linkPage_obj = chrome.Eval($"document.getElementsByClassName('x1yztbdb')[{j}]" +
                                        $".getElementsByClassName('xu06os2 x1ok221b')[0].getElementsByTagName('a')[0].getAttribute('href')");
                                    string linkPage = JsonConvert.DeserializeObject<dynamic>(linkPage_obj)["result"]["result"]["value"];
                                    string infoPage = chrome.getValueEle($"document.getElementsByClassName('x1yztbdb')[{j}]" +
                                        $".getElementsByClassName('xu06os2 x1ok221b')[1].textContent");
                                    var motaPage_obj = chrome.Eval($"document.getElementsByClassName('x1yztbdb')[{j}]" +
                                        $".getElementsByClassName('xu06os2 x1ok221b')[2].textContent");
                                    string motaPage = JsonConvert.DeserializeObject<dynamic>(motaPage_obj)["result"]["result"]["value"];
                                    await Task.Delay(500);
                                    int rdDelayClick = randomz.Next(1500, 5600);

                                    if (lenJoin == "0" && JoinFl > 0 && rdDelayClick % 2 == 0 && boolHanChe == true)
                                    {
                                        var searchBoxz = chrome.GetCenterEle($"document.getElementsByClassName('x1yztbdb')[{j}].getElementsByClassName('x6s0dn4 x78zum5 x1q0g3np')[2].getBoundingClientRect().toJSON()");
                                        await Task.Delay(500);
                                        chrome.MouseClick(searchBoxz[0], searchBoxz[1]);
                                        await Task.Delay(timeLoad);
                                        var PheDuyet = chrome.GetCenterEle("document.getElementsByClassName('x1cy8zhl x9f619 x78zum5 xl56j7k x2lwn1j " +
                                            "xeuugli x47corl xurb0ha x1sxyh0 x1x97wu9 xbr3nou x3v4vwv x1dzdb2q')[0]" +
                                            ".getElementsByClassName('x92rtbv x10l6tqk x1tk7jg1 x1vjfegm')[0].getBoundingClientRect().toJSON()");
                                        if (PheDuyet != null)
                                        {
                                            await Task.Delay(1000);
                                            chrome.MouseClick(PheDuyet[0], PheDuyet[1]);
                                            await Task.Delay(3000);
                                            PheDuyet = chrome.GetCenterEle("document.getElementsByClassName('x1cy8zhl x9f619 x78zum5 xl56j7k x2lwn1j " +
                                            "xeuugli x47corl xurb0ha x1sxyh0 x1x97wu9 xbr3nou x3v4vwv x1dzdb2q')[0]" +
                                            ".getElementsByClassName('x92rtbv x10l6tqk x1tk7jg1 x1vjfegm')[0].getBoundingClientRect().toJSON()");
                                            //await Task.Delay(3000);
                                            chrome.MouseClick(PheDuyet[0], PheDuyet[1]);

                                            await Task.Delay(1000);
                                            action_ = false;
                                            //continue;
                                        }
                                        else
                                        {
                                            action_ = true;
                                            JoinFl--;
                                        }
                                        await Task.Delay(1000);
                                        float[] hanChe = chrome.GetCenterEle("document.getElementsByClassName(\"x78zum5 xdt5ytf x1qughib x17v04x3 x14rvwrp\")[0].getElementsByClassName('x16n37ib')[0].getBoundingClientRect().toJSON()");
                                        if (hanChe != null)
                                        {
                                            await Task.Delay(500);
                                            chrome.MouseClick(hanChe[0], hanChe[1]);
                                            await Task.Delay(2000);
                                            boolHanChe = false;
                                        }

                                    }

                                    await Task.Delay(rdDelayClick);
                                    clickFanpage++;
                                    if (clickFanpage % 435 == 0)
                                    {
                                        Random randomzz = new Random();

                                        await Task.Delay(randomzz.Next(60000 * 5, 60000 * 15));
                                    }
                                    if (link_List != null)
                                    {
                                        var myLink = new
                                        {
                                            STT = clickFanpage + countLinkList,
                                            Keyword = keyWord,
                                            Location = location,
                                            Category = typeFilter,
                                            Name = namePage,
                                            Link = linkPage,
                                            Info = infoPage,
                                            Description = motaPage,
                                            Profile = profileName.Split(" ")[1],
                                            Join = action_
                                        };
                                        if (!link_List.Contains(myLink.Link))
                                        {
                                            //myLink.Profile = profileName.Split(" ")[1];
                                            link_List.Add(myLink);
                                        }
                                        else
                                        {
                                            clickFanpage--;
                                        }
                                    }
                                    else
                                    {
                                        var myLink = new
                                        {
                                            STT = clickFanpage,
                                            Keyword = keyWord,
                                            Location = location,
                                            Category = typeFilter,
                                            Name = namePage,
                                            Link = linkPage,
                                            Info = infoPage,
                                            Description = motaPage,
                                            Profile = profileName.Split(" ")[1],
                                            Join = action_
                                        };
                                        link_List = new List<dynamic> { myLink };
                                    }
                                }
                                else
                                {
                                    oldLen = j;
                                    break;
                                }
                            }
                            else
                            {
                                clickFanpage--;
                                continue;
                            }
                        }

                        chrome.Eval($"scrollBy(0, {rdScroll});");
                        await Task.Delay(rdDelay);
                    }


                } while (true);
                if (File.Exists(linkPath))
                {
                    link_string = await File.ReadAllTextAsync(linkPath);
                }
                List<dynamic> link_exists = JsonConvert.DeserializeObject<List<dynamic>>(link_string);
                int countLink_exist = link_exists != null ? link_exists.Count : 0;
                if (countLink_exist > 0)
                {
                    link_exists.AddRange(link_List);

                }
                else
                {
                    link_exists = link_List;
                }
                using (StreamWriter file = File.CreateText(linkPath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, link_exists);
                }
            }
        }
        public async Task JoinGroup(string link)
        {
            chrome.NavigateTo(link);
            bool Join = false;
            await Task.Delay(timeLoad);
            chrome.Eval("document.getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z x78zum5 x2lah0s xl56j7k x1qjc9v5 xozqiw3 x1q0g3np x1l90r2v x1ve1bff')[0]" +
                ".getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z x78zum5 xdt5ytf x2lah0s x193iq5w x150jy0e x1e558r4 xjkvuk6 x1iorvi4 xac96w6 x1azkghk " +
                "xouvc7r x1mva0g0')[0].getElementsByTagName('div')[0].click()");
            Join = true;
            var PheDuyet = chrome.GetCenterEle("document.getElementsByClassName('x1cy8zhl x9f619 x78zum5 xl56j7k x2lwn1j " +
                                    "xeuugli x47corl xurb0ha x1sxyh0 x1x97wu9 xbr3nou x3v4vwv x1dzdb2q')[0]" +
                                    ".getElementsByClassName('x92rtbv x10l6tqk x1tk7jg1 x1vjfegm')[0].getBoundingClientRect().toJSON()");
            if (PheDuyet != null)
            {
                await Task.Delay(1000);
                chrome.MouseClick(PheDuyet[0], PheDuyet[1]);
                await Task.Delay(3000);
                PheDuyet = chrome.GetCenterEle("document.getElementsByClassName('x1cy8zhl x9f619 x78zum5 xl56j7k x2lwn1j " +
                "xeuugli x47corl xurb0ha x1sxyh0 x1x97wu9 xbr3nou x3v4vwv x1dzdb2q')[0]" +
                ".getElementsByClassName('x92rtbv x10l6tqk x1tk7jg1 x1vjfegm')[0].getBoundingClientRect().toJSON()");
                //await Task.Delay(3000);
                chrome.MouseClick(PheDuyet[0], PheDuyet[1]);
                await Task.Delay(1000);
                Join = false;
            }
        }
        public async Task FollowPage(string link)
        {
            /*
             * Like & follow Fanpage
             */
            bool Follow = false;
            chrome.NavigateTo(link);
            await Task.Delay(timeLoad);
            string strLenSearch = chrome.getValueEle("document.getElementsByClassName('x78zum5 x1a02dak x139jcc6 xcud41i x9otpla x1ke80iy')[0]" +
                ".getElementsByClassName('xh8yej3').length");
            int lenLike = Convert.ToInt32(strLenSearch);
            for (int i = 0; i < lenLike; i++)
            {
                string textValue = chrome.getValueEle("document.getElementsByClassName('x78zum5 x1a02dak x139jcc6 xcud41i x9otpla x1ke80iy')[0]" +
                    $".getElementsByClassName('xh8yej3')[{i}].textContent");
                if (textValue != null)
                {
                    if ((textValue == "Like") || (textValue == "Thích") || (textValue == "Follow") || (textValue == "Theo dõi"))
                    {
                        chrome.Eval("document.getElementsByClassName('x78zum5 x1a02dak x139jcc6 xcud41i x9otpla x1ke80iy')[0]" +
                            $".getElementsByClassName('xh8yej3')[{i}].click()");
                        Follow = true;
                        await Task.Delay(1000);
                    }
                }
            }
        }
        public async Task getInfoGroup(string link)
        {
            /*
             * Lấy thông tin của nhóm
             */
            await Task.Delay(1000);
            chrome.NavigateTo(link);
            await Task.Delay(timeLoad);
            await Task.Delay(1000);
            string nameGroup = chrome.getValueEle("document.getElementsByClassName('x78zum5 xdt5ytf x1wsgfga x9otpla')[0]" +
                ".getElementsByTagName('a')[0].textContent");
            chrome.Eval("document.getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z x2lah0s x1qjc9v5 x78zum5 x1q0g3np x1a02dak xl56j7k x9otpla x1n0m28w x1wsgfga xp7jhwk')[0]" +
                ".getElementsByClassName('x1i10hfl xjbqb8w x6umtig x1b1mbwd xaqea5y xav7gou x9f619 x1ypdohk xt0psk2 xe8uvvx xdj266r x11i5rnm xat24cr " +
                "x1mh8g0r xexx8yu x4uap5 x18d9i69 xkhd6sd x16tdsg8 x1hl2dhg xggy1nq x1a2a7pz xt0b8zv xzsf02u x1s688f')[0].click()");
            await Task.Delay(500);
            string groupsRule = chrome.getValueEle("document.getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z x2lah0s x1qjc9v5 x78zum5 x1q0g3np x1a02dak xl56j7k x9otpla x1n0m28w x1wsgfga xp7jhwk')[0]" +
                ".getElementsByClassName('xod5an3')[0].innerText");
            string groupsActivaty = chrome.getValueEle("document.getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z x2lah0s x1qjc9v5 x78zum5 x1q0g3np x1a02dak xl56j7k x9otpla x1n0m28w x1wsgfga xp7jhwk')[0]" +
                ".getElementsByClassName('x1wsgfga')[2].innerText");
            string groupAbout = chrome.getValueEle("document.getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z x2lah0s x1qjc9v5 x78zum5 x1q0g3np x1a02dak xl56j7k x9otpla x1n0m28w x1wsgfga xp7jhwk')[0]" +
                ".getElementsByClassName('x1wsgfga')[0].innerText");
            //string category = chrome.getValueEle("document.getElementsByClassName('bp9cbjyn j83agx80 bkfpd7mw')[0]" +
            //    ".getElementsByClassName('a8c37x1j ni8dbmo4 stjgntxs l9j0dhe7')[1].textContent");
            //string info = chrome.getValueEle("document.getElementsByClassName('bp9cbjyn j83agx80 bkfpd7mw')[0]" +
            //    ".getElementsByClassName('a8c37x1j ni8dbmo4 stjgntxs l9j0dhe7')[2].textContent");
            //string description = chrome.getValueEle("document.getElementsByClassName('bp9cbjyn j83agx80 bkfpd7mw')[0]" +
            //    ".getElementsByClassName('a8c37x1j ni8dbmo4 stjgntxs l9j0dhe7')[3].textContent");
            //string member = chrome.getValueEle("document.getElementsByClassName('bp9cbjyn j83agx80 bkfpd7mw')[0]" +
            //    ".getElementsByClassName('a8c37x1j ni8dbmo4 stjgntxs l9j0dhe7')[4].textContent");
            //string privacy = chrome.getValueEle("document.getElementsByClassName('bp9cbjyn j83agx80 bkfpd7mw')[0]" +
            //    ".getElementsByClassName('a8c37x1j ni8dbmo4 stjgntxs l9j0dhe7')[5].textContent");
            string url = chrome.url();
            string[] urlSplit = url.Split('/');
            string idGroup = urlSplit[urlSplit.Length - 1];
            string[] infoGroup = new string[] { nameGroup, groupAbout, groupsActivaty, groupsRule, idGroup };
            string infoGroup_string = JsonConvert.SerializeObject(infoGroup);
            await File.WriteAllTextAsync(Environment.CurrentDirectory + "/Data/Data_Collect/infoGroup.txt", infoGroup_string);
        }
        public async Task searchGoogle(string Keyword)
        {
            /* 
             * Truy cập google và thực hiện tìm kiếm trên google
             */
            chrome.NavigateTo($"https://www.google.com/search?q={Keyword}");
            await Task.Delay(timeLoad);
            string stringLenSearch = chrome.getValueEle("document.getElementById('search').getElementsByClassName('MjjYud').length");
            int lenSearch = Convert.ToInt16(stringLenSearch);
            for (int i = 0; i < lenSearch; i++)
            {
                chrome.Eval("document.getElementById('search')" +
                    $".getElementsByClassName('MjjYud')[{i}]" +
                    ".scrollIntoView({ behavior: 'smooth', block: 'center', inline: 'nearest' });");
                Random random = new Random();
                int rd = random.Next(1000, 3400);
                await Task.Delay(rd);
                if (rd % 2 == 0)
                {
                    chrome.Eval("document.getElementById('search').getElementsByClassName('MjjYud')[0].getElementsByTagName('a')[0].click();");
                }
            }
        }
        public async Task ChangeAvatar()
        {
            /* 
             * Thay đổi avatar FB
             */
            await Task.Delay(1000);
            chrome.NavigateTo("http://fb.com/me");
            await Task.Delay(timeLoad);
            // Đổi avt
            chrome.NavigateTo("https://www.facebook.com/profile/avatar/?entry_point=avatar_profile_picture");
            int rd = random.Next(63);
            await Task.Delay(timeLoad);
            chrome.Eval($"document.getElementsByClassName('x8gbvx8 x78zum5 x1a02dak xh8yej3')[0]" +
                $".getElementsByTagName('div')[{rd}].click()");
            await Task.Delay(1500);
            //chrome.MouseClick(svgAVT[0], svgAVT[1]);
            //await Task.Delay(500);
            var svgAVT = chrome.GetCenterEle("document.getElementsByClassName('x17adc0v xh8yej3')[0].getBoundingClientRect().toJSON()");
            await Task.Delay(1000);
            chrome.MouseClick(svgAVT[0], svgAVT[1]);
            await Task.Delay(10000);
            svgAVT = chrome.GetCenterEle("document.getElementsByClassName('x1c4vz4f x2lah0s x65caj5 xzi3mdb')[0].getBoundingClientRect().toJSON()");
            chrome.MouseClick(svgAVT[0], svgAVT[1]);
            rd = random.Next(7);
            await Task.Delay(timeLoad);
            svgAVT = chrome.GetCenterEle($"document.getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z x78zum5 xdt5ytf x2lah0s x193iq5w x5ib6vp xc73u3c x1y1aw1k xwib8y2')[0]" +
                $".getElementsByClassName('x1rg5ohu x1n2onr6 x3ajldb x1ja2u2z')[{rd}].getBoundingClientRect().toJSON()");
            await Task.Delay(500);
            chrome.MouseClick(svgAVT[0], svgAVT[1]);
            rd = random.Next(13);
            await Task.Delay(500);
            svgAVT = chrome.GetCenterEle($"document.getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z x78zum5 xdt5ytf x2lah0s x193iq5w x5ib6vp xc73u3c x1y1aw1k xwib8y2')[1]" +
                $".getElementsByClassName('x1rg5ohu x1n2onr6 x3ajldb x1ja2u2z')[{rd}].getBoundingClientRect().toJSON()");
            await Task.Delay(500);
            chrome.MouseClick(svgAVT[0], svgAVT[1]);
            await Task.Delay(500);
            svgAVT = chrome.GetCenterEle("document.getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z x78zum5 xdt5ytf x2lah0s x193iq5w xeuugli')[0]" +
                ".getBoundingClientRect().toJSON()");
            await Task.Delay(500);
            chrome.MouseClick(svgAVT[0], svgAVT[1]);
            await Task.Delay(timeLoad);
            chrome.NavigateTo("http://fb.com/me");
            await Task.Delay(timeLoad);
            var svgCover = chrome.GetCenterEle("document.getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z xeuugli x1r8uery x1iyjqo2 xs83m0k x6s0dn4 x78zum5 " +
                "xdt5ytf xl56j7k x1uyial6 x1rr25im x1xh8ygx')[0].getBoundingClientRect().toJSON()");
            await Task.Delay(500);
            chrome.MouseClick(svgCover[0], svgCover[1]);
            await Task.Delay(1000);
            svgCover = chrome.GetCenterEle("document.getElementsByClassName('x1qjc9v5 x3vj7og x78zum5 xdt5ytf x1n2onr6 x1al4vs7')[0]" +
                ".getElementsByClassName('x78zum5 xdt5ytf xz62fqu x16ldp7u')[2].getBoundingClientRect().toJSON()");
            await Task.Delay(500);
            chrome.MouseClick(svgCover[0], svgCover[1]);
            await Task.Delay(timeLoad);
            chrome.Eval("document.getElementsByClassName('x78zum5 xdt5ytf x1iyjqo2 x7ywyr2')[0]" +
                ".getElementsByClassName('x78zum5 x1q0g3np x1a02dak xh8yej3')[0].scrollBy(0, -500)");
            await Task.Delay(1000);
            chrome.Eval("document.getElementsByClassName('x78zum5 xdt5ytf x1iyjqo2 x7ywyr2')[0]" +
                ".getElementsByClassName('x78zum5 x1q0g3np x1a02dak xh8yej3')[0].scrollBy(0, -500)");
            await Task.Delay(1000);
            chrome.Eval("document.getElementsByClassName('x78zum5 xdt5ytf x1iyjqo2 x7ywyr2')[0]" +
                ".getElementsByClassName('x78zum5 x1q0g3np x1a02dak xh8yej3')[0].scrollBy(0, -500)");
            await Task.Delay(1000);
            rd = random.Next(27);
            await Task.Delay(500);

            chrome.Eval("document.getElementsByClassName('x78zum5 xdt5ytf x1iyjqo2 x7ywyr2')[0]" +
                $".getElementsByClassName('x78zum5 x1q0g3np x1a02dak xh8yej3')[0].getElementsByTagName('img')[{rd}].click()");
            await Task.Delay(1000);
            rd = random.Next(18);
            chrome.Eval("document.getElementsByClassName('x78zum5 xdt5ytf x1iyjqo2 x7ywyr2')[0]" +
                $".getElementsByClassName('x78zum5 x1q0g3np x1a02dak xh8yej3')[1].getElementsByTagName('img')[{rd}].click()");
            await Task.Delay(500);
            svgCover = chrome.GetCenterEle("document.getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z x78zum5 xdt5ytf x2lah0s x193iq5w " +
                "x1r8uery xl9nvqe xwfmwtl')[0].getBoundingClientRect().toJSON()");
            await Task.Delay(500);
            chrome.MouseClick(svgCover[0], svgCover[1]);
            await Task.Delay(timeLoad);
            await Task.Delay(3000);

        }
        public async Task SwitchPage()
        {
            /*
             * Chuyển giữa Trang cá nhân -> Trang (Fanpage) và ngược lại
             */
            await Task.Delay(1000);
            var svgAvatr = chrome.GetCenterEle("document.getElementsByClassName('xds687c x1pi30zi x1e558r4 xixxii4 x13vifvy xzkaem6')[0]" +
                ".getElementsByTagName('svg')[0].getBoundingClientRect().toJSON()");
            await Task.Delay(500);
            chrome.MouseClick(svgAvatr[0], svgAvatr[1]);
            await Task.Delay(1000);
            svgAvatr = chrome.GetCenterEle("document.getElementsByClassName('x1n2onr6 x1lliihq x14atkfc')[1].getBoundingClientRect().toJSON()");
            await Task.Delay(500);
            chrome.MouseClick(svgAvatr[0], svgAvatr[1]);
            await Task.Delay(1000);
            var lenPage_obj = chrome.Eval("document.getElementsByClassName('x9f619 x1ja2u2z x1k90msu x6o7n8i x1qfuztq x10l6tqk x17qophe x13vifvy x1hc1fzr x71s49j xh8yej3')[0]" +
                ".getElementsByClassName('x1b0d499 x1d69dk1').length");
            int x_img = JsonConvert.DeserializeObject<dynamic>(lenPage_obj)["result"]["result"]["value"]; // số lượng fanpage
            svgAvatr = chrome.GetCenterEle("document.getElementsByClassName('x9f619 x1ja2u2z x1k90msu x6o7n8i x1qfuztq x10l6tqk x17qophe x13vifvy x1hc1fzr x71s49j xh8yej3')[0]" +
                $".getElementsByClassName('x1b0d499 x1d69dk1')[{x_img - 1}].getBoundingClientRect().toJSON()");
            await Task.Delay(500);
            chrome.MouseClick(svgAvatr[0], svgAvatr[1]);
            await Task.Delay(2000);
        }
        public async Task PostToID(PhanHoi binhluan)
        {
            /*
             * Đăng bình luận vào bài viết theo ID
             */
            chrome.NavigateTo(binhluan.Link);
            await Task.Delay(timeLoad);
			var xy_cor_click = chrome.GetCenterEle("document.getElementsByClassName('xzsf02u x1a2a7pz x1n2onr6 x14wi4xw notranslate')[0].getBoundingClientRect().toJSON()");
			await Task.Delay(1000);
			// thay hành động cuộn chuột
			chrome.MouseClick(xy_cor_click[0], xy_cor_click[1]);
			await Task.Delay(1000);
			chrome.SendText(" ");
            // giờ mới là comment nè
			await Task.Delay(1000);
			var xy_cor_click_2 = chrome.GetCenterEle("document.getElementsByClassName('xzsf02u x1a2a7pz x1n2onr6 x14wi4xw notranslate')[0].getBoundingClientRect().toJSON()");
			await Task.Delay(1000);
			chrome.MouseClick(xy_cor_click_2[0], xy_cor_click_2[1]);
            chrome.SendText(binhluan.Content);
			await Task.Delay(1000);
			if (binhluan.Image != null)
            {
				string pathAnh = binhluan.Image.Trim();
				pathAnh = pathAnh.Replace(@"/", @"\");

				if (pathAnh.Length > 0)
				{
					await Task.Delay(1000);
					var xy_cor = chrome.GetCenterEle("document.getElementsByClassName('x1rg5ohu x1mnrxsn x1w0mnb')[2].getBoundingClientRect().toJSON()");
					await Task.Delay(1000);
					chrome.MouseClick(xy_cor[0], xy_cor[1]);
					await Task.Delay(1300);
					StringBuilder pathIMG = new StringBuilder(pathAnh);
					await Task.Run(() => SendIMG(pathIMG));
					await Task.Delay(2000);
				}
			}
			var xy_cor_sub = chrome.GetCenterEle("document.getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z x78zum5 x2lah0s x1qughib x6s0dn4 xozqiw3 x1q0g3np xcud41i x139jcc6 x4cne27 xifccgj')[0].getBoundingClientRect().toJSON()");
			await Task.Delay(1000);
			chrome.MouseClick(xy_cor_sub[0], xy_cor_sub[1]);
			await Task.Delay(3000);

            await Task.Delay(timeLoad);
        }
		public async Task CommentMultiPostInGroup(string content, string linkGroup, string imgLink)
		{
			//chrome.NavigateTo(linkGroup);
			//Thread.Sleep(5000);
			int oldLen = 0;
			var post = await GetPostInGroup(linkGroup, 2);
			foreach (var p in post)
			{
				chrome.NavigateTo(p);
				Thread.Sleep(5000);

				await Comment(content, imgLink);

				Thread.Sleep(timeLoad);
			}
		}
		public async Task<List<string>> GetIDMulti(string linkgr = "")
		{
			/* Lấy ID bài viết */
			int oldLen = 0;
			await Task.Delay(2000);
			if (linkgr != "")
			{
				chrome.NavigateTo(linkgr);
				await Task.Delay(timeLoad);
			}
			var arLink = new List<string>();
			for (int i = 0; i < 10; i++)
			{
				chrome.Eval("scrollBy(0, 600);");
				await Task.Delay(1000);
				var lenBaiViet = chrome.Eval("document.getElementsByClassName('x1yztbdb x1n2onr6 xh8yej3 x1ja2u2z').length");
				int lenBaiViet_int = JsonConvert.DeserializeObject<dynamic>(lenBaiViet)["result"]["result"]["value"];
				for (int j = oldLen; j < lenBaiViet_int; j++)
				{
					var x_co = chrome.Eval($"document.getElementsByClassName('x1yztbdb x1n2onr6 xh8yej3 x1ja2u2z')[{j}]" +
						$".getElementsByClassName('xu06os2 x1ok221b')[1]" +
						$".getElementsByClassName('x4k7w5x x1h91t0o x1h9r5lt x1jfb8zj xv2umb2 x1beo9mf xaigb6o x12ejxvf x3igimt xarpa2k xedcshv x1lytzrv x1t2pt76 x7ja8zs x1qrby5j')[0]" +
						$".getElementsByTagName('a')[0].getBoundingClientRect().x + 10;");
					var y_co = chrome.Eval($"document.getElementsByClassName('x1yztbdb x1n2onr6 xh8yej3 x1ja2u2z')[{j}]" +
						$".getElementsByClassName('xu06os2 x1ok221b')[1]" +
						$".getElementsByClassName('x4k7w5x x1h91t0o x1h9r5lt x1jfb8zj xv2umb2 x1beo9mf xaigb6o x12ejxvf x3igimt xarpa2k xedcshv x1lytzrv x1t2pt76 x7ja8zs x1qrby5j')[0]" +
						$".getElementsByTagName('a')[0].getBoundingClientRect().y + 10;");
					try
					{
						float x_j = JsonConvert.DeserializeObject<dynamic>(x_co)["result"]["result"]["value"];
						float y_j = JsonConvert.DeserializeObject<dynamic>(y_co)["result"]["result"]["value"];
						//Delay(1);
						await Task.Delay(100);
						if (y_j < 127)
						{
							continue;
						}
						else if (y_j >= 127 && y_j < 950)
						{
							chrome.MouseMove(x_j, y_j);
							await Task.Delay(1000);
							var obLink = chrome.Eval($"document.getElementsByClassName('x1yztbdb x1n2onr6 xh8yej3 x1ja2u2z')[{j}]" +
								$".getElementsByClassName('xu06os2 x1ok221b')[1]" +
								$".getElementsByClassName('x4k7w5x x1h91t0o x1h9r5lt x1jfb8zj xv2umb2 x1beo9mf xaigb6o x12ejxvf x3igimt" +
								$" xarpa2k xedcshv x1lytzrv x1t2pt76 x7ja8zs x1qrby5j')[0].getElementsByTagName('a')[0].getAttribute('href')");
							string linkO = JsonConvert.DeserializeObject<dynamic>(obLink)["result"]["result"]["value"];
							int vtHoiCham = linkO.IndexOf("?");
							if (vtHoiCham == -1)
							{
								continue;
							}
							string linkOriginal = linkO.Substring(0, vtHoiCham);
							if (!arLink.Contains(linkOriginal) && linkOriginal.Contains("posts")) { arLink.Add(linkOriginal); }
						}
						else
						{
							oldLen = j;
							break;
						}
					}
					catch
					{
						continue;
					}
				}
			}
			return arLink;
		}
		public async Task Comment(string content, string img)
		{
			Thread.Sleep(500);
			// kéo xuống vùng cần comment
			var buttonComment1 = chrome.GetCenterEle($"document.getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z x78zum5 xdt5ytf x193iq5w xeuugli x1r8uery x1iyjqo2 xs83m0k xg83lxy x1h0ha7o x10b6aqq x1yrsyyn')[1]" +
												".getBoundingClientRect().toJSON()");
			chrome.Eval($"scrollBy(0, {buttonComment1[1] - buttonComment1[3] * 3})");
			Thread.Sleep(5000);
			//click nút comment
			var buttonComment2 = chrome.GetCenterEle($"document.getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z x78zum5 xdt5ytf x193iq5w xeuugli x1r8uery x1iyjqo2 xs83m0k xg83lxy x1h0ha7o x10b6aqq x1yrsyyn')[1]" +
												".getBoundingClientRect().toJSON()");
			chrome.MouseClick(buttonComment2[0], buttonComment2[1]);
			Thread.Sleep(2000);
			//gõ cmt
			chrome.SendText(content);
			Thread.Sleep(2000);
            //check ảnh
            await PostImg(img);
			//submit
			var buttonSubmit = chrome.GetCenterEle($"document.getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z x78zum5 x2lah0s x1qughib x6s0dn4 xozqiw3 x1q0g3np xcud41i x139jcc6 x4cne27 xifccgj')[0].getBoundingClientRect().toJSON()");
			Thread.Sleep(1000);
			chrome.MouseClick(buttonSubmit[0], buttonSubmit[1]);
			Thread.Sleep(5000);
		}
		public async Task<List<string>> GetPostInGroup(string linkgr = "", int numPost = 1)
		{
			/* Lấy ID bài viết */
			int oldLen = 0;
			if (linkgr != "")
			{
				chrome.NavigateTo(linkgr);
				Thread.Sleep(timeLoad);
			}
			var arLink = new List<string>();
			for (int i = 0; i < 10; i++)
			{
				chrome.Eval("scrollBy(0, 600);");
				Thread.Sleep(1000);
				var lenBaiViet = chrome.Eval("document.getElementsByClassName('x1yztbdb x1n2onr6 xh8yej3 x1ja2u2z').length");
				int lenBaiViet_int = JsonConvert.DeserializeObject<dynamic>(lenBaiViet)["result"]["result"]["value"];
				for (int j = oldLen; j < lenBaiViet_int && arLink.Count() < numPost; j++)
				{
					var x_co = chrome.Eval($"document.getElementsByClassName('x1yztbdb x1n2onr6 xh8yej3 x1ja2u2z')[{j}]" +
						$".getElementsByClassName('xu06os2 x1ok221b')[1]" +
						$".getElementsByClassName('x4k7w5x x1h91t0o x1h9r5lt x1jfb8zj xv2umb2 x1beo9mf xaigb6o x12ejxvf x3igimt xarpa2k xedcshv x1lytzrv x1t2pt76 x7ja8zs x1qrby5j')[0]" +
						$".getElementsByTagName('a')[0].getBoundingClientRect().x + 10;");
					var y_co = chrome.Eval($"document.getElementsByClassName('x1yztbdb x1n2onr6 xh8yej3 x1ja2u2z')[{j}]" +
						$".getElementsByClassName('xu06os2 x1ok221b')[1]" +
						$".getElementsByClassName('x4k7w5x x1h91t0o x1h9r5lt x1jfb8zj xv2umb2 x1beo9mf xaigb6o x12ejxvf x3igimt xarpa2k xedcshv x1lytzrv x1t2pt76 x7ja8zs x1qrby5j')[0]" +
						$".getElementsByTagName('a')[0].getBoundingClientRect().y + 10;");
					try
					{
						float x_j = JsonConvert.DeserializeObject<dynamic>(x_co)["result"]["result"]["value"];
						float y_j = JsonConvert.DeserializeObject<dynamic>(y_co)["result"]["result"]["value"];
						//Delay(1);
						Thread.Sleep(100);
						if (y_j < 127)
						{
							continue;
						}
						else if (y_j >= 127 && y_j < 950)
						{
							chrome.MouseMove(x_j, y_j);
							Thread.Sleep(1000);
							var obLink = chrome.Eval($"document.getElementsByClassName('x1yztbdb x1n2onr6 xh8yej3 x1ja2u2z')[{j}]" +
								$".getElementsByClassName('xu06os2 x1ok221b')[1]" +
								$".getElementsByClassName('x4k7w5x x1h91t0o x1h9r5lt x1jfb8zj xv2umb2 x1beo9mf xaigb6o x12ejxvf x3igimt" +
								$" xarpa2k xedcshv x1lytzrv x1t2pt76 x7ja8zs x1qrby5j')[0].getElementsByTagName('a')[0].getAttribute('href')");
							string linkO = JsonConvert.DeserializeObject<dynamic>(obLink)["result"]["result"]["value"];
							int vtHoiCham = linkO.IndexOf("?");
							if (vtHoiCham == -1)
							{
								continue;
							}
							string linkOriginal = linkO.Substring(0, vtHoiCham);
							if (!arLink.Contains(linkOriginal) && linkOriginal.Contains("posts")) { arLink.Add(linkOriginal); }
						}
						else
						{
							oldLen = j;
							break;
						}
					}
					catch
					{
						continue;
					}
				}
			}
			return arLink;
		}
		public async Task PostImg(string link)
		{
			link = link.Replace(@"/", @"\");

			if (link.Length > 0)
			{
				Thread.Sleep(1000);
				var buttonImg = chrome.GetCenterEle($"document.getElementsByClassName('x1rg5ohu x1mnrxsn x1w0mnb')[7].getBoundingClientRect().toJSON()");

				Thread.Sleep(1000);
				chrome.MouseClick(buttonImg[0], buttonImg[1]);
				Thread.Sleep(1000);

				StringBuilder pathIMG = new StringBuilder(link);
				SendIMG(pathIMG);
				Thread.Sleep(5000);
			}
		}
		public async Task LikeAndShare(bool like, bool share, string noidungCMT, string ID = "")
        {
            /*
             * Thích
             * Chia sẻ các bài viết
             */
            await Task.Delay(1000);
            if (ID != "")
            {
                chrome.NavigateTo(ID);
                await Task.Delay(timeLoad);
            }
            var svgcmtBox = chrome.GetCenterEle("document.getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z x78zum5 xdt5ytf x193iq5w xeuugli x1r8uery x1iyjqo2 xs83m0k xg83lxy x1h0ha7o x10b6aqq x1yrsyyn')[0]" +
                ".getBoundingClientRect().toJSON()");
            chrome.Eval($"scrollBy(0, {svgcmtBox[1] - svgcmtBox[3] * 3})");
            await Task.Delay(1000);
            if (like)
            {
                svgcmtBox = chrome.GetCenterEle("document.getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z x78zum5 xdt5ytf x193iq5w xeuugli x1r8uery x1iyjqo2 xs83m0k xg83lxy x1h0ha7o x10b6aqq x1yrsyyn')[0]" +
                ".getBoundingClientRect().toJSON()");
                await Task.Delay(500);
                chrome.MouseClick(svgcmtBox[0], svgcmtBox[1]);
            }
            await Task.Delay(1000);
            if (share)
            {
                string loaiNhom = chrome.getValueEle("document.getElementsByClassName('x1yztbdb x1n2onr6 xh8yej3 x1ja2u2z')[0]" +
                    ".getElementsByClassName('xu06os2 x1ok221b')[1]" +
                    ".getElementsByClassName('x4k7w5x x1h91t0o x1h9r5lt x1jfb8zj xv2umb2 x1beo9mf xaigb6o x12ejxvf x3igimt xarpa2k xedcshv x1lytzrv x1t2pt76 x7ja8zs x1qrby5j')[1]" +
                    ".getElementsByTagName('path').length");
                if (loaiNhom == "3")
                {
                    svgcmtBox = chrome.GetCenterEle("document.getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z x78zum5 xdt5ytf x193iq5w xeuugli x1r8uery x1iyjqo2 xs83m0k xg83lxy x1h0ha7o x10b6aqq x1yrsyyn')[2]" +
                    ".getBoundingClientRect().toJSON()");
                    await Task.Delay(500);
                    chrome.MouseClick(svgcmtBox[0], svgcmtBox[1]);
                    await Task.Delay(1000);
                    svgcmtBox = chrome.GetCenterEle("document.getElementsByClassName('xt7dq6l x1a2a7pz x6ikm8r x10wlt62 x1n2onr6 x14atkfc')[0]" +
                        ".getElementsByClassName('xsag5q8 xz9dl7a')[0].getElementsByTagName('div')[12]" +
                        ".getBoundingClientRect().toJSON()");
                    await Task.Delay(500);
                    chrome.MouseClick(svgcmtBox[0], svgcmtBox[1]);
                    await Task.Delay(1000);
                    chrome.SendText(noidungCMT);
                    await Task.Delay(1000);
                    var btn_Submit = chrome.GetCenterEle("document.getElementsByClassName('x6s0dn4 x9f619 x78zum5 x1qughib x1pi30zi x1swvt13 xyamay9 xh8yej3')[0]" +
                    ".getElementsByTagName('div')[0].getBoundingClientRect().toJSON()");
                    await Task.Delay(500);
                    chrome.MouseClick(btn_Submit[0], btn_Submit[1]);
                    await Task.Delay(4000);
                }
                else
                {
                }
            }
            await Task.Delay(1000);
        }
        public async Task postStories(dynamic formStories)
        {
            /*
             * Đăng lên Stories
             */
            chrome.NavigateTo("https://www.facebook.com/stories/create");
            await Task.Delay(timeLoad);
            if (formStories.bool_Anh == true)
            {
                var clickBox = chrome.GetCenterEle("document.getElementsByClassName('x78zum5 x1qughib xh8yej3')[0]" +
                    ".getElementsByClassName('xqtp20y x1n2onr6 xh8yej3')[0].getBoundingClientRect().toJSON()");
                await Task.Delay(500);
                chrome.MouseClick(clickBox[0], clickBox[1]);
                await Task.Delay(1000);
                StringBuilder pathIMG = new StringBuilder();
                foreach (var ele in formStories.pathAnh)
                {
                    pathIMG.Append("\"");
                    pathIMG.Append(ele.ToString());
                    pathIMG.Append("\"");
                }
                Task task = new Task(() => SendIMG(pathIMG));
                task.Start();
                task.Wait();
                await Task.Delay(2000);
                await Task.Delay(1500);
                if (formStories.noiDung != null)
                {
                    clickBox = chrome.GetCenterEle("document.getElementsByClassName('x1xmf6yo x78zum5 xdt5ytf x1iyjqo2')[0]" +
                        ".getElementsByClassName('x1o1ewxj x3x9cwd x1e5q0jg x13rtm0m x1ey2m1c xds687c xg01cxk x47corl x10l6tqk x17qophe " +
                        "x13vifvy x1ebt8du x19991ni x1dhq9h x1wpzbip')[0].getBoundingClientRect().toJSON()");
                    await Task.Delay(500);
                    chrome.MouseClick(clickBox[0], clickBox[1]);
                    await Task.Delay(1000);
                    chrome.SendText(formStories.noiDung);
                    await Task.Delay(1000);
                }
            }
            else
            {
                var clickBox = chrome.GetCenterEle("document.getElementsByClassName('x78zum5 x1qughib xh8yej3')[0]" +
                    ".getElementsByClassName('xqtp20y x1n2onr6 xh8yej3')[1].getBoundingClientRect().toJSON()");
                await Task.Delay(500);
                chrome.MouseClick(clickBox[0], clickBox[1]);
                await Task.Delay(1000);
                clickBox = chrome.GetCenterEle("document.getElementsByClassName('xjbqb8w x1iyjqo2 x193iq5w xeuugli x1n2onr6')[0]" +
                    ".getBoundingClientRect().toJSON()");
                await Task.Delay(500);
                chrome.MouseClick(clickBox[0], clickBox[1]);
                await Task.Delay(1000);
                chrome.SendText(formStories.noiDung);
                await Task.Delay(1000);

            }
            var clickPost = chrome.GetCenterEle("document.getElementsByClassName('x6s0dn4 x1jx94hy x10h3on x78zum5 x1q0g3np xy75621 x1qughib x1ye3gou xn6708d')[0]" +
                        ".getElementsByClassName('x1iyjqo2 x1vqgdyp xsgj6o6 xw3qccf')[1].getBoundingClientRect().toJSON()");
            await Task.Delay(500);
            chrome.MouseClick(clickPost[0], clickPost[1]);
            await Task.Delay(2000);
        }
        public async Task postReels(dynamic formReels)
        {
            /*
             * Đăng Reels
             */
            await Task.Delay(500);
            chrome.NavigateTo("https://www.facebook.com/reels/create");
            await Task.Delay(timeLoad);
            var boxClick = chrome.GetCenterEle("document.getElementsByClassName('xexx8yu xn6708d x18d9i69 x1ye3gou')[0].getBoundingClientRect().toJSON()");
            await Task.Delay(1000);
            chrome.MouseClick(boxClick[0], boxClick[1]);
            await Task.Delay(1000);
            StringBuilder pathIMG = new StringBuilder();
            foreach (var ele in formReels.pathAnh)
            {
                pathIMG.Append("\"");
                pathIMG.Append(ele.ToString());
                pathIMG.Append("\"");
            }
            Task task = new Task(() => SendIMG(pathIMG));
            task.Start();
            task.Wait();
            await Task.Delay(2000);
            boxClick = chrome.GetCenterEle("document.getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z x78zum5 xdt5ytf x193iq5w xeuugli x1iyjqo2 xs83m0k x150jy0e x1e558r4 xjkvuk6 x1iorvi4 xdl72j9')[0]" +
                ".getBoundingClientRect().toJSON()");
            await Task.Delay(500);
            chrome.MouseClick(boxClick[0], boxClick[1]);
            await Task.Delay(1000);
            boxClick = chrome.GetCenterEle("document.getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z x78zum5 xdt5ytf x193iq5w xeuugli x1iyjqo2 xs83m0k x150jy0e x1e558r4 xjkvuk6 x1iorvi4 xdl72j9')[1]" +
                ".getBoundingClientRect().toJSON()");
            await Task.Delay(500);
            chrome.MouseClick(boxClick[0], boxClick[1]);
            await Task.Delay(1000);
            boxClick = chrome.GetCenterEle("document.getElementsByClassName('x1xb5f1y xx7dd87 xawzvin x1hoxbwm xhk9q7s x1otrzb0 x1i1ezom x1o6z2jb xzsf02u x78zum5 x1hkcv85 xseoqlg x1odjw0f xyamay9 x1pi30zi x1l90r2v x1swvt13 x1a2a7pz')[0]" +
                ".getBoundingClientRect().toJSON()");
            await Task.Delay(500);
            chrome.MouseClick(boxClick[0], boxClick[1]);
            chrome.SendText(formReels.noiDung);
            await Task.Delay(500);
            boxClick = chrome.GetCenterEle("document.getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z x78zum5 xdt5ytf x193iq5w xeuugli x1iyjqo2 xs83m0k x150jy0e x1e558r4 xjkvuk6 x1iorvi4 xdl72j9')[1]" +
                ".getBoundingClientRect().toJSON()");
            await Task.Delay(500);
            chrome.MouseClick(boxClick[0], boxClick[1]);
            await Task.Delay(5000);
            // Nếu muốn lấy link reel vừa đăng thì lấy url sau khi tải xong là được

        }
        public async Task GuiKB()
        {
            /*
             * Gửi yêu cầu kết bạn từ gợi ý của Facebook
             */
            chrome.NavigateTo("https://www.facebook.com/friends/suggestions");
            await Task.Delay(timeLoad);
            int idx = random.Next(0, 40);
            var btnAddFr = chrome.GetCenterEle("document.getElementsByClassName('x1n2onr6 x1ja2u2z x78zum5 x2lah0s xl56j7k x6s0dn4 xozqiw3 x1q0g3np xi112ho " +
                $"x17zwfj4 x585lrc x1403ito x972fbf xcfux6l x1qhh985 xm0m39n x9f619 xn6708d x1ye3gou xtvsq51 x1r1pt67')[{idx}].getBoundingClientRect().toJSON()");
            if (btnAddFr != null)
            {
                chrome.Eval($"scrollBy(0, {btnAddFr[1] - btnAddFr[3] * 3})");
                await Task.Delay(2000);
                btnAddFr = chrome.GetCenterEle("document.getElementsByClassName('x1n2onr6 x1ja2u2z x78zum5 x2lah0s xl56j7k x6s0dn4 xozqiw3 x1q0g3np xi112ho " +
                $"x17zwfj4 x585lrc x1403ito x972fbf xcfux6l x1qhh985 xm0m39n x9f619 xn6708d x1ye3gou xtvsq51 x1r1pt67')[{idx}].getBoundingClientRect().toJSON()");
                chrome.MouseClick(btnAddFr[0], btnAddFr[1]);
                await Task.Delay(2000);
            }
        }
        public async Task AcceptKB()
        {
            /*
             * Đồng ý kết bạn
             */
            chrome.NavigateTo("https://www.facebook.com/friends/requests");
            await Task.Delay(timeLoad);
            var btnAddFr = chrome.GetCenterEle("document.getElementsByClassName('x1n2onr6 x1ja2u2z x78zum5 x2lah0s xl56j7k x6s0dn4 xozqiw3 x1q0g3np xi112ho " +
                $"x17zwfj4 x585lrc x1403ito x972fbf xcfux6l x1qhh985 xm0m39n x9f619 xn6708d x1ye3gou xtvsq51 x1r1pt67')[0].getBoundingClientRect().toJSON()");
            if (btnAddFr != null)
            {
                chrome.Eval($"scrollBy(0, {btnAddFr[1] - btnAddFr[3] * 3})");
                await Task.Delay(2000);
                btnAddFr = chrome.GetCenterEle("document.getElementsByClassName('x1n2onr6 x1ja2u2z x78zum5 x2lah0s xl56j7k x6s0dn4 xozqiw3 x1q0g3np xi112ho " +
                $"x17zwfj4 x585lrc x1403ito x972fbf xcfux6l x1qhh985 xm0m39n x9f619 xn6708d x1ye3gou xtvsq51 x1r1pt67')[0].getBoundingClientRect().toJSON()");
                chrome.MouseClick(btnAddFr[0], btnAddFr[1]);
                await Task.Delay(2000);
            }
        }
        public async Task JoinGroupAsync(string keySearch, int Ngroup)
        {
            /* 
             * Tham gia nhóm
             */
            int oldLen = 0;
            int clickGroups = 0;
            chrome.NavigateTo("https://www.facebook.com/groups/feed/");
            await Task.Delay(timeLoad);
            var searchBox = chrome.GetCenterEle("document.getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z xod5an3 x1gslohp')[0]" +
                ".getElementsByTagName('input')[0].getBoundingClientRect().toJSON()");
            await Task.Delay(400);
            chrome.MouseClick(searchBox[0], searchBox[1]);
            await Task.Delay(500);
            chrome.SendText(keySearch);
            await Task.Delay(500);
            chrome.PressEnter();
            await Task.Delay(timeLoad);
            for (int i = 0; i < 10; i++)
            {
                if (clickGroups < Ngroup)
                {
                    var lenGroup_obj = chrome.Eval("document.getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z x78zum5 xdt5ytf x193iq5w xeuugli " +
                        "x1r8uery x1iyjqo2 xs83m0k x150jy0e x1e558r4 xjkvuk6 x1iorvi4').length");
                    int lenGroup = JsonConvert.DeserializeObject<dynamic>(lenGroup_obj)["result"]["result"]["value"];
                    for (int j = oldLen; j < lenGroup; j++)
                    {
                        var lenA_obj = chrome.Eval("document.getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z x78zum5 xdt5ytf x193iq5w xeuugli " +
                        $"x1r8uery x1iyjqo2 xs83m0k x150jy0e x1e558r4 xjkvuk6 x1iorvi4')[{j}].getElementsByTagName('a').length");
                        int lenJoin = JsonConvert.DeserializeObject<dynamic>(lenA_obj)["result"]["result"]["value"];
                        if (lenJoin == 0)
                        {
                            searchBox = chrome.GetCenterEle("document.getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z x78zum5 xdt5ytf x193iq5w xeuugli " +
                        $"x1r8uery x1iyjqo2 xs83m0k x150jy0e x1e558r4 xjkvuk6 x1iorvi4')[{j}].getBoundingClientRect().toJSON()");
                            await Task.Delay(300);
                            if (searchBox[1] < 127)
                            {
                                continue;
                            }
                            else if (searchBox[1] >= 127 && searchBox[1] < 950)
                            {
                                chrome.MouseClick(searchBox[0], searchBox[1]);
                                await Task.Delay(3000);
                                var PheDuyet = chrome.GetCenterEle("document.getElementsByClassName('x1cy8zhl x9f619 x78zum5 xl56j7k x2lwn1j " +
                                    "xeuugli x47corl xurb0ha x1sxyh0 x1x97wu9 xbr3nou x3v4vwv x1dzdb2q')[0]" +
                                    ".getElementsByClassName('x92rtbv x10l6tqk x1tk7jg1 x1vjfegm')[0].getBoundingClientRect().toJSON");
                                if (PheDuyet != null)
                                {
                                    await Task.Delay(1000);
                                    chrome.MouseClick(PheDuyet[0], PheDuyet[1]);
                                    await Task.Delay(3000);
                                    PheDuyet = chrome.GetCenterEle("document.getElementsByClassName('x1cy8zhl x9f619 x78zum5 xl56j7k x2lwn1j " +
                                    "xeuugli x47corl xurb0ha x1sxyh0 x1x97wu9 xbr3nou x3v4vwv x1dzdb2q')[0]" +
                                    ".getElementsByClassName('x92rtbv x10l6tqk x1tk7jg1 x1vjfegm')[0].getBoundingClientRect().toJSON");
                                    chrome.MouseClick(PheDuyet[0], PheDuyet[1]);
                                    await Task.Delay(1000);
                                    continue;
                                }
                                clickGroups++;
                                var groupLink_Obj = chrome.Eval("document.getElementsByClassName('x9f619 x1n2onr6 x1ja2u2z x78zum5 xdt5ytf x193iq5w xeuugli " +
                                    $"x1r8uery x1iyjqo2 xs83m0k x150jy0e x1e558r4 xjkvuk6 x1iorvi4')[{j}].getElementsByTagName('a')[0].getAttribute('href')");
                                string groupLink_Join = JsonConvert.DeserializeObject<dynamic>(groupLink_Obj)["result"]["result"]["value"];
                                var groupName_Obj = chrome.Eval($"document.getElementsByClassName('x1yztbdb')[{j}]" +
                                    ".getElementsByClassName('xu06os2 x1ok221b')[0].textContent");
                                string groupName_Join = JsonConvert.DeserializeObject<dynamic>(groupName_Obj)["result"]["result"]["value"];
                                string link_String = "";
                                var myLink = new
                                {
                                    Name = groupName_Join,
                                    Link = groupLink_Join
                                };
                                string linkPath = Environment.CurrentDirectory + "Data/Link_group.json";
                                if (File.Exists(linkPath))
                                {
                                    link_String = File.ReadAllText(linkPath);
                                }
                                List<dynamic> link_List = JsonConvert.DeserializeObject<List<dynamic>>(link_String);

                                if (link_List != null)
                                {
                                    link_List.Add(myLink);
                                }
                                else
                                {
                                    link_List = new List<dynamic> { myLink };
                                }
                                using (StreamWriter file = File.CreateText(linkPath))
                                {
                                    JsonSerializer serializer = new JsonSerializer();
                                    serializer.Serialize(file, link_List);
                                }
                                if (clickGroups >= Ngroup)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                oldLen = j;
                                break;
                            }
                        }
                    }
                    chrome.Eval("scroll(0, 600);");
                    await Task.Delay(1000);
                }
            }
        }
        private const UInt32 WM_CLOSE = 0x0010;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
        public void CloseBrowser()
        {
            string title = chrome.getValueEle("document.title");
            if (title != null)
            {
                IntPtr hwnd = FindHandle(IntPtr.Zero, null, title + " - Brave");
                if (hwnd != IntPtr.Zero)
                {
                    SendMessage(hwnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                }
                else
                {
                    hwnd = FindHandle(IntPtr.Zero, null, title);
                    SendMessage(hwnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                }

            }
        }
    }

}