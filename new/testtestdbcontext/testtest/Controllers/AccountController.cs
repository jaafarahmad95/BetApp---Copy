using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using testtest.dto;
using testtest.dto.Site;
using testtest.Extensions;
using testtest.Helpers;
using testtest.Models.Site;
using testtest.Service;
using testtest.Service.General;
using testtest.Service.Site;
using testtestdbcontext.AppUserService;
using testtestdbcontext.AppUserService.Site;
using testtestdbcontext.testtest.dto.Withdrow;
using testtestdbcontext.testtest.Models;

namespace testtest.Controllers
{

   
   [Produces("application/json")]
   [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _AccountService;
        private readonly IMapper _mapper;
         private IpropertyMappingService _propertyMappingService;
         private LinkGenerator _urlHelper;
          private ITypeHelperService _typeHelperService;
          private IWithdrowService _withserv;
           private readonly IOfferService _OfferService;
        public AccountController(IOfferService offerservice,IAccountService accountService,IMapper mapper
        , LinkGenerator urlHelper , IpropertyMappingService propservice,
         ITypeHelperService typeHelperService,
         IWithdrowService withserv)
        {
            _AccountService = accountService;
            _mapper = mapper;
             _urlHelper = urlHelper;
            _propertyMappingService = propservice;
             _typeHelperService = typeHelperService;
             _withserv = withserv;
             _OfferService=offerservice;
        }

        [HttpGet(ApiRoutes.Accounts.GetAccount)]
        public async Task<IActionResult> Get(string username)
        {
            
            var Account = await _AccountService.GetAccount(username);

            if (Account == null)
            {
                return NotFound();
            }


            return Ok(Account);
        }

        [HttpGet(ApiRoutes.Accounts.GetBalancebyUsername)]

        public async Task<IActionResult> GetBalancebyUserName(string username)
          {
              var balance = await _AccountService.GetBalancebyUsername(username);
              return Ok(balance);
          }

        [HttpGet(ApiRoutes.Accounts.GetBalancebyid)]

        public async Task<IActionResult> GetBalancebyId(string id)
          {
              var balance = await _AccountService.GetBalancebyID(id);
              return Ok(balance);
          }

        [HttpGet(ApiRoutes.Accounts.GetBetHistory)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles ="Client")]
    
        public async Task<IActionResult> GetBetHistory([FromQuery]string Userid ,[FromQuery] ResourceParameter parameter)
        {
           
            if (!_typeHelperService.TypeHasProperties<UserListViewDto>(parameter.Fields))
                return BadRequest();
            var users = await  _AccountService.GetBetHistory(Userid,parameter);
            var prevLink = users.HasPrevious ?
                CreateUsersResourceUri(parameter, ResourceUriType.PreviousPage) : null;

            var nextLink = users.HasNext ?
                CreateUsersResourceUri(parameter, ResourceUriType.NextPage) : null;

            var paginationMetaData = new
            {
                totalCount = users.TotalCount,
                pageSize = users.PageSize,
                totalPages = users.TotalPages,
                currentPage = users.CurrentPage,
                PreviousPageLink = prevLink,
                NextPageLink = nextLink
            };

            Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetaData));
            Response.Headers.Add("Access-Control-Expose-Headers", "X-Pagination");
            var userMap =  _mapper.Map<IEnumerable<BetViewDto>>(users);
             return Ok(userMap.ShapeData(parameter.Fields));
        }


        [HttpPost(ApiRoutes.Accounts.Withdrow , Name = "Withdrow ")]
        [ProducesResponseType(StatusCodes.Status201Created)]
         [Authorize(Roles ="Client")]
  
        public async Task<IActionResult> CreateWithdrow( [FromBody] WithdrowreqDto withdrowreq)
        {
            if (withdrowreq == null)
                return BadRequest("check Your Withdrow Data");
            if (await _OfferService.CheckLaiaabilty(withdrowreq.Amount, withdrowreq.UserId) == false)
                return BadRequest("you don't have enough cash to place this bet");

            var withdrowentity = _mapper.Map<Withdrow>(withdrowreq);

            withdrowentity.Date = DateTime.Now;

            if (!ModelState.IsValid)
            {
                return new ModelValidator.UnprocessableEntityObjectResult(ModelState);
            }

            await _withserv.AddWithdrow(withdrowentity);
           
            await _AccountService.TakeFromBalance(withdrowentity.UserId, withdrowentity.Amount);
            
            var WithdrowMapToReturn = _mapper.Map<WithdrowViewDto>(withdrowentity);

            return Ok(WithdrowMapToReturn);
           
        }

        [HttpGet(ApiRoutes.Accounts.GetWithdrowHistory)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles ="Client")]
    
        public async Task<IActionResult> GetWithdrowHistory([FromQuery]string Userid ,[FromQuery] ResourceParameter parameter)
        {
           
            if (!_typeHelperService.TypeHasProperties<WithdrowViewDto>(parameter.Fields))
                return BadRequest();
            var withdrows = await  _withserv.GetWithdrowHistory(Userid,parameter);
            var prevLink = withdrows.HasPrevious ?
                CreateUsersResourceUri(parameter, ResourceUriType.PreviousPage) : null;

            var nextLink = withdrows.HasNext ?
                CreateUsersResourceUri(parameter, ResourceUriType.NextPage) : null;

            var paginationMetaData = new
            {
                totalCount = withdrows.TotalCount,
                pageSize = withdrows.PageSize,
                totalPages = withdrows.TotalPages,
                currentPage = withdrows.CurrentPage,
                PreviousPageLink = prevLink,
                NextPageLink = nextLink
            };

            Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetaData));
            Response.Headers.Add("Access-Control-Expose-Headers", "X-Pagination");
            var userMap =  _mapper.Map<IEnumerable<WithdrowViewDto>>(withdrows);
             return Ok(userMap.ShapeData(parameter.Fields));
        }



        private string CreateUsersResourceUri(
           ResourceParameter resourceParameters,
           ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _urlHelper.GetPathByAction(HttpContext,"GetUsers",values:
                      new
                      {
                          fields = resourceParameters.Fields,
                          orderBy = resourceParameters.OrderBy,
                          searchQuery = resourceParameters.SearchQuery,
                          pageNumber = resourceParameters.PageNumber - 1,
                          pageSize = resourceParameters.PageSize,
                         
                      });
                case ResourceUriType.NextPage:
                    return _urlHelper.GetPathByAction(HttpContext, "GetUsers", values:
                     new 
                     {
                         fields = resourceParameters.Fields,
                         orderBy = resourceParameters.OrderBy,
                         searchQuery = resourceParameters.SearchQuery,
                         pageNumber = resourceParameters.PageNumber + 1,
                         pageSize = resourceParameters.PageSize,

                     });
                        
                case ResourceUriType.Current:
                default:
                    return _urlHelper.GetPathByAction(HttpContext, "GetUsers", values:
                    new
                    {
                        fields = resourceParameters.Fields,
                        orderBy = resourceParameters.OrderBy,
                        searchQuery = resourceParameters.SearchQuery,
                        pageNumber = resourceParameters.PageNumber,
                        pageSize = resourceParameters.PageSize,
                        
                    });
            }
        }




    }
}
