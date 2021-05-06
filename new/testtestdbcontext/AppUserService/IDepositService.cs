using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using testtest.Helpers;
using testtest.Models.Deposit;

namespace testtest.Service
{
   public interface IDepositService
    {
        Task AddDeposit (Deposit  deposit );
        Task<bool> UserExists(string userId);
        Task<PagedList<Deposit>> GetDepositHistory(string userId, ResourceParameter parameter);
        Task<IEnumerable<DepositMethod>> GetDepositMethods();
        Task<double> ConvertCurrency(Deposit deposit);

    }
}
