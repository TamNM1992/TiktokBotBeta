
using SmartBot.Common;
using SmartBot.DataDto.User;

namespace SmartBot.Service.Api.Users
{
    public interface IUsersApiServices
    {
        //public ResponseBase<int> InsertUser(UserInsertDataDto param);
        public ResponseBase<LoginDto> CheckUserByAccount(string email, string password);
        public ResponseBase<LoginDto> CheckUserByToken(string token);


    }
}
