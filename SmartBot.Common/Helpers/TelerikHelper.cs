using SmartBot.Common.Configuration;
using SmartBot.Common.Enums;
using SmartBot.Common.Extention;
using System.Text;

namespace SmartBot.Common.Helpers
{
	public class TelerikHelper
	{
		public static string PathFileTypeExtention(string fileType)
		{
			var result = AppConfigs.FullPath + "extention/"+fileType.Replace(".","")+"/"+ fileType.Replace(".", "") + ".png";
            return result;
		}
	}
}
