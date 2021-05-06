using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.JsonPatch;
using testtest.Service;
using testtest.dto;

namespace testtest.Controllers
{
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CurrencyController : Controller
    {
        private ICurrencyService _context;
        private IMapper _mapper;
        public CurrencyController(ICurrencyService context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Get List of Currencies
        /// </summary>
        /// <returns>return List of Currencies</returns>
        [HttpGet(ApiRoutes.Currency.GetCurrencies, Name = "GetCurrencies")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCurrencies()
        {
            var currencies = await _context.GetCurrencies();
            var CurrencyMap = _mapper.Map<IEnumerable<CurrencyDto>>(currencies);
            return Ok(CurrencyMap);
        }

        /// <summary>
        /// Get Currency By Id
        /// </summary>
        /// <param name="currencyId"></param>
        /// <returns>return Author</returns>
        [HttpGet(ApiRoutes.Currency.GetCurrency, Name = "GetCurrency")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCurrency(int currencyId)
        {
            var currency = await _context.GetCurrencyById(currencyId);

            if (currency == null)
                return NotFound();

            var CurrencyMap = _mapper.Map<CurrencyDto>(currency);
            return Ok(CurrencyMap);
        }

        /// <summary>
        /// Get Default Currency
        /// </summary>
        /// <returns>Return Default Currency</returns>
        [HttpGet(ApiRoutes.Currency.GetDefaultCurrency, Name = "GetDefaultCurrency")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDefaultCurrency([FromQuery]string id)
        {
            if (! await _context.IsUserExists(id))
                return BadRequest("user not exists");

            var currency = await  _context.GetDefaultCurrency(id);

            if (currency == null)
                return BadRequest(" not exists");

         

            return Ok(currency);
        }

        /// <summary>
        /// Set Default Currency
        /// </summary>
        /// <returns>return 200 Ok </returns>
        [HttpPost(ApiRoutes.Currency.SetDefaultCurrency, Name ="SetDefaultCurrency")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        
        public async Task<IActionResult> SetDefaultCurrency(int currencyId, [FromQuery]string userid)
        {
            if (!await _context.IsUserExists(userid))
                return NotFound();

            _context.SetDefaultCurrency(currencyId, userid);
            
            return Ok("Defualt Currency Updated");
        }

        /// <summary>
        /// Update Currency with Put Method
        /// </summary>
        /// <param name="currencyUpdate">currency update parameter</param>
        /// <returns></returns>
        [HttpPut(ApiRoutes.Currency.UpdateCurrencyPut, Name = "UpdateCurrencyPut")]
      
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCurrencyPut([FromBody]CurrencyUpdateList currencyUpdate)
        {
            if (currencyUpdate == null)
                return BadRequest();

            await _context.UpdateCurrencyList(currencyUpdate.CurrencyList);

           
            await _context.AddCurrenyUpdateRecord(currencyUpdate);

           
            return Ok();
        }
    }
}
