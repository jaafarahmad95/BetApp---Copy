using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using testtest.DataAccess;
using testtest.Helpers;
using testtest.Models;
using testtest.Models.Deposit;

namespace testtest.Service
{
    public class DepositService : IDepositService
    {
        protected IMongoCollection<Deposit > _Deposit ;
        private readonly IMongoDbContext _context;
        protected IMongoCollection<Currency> _currency;
        protected IMongoCollection<AppUser> _appUser;
        protected IMongoCollection<DepositMethod> _Depositmeth;
        public DepositService(IMongoDbContext context)
        {
            _context = context;
            _Deposit  = _context.GetCollection<Deposit >(typeof(Deposit ).Name);
            _currency = _context.GetCollection<Currency>(typeof(Currency).Name);
            _appUser = _context.GetCollection<AppUser>(typeof(AppUser).Name);
            _Depositmeth = _context.GetCollection<DepositMethod>(typeof(DepositMethod).Name);
        }
        public async Task AddDeposit (Deposit  deposit )
        {
            var currency = await _currency.Find(x => x.CurrenceyId == deposit .CurrencyId).FirstOrDefaultAsync();
            var met = await _Depositmeth.Find(x => x.MethodId == deposit .MethodId).FirstOrDefaultAsync();
            deposit .CurrencyRate = currency.Value;
            deposit .Currency = currency;
            deposit.depositMethod = met;
            deposit.Date = DateTime.Now;
            await _Deposit .InsertOneAsync(deposit );

           
        }

        public async Task<bool> UserExists(string userId)
        {
            var user = await _appUser.Find(a => a.Id == userId).FirstOrDefaultAsync();
            if (user == null)
                return false;
            return true;
        }

        
        [Obsolete]
        public async Task<PagedList<Deposit>> GetDepositHistory(string UserId,ResourceParameter parameter)
        {

            
             var collectionPaging = _Deposit.Find(d => true)
            .SortBy(d => d.Date);
            if (!string.IsNullOrEmpty(parameter.SortingStatus))
            {
                if (parameter.SortingStatus.Equals("Descending"))
                {
                    collectionPaging = _Deposit.Find(AppUser => true)
                   .SortByDescending(d => d.Date);
                }
                if (parameter.SortingStatus.Equals("Ascending"))
                {
                    collectionPaging = _Deposit.Find(d => true).SortBy(x => x.Date);
                }
            }
            
            return   PagedList<Deposit>.Create(collectionPaging, parameter.PageNumber, parameter.PageSize);
            
        }
        public async Task<IEnumerable<DepositMethod>> GetDepositMethods()
        {
            return await _Depositmeth.Find(x=>true).ToListAsync();
        }

        public async Task<double> ConvertCurrency(Deposit deposit)
        {
            var currency = await _currency.Find(x => x.CurrenceyId == deposit.CurrencyId).FirstOrDefaultAsync();
            var rate = currency.Value;
            var converresult = deposit.Amount * (double)rate;
            return converresult;
        }

    }
}
