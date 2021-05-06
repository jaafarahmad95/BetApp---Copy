using System.Collections.Generic;
using System.Threading.Tasks;
using testtest.dto;
using testtest.Models;

namespace testtest.Service
{
    public interface ICurrencyService
    {
        Task AddCurrenyUpdateRecord(CurrencyUpdateList currencyUpdate);
        Task<IEnumerable<Currency>> GetCurrencies();
        Task<Currency> GetCurrencyById(int id);
        Task<string> GetDefaultCurrency(string userId);
        void SetDefaultCurrency(int id, string userId);
        void UpdateCurrency(Currency currency);
        Task UpdateCurrencyList(IEnumerable<UpdateCurrencyDto> currencyList);
        Task<bool> IsUserExists(string userid);
    }
}