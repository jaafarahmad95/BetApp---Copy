using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using testtest.dto;
using testtest.Models;
using testtest.Service;
using Microsoft.AspNetCore.Authorization;
using testtest.Helpers;
using testtest.Extensions;
using testtest.Service.General;
using Microsoft.AspNetCore.Routing;
using testtestdbcontext.testtest.Models;

namespace testtest.Controllers
{
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    
    public class AppUsersController : ControllerBase
    {
        private readonly IAppUserService _AppUserService;
        private readonly IAuthenticationService _Auth;
        private IMapper _mapper;
        private LinkGenerator _urlHelper;
        private IpropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;

        public AppUsersController(IAppUserService AppUserService , IMapper mapper
            , LinkGenerator urlHelper , IpropertyMappingService propservice,
             ITypeHelperService typeHelperService, IAuthenticationService Auth)
        {
            _AppUserService = AppUserService;
            _mapper = mapper;
            _urlHelper = urlHelper;
            _propertyMappingService = propservice;
            _typeHelperService = typeHelperService;
            _Auth=Auth;
        }

        [HttpGet(ApiRoutes.SystemUser.GetUsers, Name = "GetUsers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Get([FromQuery] ResourceParameter parameter)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<UserListViewDto, AppUser>(parameter.OrderBy))
                return BadRequest();
            if (!_typeHelperService.TypeHasProperties<UserListViewDto>(parameter.Fields))
                return BadRequest();
            var users = await  _AppUserService.Get(parameter);

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

            var userMap =  _mapper.Map<IEnumerable<UserListViewDto>>(users);
             return Ok(userMap.ShapeData(parameter.Fields));
        }
            

        [HttpGet(ApiRoutes.SystemUser.GetUser, Name = "GetAppUser")]
        public async Task<IActionResult> Get(string id)
        {
          
            var AppUser = await _AppUserService.Get(id);

            if (AppUser == null)
            {
                return NotFound();
            }
            var user = _mapper.Map<UserListViewDto>(AppUser);

            return Ok(user);
        }

        [HttpGet(ApiRoutes.SystemUser.GetUserByuserName, Name = "GetByUserName")]
        public async Task<IActionResult> GetByuserName(string UserName)
        {
            
            var AppUser = await _AppUserService.GetByuserName(UserName);

            if (AppUser == null)
            {
                return NotFound();
            }
            var user = _mapper.Map<UserListViewDto>(AppUser);

            return Ok(user);
        }

        [HttpPost(ApiRoutes.SystemUser.CreateAdmin,Name = "CreateAdmin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAdmin([FromBody] RegisterAdmin model)
        {  
            if (model == null)
                return BadRequest();
             var user = _mapper.Map<AppUser>(model);
            user.Roles.Add("Admin");
            if (!ModelState.IsValid)
                 {
                     return new ModelValidator.UnprocessableEntityObjectResult(ModelState);
                 }
            if (!(_AppUserService.checkbyusername(model.UserName) ||
               _AppUserService.checkbyuseremail(model.Email)))
            {
              var creuser= await _AppUserService.CreateAdmin(user, model.Password.ToString());
                var result = _mapper.Map<UserResponse>(creuser);
                return CreatedAtRoute("GetAppUser", new { id = user.Id.ToString() }, result);
            }

           else return BadRequest("user name or email allready exist");
        }

        [HttpPost(ApiRoutes.SystemUser.CreateClient, Name ="CreateClient")]
        [AllowAnonymous]
         public async Task<IActionResult> CreateClient([FromBody] RegisterEntity model)
         {
               if (model == null)
                   return BadRequest("Some User Info are missing or not as requested");
               var user = _mapper.Map<AppUser>(model);
               user.Roles.Add("Client");
               if (!ModelState.IsValid)
               {
                   return new ModelValidator.UnprocessableEntityObjectResult(ModelState);
               }
               if ((_AppUserService.checkbyusername(model.UserName) ))
                 return BadRequest("user name  allready used");
               else if(_AppUserService.checkbyuseremail(model.Email))
                 return BadRequest("E-mail allready used");
               else
               {
                   var creuser = await _AppUserService.CreateClient(user, model.Password.ToString());
                   var result = _mapper.Map<UserResponse>(creuser);
                   return CreatedAtRoute("GetAppUser", new { id = user.Id.ToString() }, result);
               }

              
         }


        [HttpPut(ApiRoutes.SystemUser.UpdatePassword,Name = "UpdatePassword")]
        [Authorize(Roles ="Client")]
        public async Task<IActionResult> UpdatePassword(string id, [FromBody] UpdatePasswordDto model)
        {
            var AppUser = _AppUserService.Get(id);

            if (AppUser == null)
            {
                return NotFound();
            }
            if (model == null)
                return BadRequest();
            if (!_AppUserService.checkPass(id, model.OldPassword))
                return BadRequest("you don't have permisinn");
          
            else
            {
               

                if (!ModelState.IsValid)
                {
                    return new ModelValidator.UnprocessableEntityObjectResult(ModelState);
                }

                var upuser = await _AppUserService.UpdatePassword(id, model.NewPassword.ToString());

                
                return Ok("Password Updated");
            }
           
        }

        [HttpDelete(ApiRoutes.SystemUser.DeleteUser,Name ="DeleteUser")]
        public async Task<IActionResult> Delete(string id,string Password)
        {
            var AppUser = await _AppUserService.Get(id);

            if (AppUser == null)
            {
                return BadRequest("No User To Remove");
            }

        var check =  await  _AppUserService.Remove(AppUser.Id,Password);
            if (check == false)
                return BadRequest("Insert Correct Password");
            return Ok("user deleted");
        }
        
        [HttpGet(ApiRoutes.SystemUser.userstat,Name="UserStat")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> GetUserStat( string UserName)
        {
            var result =await _AppUserService.getuserstat(UserName) ;
            return Ok(result);
        }
        
        [HttpPost(ApiRoutes.SystemUser.Guest,Name="Guest")]
        [AllowAnonymous]
        public async Task<IActionResult> creatguestlog ()
        {
            var newguest = await _AppUserService.createGuest();
          
            var result = await _Auth.AuthenticateGuest(newguest);
            return Ok(result);

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
