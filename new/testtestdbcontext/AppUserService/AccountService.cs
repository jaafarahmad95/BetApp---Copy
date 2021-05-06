using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using testtest.DataAccess;
using testtest.helpers;
using testtest.Helpers;
using testtest.Models;
using testtest.Models.Site;

namespace testtest.Service
{
    public class AccountService  : IAccountService 
    {
        protected IMongoCollection<Account> _Account;
        private readonly IMongoDbContext _context;
        protected IMongoCollection<Bet> _bet;
       
        public AccountService(IMongoDbContext context)
        {
            _context = context;
            _Account = _context.GetCollection<Account>(typeof(Account).Name);
            _bet = _context.GetCollection<Bet>(typeof(Bet).Name);
           
        }

        public async Task<Account> CreateClientAccount(AppUser user, string id)
        {
            Account account = new Account();
            account.Id = id;
            account.Balance = 0.0;
            account.Bet_Slip_pinned = true;
            account.Commission_resreve = 0.0;
            account.Commission_Type = "volum";
            account.Country = user.Country;
            account.Currency = user.Currency;
            account.Exchange_type = "back-lay";
            account.Free_founds = 0.0;
            account.Language = "ENG";
            account.Odds_type = "Deciaml";
            account.Roles="Client";
            account.Show_bet_Confirm = true;
            account.Status = "Active";
            account.Username = user.UserName;
            account.birthdate = user.birthdate;
            account.City = user.City;
            account.PersonalID = user.PersonalID;

           await _Account.InsertOneAsync(account);
            return account;

        }

        public async Task<Account> CreateAdminAccount(AppUser user, string id)
        {
            Account account = new Account();
            account.Id = id;
            account.Balance = 0.0;
            account.Bet_Slip_pinned = false;
            account.Commission_resreve = 0.0;
            account.Commission_Type = "volum";
            account.Country = user.Country;
            account.Currency = "";
            account.Exchange_type = "";
            account.Free_founds = 0.0;
            account.Language = "ENG";
            account.Odds_type = "";
            account.Roles = "Admin";
            account.Show_bet_Confirm = false;
            account.Status = "Active";
            account.Username = user.UserName;
            account.birthdate = user.birthdate;
            account.City = user.City;
            account.PersonalID = user.PersonalID;

            await _Account.InsertOneAsync(account);
            return account;

        }

        public async Task<Account> GetAccount(string username)
        {
            var filter = Builders<Account>.Filter.Eq("Username", username);
            var acc = await _Account.Find(filter).FirstAsync();
           // var acc =  _Account.Find<Account>(x => x.Username == username).First();
            return acc;
        }

        public async Task<double> GetBalancebyID(string id)
        {
            // var filter = Builders<Account>.Filter.Eq("Id", id);
            // var acc = await _Account.Find(filter).FirstOrDefaultAsync();

            var acc = await _Account.Find<Account>(x => x.Id == id).FirstOrDefaultAsync();
            var balance = acc.Balance;
            return balance;

        }
        public async Task<double> GetBalancebyUsername(string username)
        {
            var filter = Builders<Account>.Filter.Eq("Username", username);
            var acc = await _Account.Find(filter).FirstAsync();
            //var acc = _Account.Find<Account>(x => x.Username == username).First();
            var balance = acc.Balance;
            return balance;

        }

        public async Task<bool> TakeFromBalance(string id, double laibelty)
        {
            var filter = Builders<Account>.Filter.Eq("Id", id);
            var acc = await _Account.Find(filter).FirstAsync();
           var x = acc.Balance - laibelty;
            var update = Builders<Account>.Update.Set("Balance", x);
            await _Account.UpdateOneAsync(filter, update);
            return true;

        }

        public async Task<bool> AddToBalance(string id, double winnings)
        {
            var filter = Builders<Account>.Filter.Eq("Id", id);
            var acc = await _Account.Find(filter).FirstAsync();
            var x = acc.Balance + (winnings-((winnings*acc.Commission_resreve)/100));
            var update = Builders<Account>.Update.Set("Balance", x);
            await _Account.UpdateOneAsync(filter, update);

            /// code to add website winning and user winnings to bank accounts 
            return true;

        }
        public async Task<bool> AddDepositToBalance(string id, double amount)
        {
            /// code to connect to bank service and transfer mony to website account 
            var filter = Builders<Account>.Filter.Eq("Id", id);
            var acc = await _Account.Find(filter).FirstAsync();
            var x = acc.Balance + amount;
            var update = Builders<Account>.Update.Set("Balance", x);
            await _Account.UpdateOneAsync(filter, update);
            return true;
        }
        public async Task DeleteAccount(string id)
        {
           
           await _Account.DeleteOneAsync(x => x.Id == id);
          
        }

        [Obsolete]
        public async Task<PagedList<Bet>> GetBetHistory(string Userid,ResourceParameter parameter)
        {
          
            var collectionPaging = _bet.Find(a => a.UserId == Userid)
            .SortBy(a => a.CreationDate);
            
            if (!string.IsNullOrEmpty(parameter.SearchQuery))
            {
              
                if (parameter.SearchQuery == "All"&&parameter.SortingStatus=="true")
                    collectionPaging = (IOrderedFindFluent<Bet, Bet>) _bet.Find(a => true).SortByDescending(x =>x.CreationDate);
                else if(parameter.SearchQuery == "All"&&parameter.SortingStatus=="false")
                  collectionPaging = _bet.Find(a => a.UserId == Userid)
                        .SortBy(a => a.CreationDate);
                else 
                    collectionPaging = (IOrderedFindFluent<Bet, Bet>) _bet.Find(a => a.Status == parameter.SearchQuery);
            }

            
            return   PagedList<Bet>.Create(collectionPaging, parameter.PageNumber, parameter.PageSize);
        }

        public async Task<string> GetusernamebyUserId(string id)
        {
            var f = Builders<Account>.Filter.Eq("Id",id);
            var username = await _Account.Distinct<string>("Username",f).SingleOrDefaultAsync();
            return username ;

        }
    }
}
