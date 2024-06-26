using SmartBot.Common.Enums;
using SmartBot.Common.Helpers;
using System.ComponentModel.Design;
using System.Timers;

namespace SmartBot.Common.Timer
{
	public class AutoRun
	{
		static int minute = 10;
		static System.Timers.Timer aTimer = new System.Timers.Timer();
		static double _time = 60000 * minute;
		public static void Init()
		{
			try
			{
				FileHelper.GeneratorFileByDay(FileStype.Log, $"Khởi động hệ thống lúc {DateTime.Now.ToString("HH:mm:ss")}.", "Init");
				aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
				aTimer.Interval = _time;
				aTimer.Enabled = true;
			}
			catch (Exception ex)
			{
				FileHelper.GeneratorFileByDay(FileStype.Error, ex.ToString(), "Init");
			}
		}
		private static void OnTimedEvent(object source, ElapsedEventArgs e)
		{
			var now = DateTime.Now.TimeOfDay;

			if(now > new TimeSpan(6,0,0) && now <= new TimeSpan(6, minute, 0))
			{
				// post
			}
			else if (now > new TimeSpan(8, 0, 0) && now <= new TimeSpan(8, minute, 0))
			{
				// comment
			}
			else if (now > new TimeSpan(16, 0, 0) && now <= new TimeSpan(16, minute, 0))
			{
				// post
			}
			else if (now > new TimeSpan(18, 0, 0) && now <= new TimeSpan(18, minute, 0))
			{
				// comment    
			}
		}
	}
}
