using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using testtest.dto;
using testtest.Service;

namespace testtest.Controllers
{
    public class AuthenticationController : ControllerBase
    {
        private IAuthenticationService _context;

        public AuthenticationController(IAuthenticationService context)
        {
            
            _context = context;
            
        }

        [HttpPost(ApiRoutes.Authentication.Login,Name = "Login")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticationReqest ar)
        {
            var staff = await _context.AuthenticateUser(ar);
            if (staff.Success == false)
                return BadRequest(staff.Errors);
            else
            return Ok(staff);
        }

        [HttpPost(ApiRoutes.Authentication.Refresh,Name = "Refresh")]
        public async Task<IActionResult> RefreshToken([FromBody]RefreshTokenRequest ar)
        {
            var staff = await _context.RefreshTokenAsync(ar.Token, ar.RefreshToken);
            if (staff.Success == false)
                return BadRequest(staff.Errors);
            else
           return Ok(staff);
        }
    }
}
