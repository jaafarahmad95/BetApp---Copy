using System.Threading.Tasks;
using testtest.Helpers;
using testtestdbcontext.testtest.Models;

namespace testtestdbcontext.AppUserService
{
    public interface IWithdrowService
    {

        Task AddWithdrow(Withdrow withdrowentity);
        Task<PagedList<Withdrow>>  GetWithdrowHistory (string UserID,ResourceParameter parameter);


    }
}