using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using testtest.dto;
using testtest.Models;
using testtest.Models.DataBaseSetting;
using AspNetCore.Identity.Mongo;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using testtest.helpers;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using testtest.DataAccess;
using testtest.Helpers;
using testtest.Models.Site;
using testtest.Models.Deposit;
using testtestdbcontext.testtest.Models;

namespace testtest.Service
{
    public class AppUserService : IAppUserService
    {
        protected IMongoCollection<AppUser> _AppUser;
        private readonly IMongoDbContext _context;
        private IAccountService _accserv;
        protected IMongoCollection<Account> _acc;
        protected IMongoCollection<Bet> _bet;
         protected IMongoCollection<GuestUser> _gU;
        protected IMongoCollection<Deposit> _dep;
       

        public AppUserService(IMongoDbContext context, IAccountService accserv)
        {
            _context = context;
            _AppUser = _context.GetCollection<AppUser>(typeof(AppUser).Name);
            _accserv = accserv;
            _acc = _context.GetCollection<Account>(typeof(Account).Name);
            _dep = _context.GetCollection<Deposit>(typeof(Deposit).Name);
            _bet = _context.GetCollection<Bet>(typeof(Bet).Name);
             _gU = _context.GetCollection<GuestUser>(typeof(GuestUser).Name);
           
        }

        [Obsolete]
        public async Task<PagedList<AppUser>> Get(ResourceParameter parameter)
        {
            var collectionPaging = _AppUser.Find(AppUser => true)
            .SortBy(AppUser => AppUser.Name);
            if (!string.IsNullOrEmpty(parameter.SortingStatus))
            {
                if (parameter.SortingStatus.Equals("Descending"))
                {
                    collectionPaging = _AppUser.Find(AppUser => true)
                   .SortByDescending(AppUser => AppUser.Name);
                }
                if (parameter.SortingStatus.Equals("Ascending"))
                {
                    collectionPaging = _AppUser.Find(AppUser => true).SortBy(x => x.Name);
                }
            }
            if (!string.IsNullOrEmpty(parameter.SearchQuery))
            {
                string searchQueryForWhereClause = parameter.SearchQuery.Trim().ToLowerInvariant();
               
            collectionPaging = (IOrderedFindFluent<AppUser, AppUser>) _AppUser.Find(a => a.Name.Contains(searchQueryForWhereClause)|| a.UserName.Contains(searchQueryForWhereClause));
            }
            
            return   PagedList<AppUser>.Create(collectionPaging, parameter.PageNumber, parameter.PageSize);

            
        }



        public async Task<AppUser> Get(string id)
        {
           var v =await _AppUser.Find<AppUser>(AppUser => AppUser.Id == id).FirstOrDefaultAsync();
            return v;
        }
        public bool checkbyusername(string username)
        {
            var appuser = _AppUser.Find<AppUser>(AppUser => AppUser.UserName == username).FirstOrDefault();
            if (appuser == null)
                return false;
            else return true;
        }
        public bool checkbyuseremail(string email)
        {
            var appuser = _AppUser.Find<AppUser>(AppUser => AppUser.Email == email).FirstOrDefault();
            if (appuser == null)
                return false;
            else return true;
        }
        public async Task<AppUser> CreateAdmin(AppUser AppUser, string password)
        {

            var creater = new Passhash();
            AppUser.PasswordHash = creater.gethash(password);
            await _AppUser.InsertOneAsync(AppUser);
        
            return AppUser;
        }

        public async Task<AppUser> CreateClient(AppUser AppUser, string password)
        {

            var creater = new Passhash();
            AppUser.PasswordHash = creater.gethash(password);
            _AppUser.InsertOne(AppUser);
            var ussr = _AppUser.Find<AppUser>(x => x.UserName == AppUser.UserName).FirstOrDefault();
            var id = ussr.Id;
            await _accserv.CreateClientAccount(AppUser, id);
            return AppUser;
        }

