namespace SmartBot.Common
{
    public class ResponseBase<T>
    {
        public ResponseBase()
        {
            Message = ErrorCodeMessage.Success.Value;
            Code = ErrorCodeMessage.Success.Key;
        }

        public int Code { get; set; }
        public string Message { get; set; }
        public int Count { get; set; }
        public bool IsSuccessful => Code == ErrorCodeMessage.Success.Key;

        public T Data { get; set; }

    }

    public class ResponseBase : ResponseBase<object>
    {
    }
    public class AppSettings
    {
        public string Secret { get; set; }
    }
}
