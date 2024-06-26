using SmartBot.Common;
using SmartBot.DataDto.User;

namespace SmartBot.Service.Api.Users
{
    public class UsersApiServices : ApiServiceBase, IUsersApiServices
    {
        //public ResponseBase<int> InsertUser(UserInsertDataDto param)
        //{
        //    var response = Post<UserInsertDataDto, int>("User/insert", param);
        //    return response;
        //}
        public ResponseBase<LoginDto> CheckUserByAccount(string email, string password)
        {
            var response = Get<LoginDto>("user/login-account"
                , new KeyValuePair<string, object>("email", email)
                , new KeyValuePair<string, object>("password", password));
            return response;
        }
        public ResponseBase<LoginDto> CheckUserByToken(string token)
        {
            var response = Get<LoginDto>("user/check-token"
                , new KeyValuePair<string, object>("token", token));
            return response;
        }
    }
}