        public async Task<AppUser> UpdatePassword(string id, string newpass)
        {

            var creater = new Passhash();
            var hashedpass = creater.gethash(newpass);

            var filter = Builders<AppUser>.Filter.Eq("Id", id);
            var update = Builders<AppUser>.Update.Set("PasswordHash", hashedpass);
            await _AppUser.UpdateOneAsync(filter, update);

            var result = await Get(id);
            return result;

        }



        public async Task<bool> Remove(string id,string password)
        {
            var check = checkPass(id, password);
            if (check == false)
            {
                return false;
            }
            await _AppUser.DeleteOneAsync(AppUser => AppUser.Id == id);
            await _accserv.DeleteAccount(id);
            return true;
        }
            

        public bool checkPass(string id, string pass)
        {
            var appuser = _AppUser.Find<AppUser>(AppUser => AppUser.Id == id).FirstOrDefault();
            var creater = new Passhash();
            var z = creater.gethash(pass);
            if (z == appuser.PasswordHash)
                return true;
            else return false;
        }

        public async Task<AppUser> GetByuserName(string UserName)
        {
            var x =await _AppUser.Find(x => x.UserName == UserName).FirstOrDefaultAsync();
            return x;
        }
        public async Task<UserState> getuserstat(string userName)
        {
           UserState result = new UserState();
           var userpro = await _AppUser.Find(x => x.UserName == userName).FirstOrDefaultAsync();
           result.UserName= userpro.UserName;
           result.Country=userpro.Country;
           result.birthdate=userpro.birthdate;
           result.Currency=userpro.Currency;
           var acc = await _acc.Find(x => x.Username == userName).FirstOrDefaultAsync();
           result.CurrentBalance = acc.Balance;
           var dep = await _dep.Find(x => x.UserId == userpro.Id).SortByDescending(X => X.Date).ToListAsync();
           result.LatestDeposit=dep[0];
           var dep2 = await _dep.Find(x => x.UserId == userpro.Id).SortByDescending(X => X.Amount).ToListAsync();
           result.LatestDeposit=dep2[0];
            var userbets = await _bet.Find(x => x.UserId == userpro.Id).SortByDescending(X => X.InStake).ToListAsync();
            result.numberofPlacedBets = userbets.Count;
            var userWbets = await _bet.Find(x => x.UserId == userpro.Id && x.Status == "Won").ToListAsync();
            result.NoOfWonBet=userWbets.Count;
            var stakeSum =0.0;
            foreach (var item in userbets)
            {
              stakeSum=  stakeSum +item.InStake ;  
            }
            result.AvgStake = stakeSum/result.numberofPlacedBets;
            result.highistStake = userbets[0].InStake;
            

           return result;
           
        }


       public async Task<GuestUser> createGuest()
       {
           
           string rv = await generateRandomUname();
            
            var ex = await  _gU.Find(x => x.UserName == rv).ToListAsync();
            if (ex.Count !=0)
            {
                while (ex.Count != 0)
                {
                    rv = await generateRandomUname();
                    ex = await _gU.Find(x => x.UserName == rv).ToListAsync();
                }
            }
            
            GuestUser newguest = new GuestUser()
            {
                UserName=rv,
               
            };
            newguest.Roles.Add("guest");
            await _gU.InsertOneAsync(newguest);
            return newguest ;

       }

       public async Task<string> generateRandomUname()
        {
            string rv = "";
 
            char[] lowers = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'j', 'k', 'm', 'n', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            char[] uppers = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            char[] numbers = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
 
            int l = lowers.Length;
            int u = uppers.Length;
            int n = numbers.Length;
 
            Random random = new Random();
 
            rv += lowers[random.Next(0, l)].ToString();
            rv += lowers[random.Next(0, l)].ToString();
            rv += lowers[random.Next(0, l)].ToString();
 
            rv += uppers[random.Next(0, u)].ToString();
            rv += uppers[random.Next(0, u)].ToString();
            rv += uppers[random.Next(0, u)].ToString();
 
            rv += numbers[random.Next(0, n)].ToString();
            rv += numbers[random.Next(0, n)].ToString();
            rv += numbers[random.Next(0, n)].ToString();
 
            return rv;

        }
    

       
    }
}

