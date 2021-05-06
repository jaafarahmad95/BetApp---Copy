using System.Threading.Tasks;
using testtest.Helpers;
using testtest.Models.Site;
using testtest.Models;
using testtestdbcontext.testtest.Models;

namespace testtest.Service
{
    public interface IAppUserService
    {
        bool checkbyuseremail(string email);
        bool checkbyusername(string username);
        bool checkPass(string id, string pass);
        Task<AppUser> CreateAdmin(AppUser AppUser, string password);
        Task<AppUser> CreateClient(AppUser AppUser, string password);
        Task<PagedList<AppUser>> Get(ResourceParameter parameter);
        Task<AppUser> Get(string id);
        Task<AppUser> GetByuserName(string UserName);
        Task<bool> Remove(string id, string Password);
        Task<AppUser> UpdatePassword(string id, string newpass);
        Task<UserState> getuserstat(string userName);
        Task<GuestUser> createGuest();
        Task<string> generateRandomUname();
       
    }
}