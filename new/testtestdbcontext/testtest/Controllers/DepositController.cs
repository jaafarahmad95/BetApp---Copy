using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using testtest.dto;
using testtest.dto.Payment;
using testtest.Models.Deposit;
using testtest.Service;
using testtest.Helpers;
using testtest.Service.General ;
using Microsoft.AspNetCore.Routing;
using testtest.Extensions;

namespace testtest.Controllers
{
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DepositController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDepositService _depositserv;
        private readonly IAccountService _accountService;
        private LinkGenerator _urlHelper;
        private IpropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;

        public DepositController(IMapper mapper, IDepositService depserv,IAccountService accountService,
          LinkGenerator urlHelper , IpropertyMappingService propservice, ITypeHelperService typeHelperService)
        {
            _mapper = mapper;
            _depositserv = depserv;
            _accountService = accountService;
            _urlHelper = urlHelper;
            _propertyMappingService = propservice;
            _typeHelperService = typeHelperService;
        }

        [HttpPost(ApiRoutes.Deposits.CreateDeposit , Name = "CreateDeposit ")]
        [ProducesResponseType(StatusCodes.Status201Created)]
         [Authorize(Roles ="Client")]
  
        public async Task<IActionResult> CreateDeposit( [FromBody] DepositCreationDto deposits)
        {
            if (deposits == null)
                return BadRequest();

            var depositEntity = _mapper.Map<Deposit>(deposits);

            depositEntity.Date = DateTime.Now;

            if (!ModelState.IsValid)
            {
                return new ModelValidator.UnprocessableEntityObjectResult(ModelState);
            }

            await _depositserv.AddDeposit (depositEntity);
            var x = _depositserv.ConvertCurrency(depositEntity);
            await _accountService.AddDepositToBalance(deposits.UserId, x.Result);
            
            var DepositMapToReturn = _mapper.Map<DepositViewDto>(depositEntity);

            return Ok(DepositMapToReturn);
           
        }

        [HttpGet(ApiRoutes.Deposits.GetDepositMethods, Name = "GetDeposit Methods")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaymentMethods()
        {
            var depositMethods = await _depositserv.GetDepositMethods();

            if (depositMethods == null)
                return NotFound();

            var ModelToReturn = _mapper.Map<IEnumerable<DepositMethodsDto>>(depositMethods);
            return Ok(ModelToReturn.ToList());
        }

        [HttpGet(ApiRoutes.Deposits.GetDeposits, Name = "GetDeposits")]
         [Authorize(Roles ="Client")]
         public async Task<IActionResult> Get([FromQuery]string userid,[FromQuery] ResourceParameter parameter)
        {
            
            if (!_typeHelperService.TypeHasProperties<DepositViewDto>(parameter.Fields))
                return BadRequest();
            var users = await  _depositserv.GetDepositHistory(userid,parameter);
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
            var userMap =  _mapper.Map<IEnumerable<DepositViewDto>>(users);
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
