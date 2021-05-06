
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using testtest.DataAccess;
using testtest.dto;
using testtest.Models;
using testtestdbcontext.testtest.Models;

namespace testtest.Service
{
    public class CurrencyService : ICurrencyService
    {
        protected IMongoCollection<Currency> _Currency;
        private readonly IMongoDbContext _context;
        protected IMongoCollection<AppUser> _AppUser;
        protected IMongoCollection<CurrencyUpdate> _CurrencyUpdate;
        protected IMongoCollection<Account> _Account;
        public CurrencyService(IMongoDbContext context)
        {
            _context = context;
            _Currency = _context.GetCollection<Currency>(typeof(Currency).Name);
            _AppUser = _context.GetCollection<AppUser>(typeof(AppUser).Name);
            _CurrencyUpdate = _context.GetCollection<CurrencyUpdate>(typeof(CurrencyUpdate).Name);
            _Account = _context.GetCollection<Account>(typeof(Account).Name);
        }
        public async Task<IEnumerable<Currency>> GetCurrencies()
        {
            return await _Currency.Find(x => true).ToListAsync();
        }
        public async Task<Currency> GetCurrencyById(int id)
        {
            return await _Currency.Find(i => i.CurrenceyId == id).FirstOrDefaultAsync();
        }
        public async Task<string> GetDefaultCurrency(string userId)
        {
            var filter = Builders<AppUser>.Filter.Eq(x=>x.Id, userId);
            var curr = await _AppUser.Distinct<string>("Currency", filter).FirstOrDefaultAsync();
            return curr;

        }
        public async Task UpdateCurrencyList(IEnumerable<UpdateCurrencyDto> currencyList)
        {
            foreach (var item in currencyList)
            {
                var currentCurrency = await GetCurrencyById(item.CurrenceyId);
                currentCurrency.Value = item.value;
               
                UpdateCurrency(currentCurrency);
            }
        }
        public async Task AddCurrenyUpdateRecord(CurrencyUpdateList currencyUpdate)
        {
            CurrencyUpdate newUpdate = new CurrencyUpdate()
            {
                AppuserId = currencyUpdate.userId,

                UpdateDate = DateTimeOffset.UtcNow
            };
            foreach (var item in currencyUpdate.CurrencyList)
            {
                var currency = await GetCurrencyById(item.CurrenceyId);
                if (currency.Code == "USD")
                    newUpdate.USD = item.value;
                else if (currency.Code == "EURO")
                    newUpdate.EURO = item.value;
                else if (currency.Code == "STG")
                    newUpdate.STG = item.value;
            }
            await _CurrencyUpdate.InsertOneAsync(newUpdate);
        }
        public async void UpdateCurrency(Currency currency)
        {
            var filter = Builders<Currency>.Filter.Eq("CurrenceyId", currency.CurrenceyId);
            var update = Builders<Currency>.Update.Set("LastUpdate", currency.LastUpdate)
                                                  .Set("Value", currency.Value);
            await _Currency.UpdateOneAsync(filter, update);
            
           
        }
        public async void SetDefaultCurrency(int id, string userId)
        {
            var Newdefault = _Currency.Find(x => x.CurrenceyId == id).FirstOrDefault();
            var filter = Builders<AppUser>.Filter.Eq("Id", userId);
            var filter2 = Builders<Account>.Filter.Eq("Id", userId);
            var update = Builders<AppUser>.Update.Set("Currency", Newdefault.Name);
            var update2 = Builders<Account>.Update.Set("Currency", Newdefault.Name);
            await _AppUser.UpdateOneAsync(filter, update);
            await _Account.UpdateOneAsync(filter2, update2); 
        }

        public async Task<bool> IsUserExists(string userid)
        {
            var count = await _AppUser.Find(x => x.Id == userid).CountDocumentsAsync();
            if (count != 1)
                return false;
            return true;
        }




    }
}
