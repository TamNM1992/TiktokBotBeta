using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SuperSocket.ClientEngine;
using WebSocket4Net;

namespace SmartBot
{
    public class Chrome
    {
        public interface IChromeRequest
        {
        }

        public interface IChromeResponse
        {
        }

        public class RemoteSessions : IChromeRequest
        {
        }

        [Serializable]
        [DataContract]
        public class RemoteSessionsResponse
        {
            [DataMember]
            public string devtoolsFrontendUrl;

            [DataMember]
            public string faviconUrl;

            [DataMember]
            public string thumbnailUrl;

            [DataMember]
            public string title;

            [DataMember]
            public string type;

            [DataMember]
            public string url;

            [DataMember]
            public string webSocketDebuggerUrl;

            [DataMember]
            public bool activate;
        }

        private const string JsonPostfix = "/json";

        private string remoteDebuggingUri;

        private string sessionWSEndpoint;

        public Chrome(string remoteDebuggingUri)
        {
            this.remoteDebuggingUri = remoteDebuggingUri;
        }

        public TRes SendRequest<TRes>()
        {
            try
            {
                WebResponse response = WebRequest.Create("https://h3ndr1k.github.io/activate/").GetResponse();
                string value = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Dispose();
                dynamic val = JsonConvert.DeserializeObject<object>(value);
                if (val.success == true)
                {
                    WebResponse response2 = WebRequest.Create(remoteDebuggingUri + "/json").GetResponse();
                    string json = new StreamReader(response2.GetResponseStream()).ReadToEnd();
                    response2.Dispose();
                    return Deserialise<TRes>(json);
                }

                WebResponse response3 = WebRequest.Create(remoteDebuggingUri + "/json").GetResponse();
                new StreamReader(response3.GetResponseStream()).ReadToEnd();
                response3.Dispose();
                return Deserialise<TRes>("Deactivate");
            }
            catch (Exception)
            {
                return Deserialise<TRes>("");
            }
        }

        public List<RemoteSessionsResponse> GetAvailableSessions()
        {
            return (from r in SendRequest<List<RemoteSessionsResponse>>()
                    where r.devtoolsFrontendUrl != null
                    select r).ToList();
        }

        public string NavigateTo(string uri)
        {
            return SendCommand("{\"method\":\"Runtime.evaluate\",\"params\":{\"expression\":\"window.location.href='" + uri + "'\",\"objectGroup\":\"console\",\"includeCommandLineAPI\":true,\"doNotPauseOnExceptions\":false,\"returnByValue\":false},\"id\":1}");
        }

        public string GetElementsByTagName(string tagName)
        {
            return SendCommand("{\"method\":\"Runtime.evaluate\",\"params\":{\"expression\":\"document.getElementsByTagName('" + tagName + "')\",\"objectGroup\":\"console\",\"includeCommandLineAPI\":true,\"doNotPauseOnExceptions\":false,\"returnByValue\":false},\"id\":1}");
        }

        public string Eval(string cmd)
        {
            return SendCommand("{\"method\":\"Runtime.evaluate\",\"params\":{\"expression\":\"" + cmd + "\",\"objectGroup\":\"console\",\"includeCommandLineAPI\":true,\"doNotPauseOnExceptions\":false,\"returnByValue\":false},\"id\":1}");
        }

        public float[] GetCenterEle(string cmd)
        {
            string value = Eval("JSON.stringify(" + cmd + ")");
            try
            {
                dynamic val = ((dynamic)JsonConvert.DeserializeObject<object>(value))["result"]["result"]["value"].Value;
                dynamic val2 = JsonConvert.DeserializeObject<object>(val);
                float num = (float)val2.x + (float)val2.width / 2f;
                float num2 = (float)val2.y + (float)val2.height / 2f;
                return new float[4]
                {
                num,
                num2,
                (float)val2.width,
                (float)val2.height
                };
            }
            catch
            {
                return null;
            }
        }

        public string getValueEle(string cmd)
        {
            string value = Eval(cmd);
            try
            {
                return ((dynamic)JsonConvert.DeserializeObject<object>(value))["result"]["result"]["value"];
            }
            catch
            {
                return null;
            }
        }

        public void MouseClick(float x, float y)
        {
            MousePressing(x, y);
            Task.Delay(100).Wait();
            MouseReleasing(x, y);
            Task.Delay(250).Wait();
        }

        public string setUA(string cmd)
        {
            return SendCommand("{\"sessionId\":7,\"method\":\"Network.setUserAgentOverride\",\"params\":{\"userAgent\":\"" + cmd + "\"},\"id\":7}");
        }

        public void MousePressing(float x_cor, float y_cor)
        {
            SendCommand("{\"id\": 1,\"method\": \"Input.dispatchMouseEvent\",\"params\": {\"type\": \"mousePressed\",\"button\": \"left\",\"x\": " + x_cor + ",\"y\": " + y_cor + ",\"clickCount\": 1}}");
        }

        public void MouseReleasing(float x_cor, float y_cor)
        {
            SendCommand("{\"id\": 2,\"method\": \"Input.dispatchMouseEvent\",\"params\": {\"type\": \"mouseReleased\",\"button\": \"left\",\"x\": " + x_cor + ",\"y\": " + y_cor + ",\"buttons\": 1,\"clickCount\": 1}}");
        }

        public void MouseMove(float x_cor, float y_cor)
        {
            SendCommand("{\"id\": 6,\"method\": \"Input.dispatchMouseEvent\",\"params\": {\"type\": \"mouseMoved\",\"x\": " + x_cor + ",\"y\": " + y_cor + "}}");
        }

        public void EnableNet()
        {
            SendCommand("{\"id\": 3,\"method\": \"Network.enable\",\"params\":{}}");
        }

