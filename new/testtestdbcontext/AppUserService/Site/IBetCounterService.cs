using System.Threading.Tasks;

namespace testtestdbcontext.AppUserService.Site
{
    public interface IBetCounterService
    {
        Task<bool>  addtoCounter(string EventId);
        Task<bool>  takefromCounter(string EventId);
    }
}