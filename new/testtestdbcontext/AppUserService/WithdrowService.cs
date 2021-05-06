using System.Threading.Tasks;
using MongoDB.Driver;
using testtest.DataAccess;
using testtest.Helpers;
using testtestdbcontext.AppUserService.Site;
using testtestdbcontext.testtest.Models;

namespace testtestdbcontext.AppUserService
{
    public class WithdrowService : IWithdrowService
    {   protected IMongoCollection<Withdrow> _withdrow ;
        private readonly IMongoDbContext _context;
        public WithdrowService(IMongoDbContext context)
        {
            _context = context;
            _withdrow = _context.GetCollection<Withdrow>(typeof(Withdrow).Name);
            
        }
        public async Task AddWithdrow(Withdrow withdrowentity)
        {
            await _withdrow.InsertOneAsync(withdrowentity);
        }

        [System.Obsolete]
        public async Task<PagedList<Withdrow>> GetWithdrowHistory(string UserID,ResourceParameter parameter )
        {
             var collectionPaging = _withdrow.Find(d => true)
            .SortBy(d => d.Date);
            if (!string.IsNullOrEmpty(parameter.SortingStatus))
            {
                if (parameter.SortingStatus.Equals("Descending"))
                {
                    collectionPaging = _withdrow.Find(AppUser => true)
                   .SortByDescending(d => d.Date);
                }
                if (parameter.SortingStatus.Equals("Ascending"))
                {
                    collectionPaging = _withdrow.Find(d => true).SortBy(x => x.Date);
                }
            }
            
            return   PagedList<Withdrow>.Create(collectionPaging, parameter.PageNumber, parameter.PageSize);
        }
    }
}