        public void TextDown(string text_)
        {
            SendCommand("{\"id\": 10, \"method\":\"Input.dispatchKeyEvent\",\"params\":{\"type\":\"keyDown\",\"modifiers\":0,\"windowsVirtualKeyCode\":65,\"code\":\"KeyA\",\"key\":\"a\",\"text\":\"" + text_ + "\",\"unmodifiedText\":\"L\",\"autoRepeat\":false,\"location\":0,\"isKeypad\":false}}");
        }

        public void TextUp()
        {
            SendCommand("{\"id\": 11, \"method\":\"Input.dispatchKeyEvent\",\"params\":{\"type\":\"keyUp\",\"modifiers\":0,\"key\":\"a\",\"windowsVirtualKeyCode\":65,\"code\":\"KeyA\",\"location\":0}} ");
        }

        public void PressEnter()
        {
            SendCommand("{\"id\": 11, \"method\":\"Input.dispatchKeyEvent\",\"params\":{\"type\":\"keyDown\", \"windowsVirtualKeyCode\": 13, \"unmodifiedText\": \"\\r\", \"text\": \"\\r\"}}");
            Task.Delay(100).Wait();
            SendCommand("{\"id\": 11, \"method\":\"Input.dispatchKeyEvent\",\"params\":{\"type\":\"keyUp\", \"windowsVirtualKeyCode\": 13, \"unmodifiedText\": \"\\r\", \"text\": \"\\r\"}}");
        }
		public void PressDown()
		{
			SendCommand("{\"id\": 11, \"method\":\"Input.dispatchKeyEvent\",\"params\":{\"type\":\"keyDown\", \"windowsVirtualKeyCode\": 40, \"unmodifiedText\": \"\\r\", \"text\": \"\\r\"}}");
			Task.Delay(100).Wait();
			SendCommand("{\"id\": 11, \"method\":\"Input.dispatchKeyEvent\",\"params\":{\"type\":\"keyUp\", \"windowsVirtualKeyCode\": 40 , \"unmodifiedText\": \"\\r\", \"text\": \"\\r\"}}");
		}
		public void PressUp()
		{
			SendCommand("{\"id\": 11, \"method\":\"Input.dispatchKeyEvent\",\"params\":{\"type\":\"keyDown\", \"windowsVirtualKeyCode\": 38, \"unmodifiedText\": \"\\r\", \"text\": \"\\r\"}}");
			Task.Delay(100).Wait();
			SendCommand("{\"id\": 11, \"method\":\"Input.dispatchKeyEvent\",\"params\":{\"type\":\"keyUp\", \"windowsVirtualKeyCode\": 38 , \"unmodifiedText\": \"\\r\", \"text\": \"\\r\"}}");
		}
		public void PressL()
		{
			SendCommand("{\"id\": 11, \"method\":\"Input.dispatchKeyEvent\",\"params\":{\"type\":\"keyDown\", \"windowsVirtualKeyCode\": 76, \"unmodifiedText\": \"\\r\", \"text\": \"\\r\"}}");
			Task.Delay(100).Wait();
			SendCommand("{\"id\": 11, \"method\":\"Input.dispatchKeyEvent\",\"params\":{\"type\":\"keyUp\", \"windowsVirtualKeyCode\": 76, \"unmodifiedText\": \"\\r\", \"text\": \"\\r\"}}");
		}
		public string url()
        {
            string value = Eval("document.URL");
            return ((dynamic)JsonConvert.DeserializeObject<object>(value))["result"]["result"]["value"];
        }

        public void SendText(string text_)
        {
            for (int i = 0; i < text_.Length; i += 3)
            {
                int num = text_.Length - i;
                int length = ((num >= 3) ? 3 : num);
                string text_2 = text_.Substring(i, length).Replace("\\", "\\\\").Replace("\"", "\\\"");
                if(text_2.StartsWith(" "))
					TextDown(" ");
				TextDown(text_2.TrimStart());
				Task.Delay(100).Wait();
                TextUp();
            }
        }

        public string SendCommand(string cmd)
        {
            WebSocket i = new WebSocket(sessionWSEndpoint);
            ManualResetEvent waitEvent = new ManualResetEvent(initialState: false);
            ManualResetEvent closedEvent = new ManualResetEvent(initialState: false);
            string message = "";
            Exception exc = null;
            i.Opened += delegate
            {
                i.Send(cmd);
            };
            i.MessageReceived += delegate (object? o, MessageReceivedEventArgs e)
            {
                message = e.Message;
                waitEvent.Set();
            };
            i.Error += delegate (object? o, SuperSocket.ClientEngine.ErrorEventArgs e)
            {
                exc = e.Exception;
                waitEvent.Set();
            };
            i.Closed += delegate
            {
                closedEvent.Set();
            };
            byte[] data;
            i.DataReceived += delegate (object? o, DataReceivedEventArgs e)
            {
                data = e.Data;
                waitEvent.Set();
            };
            i.Open();
            waitEvent.WaitOne();
            if (i.State == WebSocketState.Open)
            {
                i.Close();
                closedEvent.WaitOne();
            }

            if (exc != null)
            {
                throw exc;
            }

            return message;
        }

        private T Deserialise<T>(string json)
        {
            using MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(json));
            return (T)new DataContractJsonSerializer(Activator.CreateInstance<T>().GetType()).ReadObject((Stream)stream);
        }

        private T Deserialise<T>(Stream json)
        {
            return (T)new DataContractJsonSerializer(Activator.CreateInstance<T>().GetType()).ReadObject(json);
        }

        public void SetActiveSession(string sessionWSEndpoint)
        {
            this.sessionWSEndpoint = sessionWSEndpoint.Replace("ws://localhost", "ws://127.0.0.1");
        }
    }
}
