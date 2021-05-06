using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using testtest.Helpers;
using testtest.Models;
using testtest.Models.Site;

namespace testtest.Service
{
    public interface IAccountService
    {
        Task<Account> CreateClientAccount(AppUser user, string id);

        Task<Account> CreateAdminAccount(AppUser user, string id);
        Task<Account> GetAccount(string username);
        Task<double> GetBalancebyID(string id);

        Task<double> GetBalancebyUsername(string username);

        Task<bool> TakeFromBalance(string id, double laibelty);

        Task<bool> AddToBalance(string id, double winnings);
        Task<bool> AddDepositToBalance(string id, double amount);

        Task DeleteAccount(string id);

        Task<PagedList<Bet>> GetBetHistory(string Userid,ResourceParameter parameter);
        Task<string> GetusernamebyUserId(string Id);

    }
}