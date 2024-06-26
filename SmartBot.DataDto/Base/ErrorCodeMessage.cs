namespace SmartBot.DataDto.Base
{
	public class ErrorCodeMessage
	{
		#region Return Code (0 - 99): Common error
		public static readonly KeyValuePair<int, string> Success = new KeyValuePair<int, string>(0, "The operation completed successfully.");
		public static readonly KeyValuePair<int, string> Exception = new KeyValuePair<int, string>(1, "An error occurred.");
		public static readonly KeyValuePair<int, string> NotFound = new KeyValuePair<int, string>(2, "Not Found.");
		public static readonly KeyValuePair<int, string> NoPermission = new KeyValuePair<int, string>(3, "No Permission");

		#endregion
	}

}
