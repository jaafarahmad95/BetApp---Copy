using System;
using System.Collections.Generic;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using testtest.dto;
using testtest.Models;
using MongoDB.Driver;
using testtest.helpers;
using Microsoft.IdentityModel.Tokens;
using AspNetCore.Identity.Mongo;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using testtest.DataAccess;
using testtestdbcontext.testtest.dto;
using testtestdbcontext.testtest.Models;

namespace testtest.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        protected IMongoCollection<AppUser> _AppUser;
         protected IMongoCollection<GuestUser> _gU;
        protected IMongoCollection<RefreshToken> _refresh;
        private readonly IMongoDbContext _context;
        private readonly JwtSettings _jwtSettings;
        private TokenValidationParameters _tokenValidationParameters;

        public AuthenticationService(JwtSettings jwtSettings, IMongoDbContext context
            , TokenValidationParameters tokenValidationParameters)

        {
            _context = context;
            _AppUser = _context.GetCollection<AppUser>(typeof(AppUser).Name);
            _refresh = _context.GetCollection<RefreshToken>(typeof(RefreshToken).Name);
            _jwtSettings = jwtSettings;
            _tokenValidationParameters = tokenValidationParameters;
            _gU= context.GetCollection<GuestUser>(typeof(GuestUser).Name);

        }
        public async Task<AuthenticationResult> AuthenticateUser(AuthenticationReqest ar)
        {
            var user = _AppUser.Find<AppUser>(AppUser => AppUser.UserName == ar.Username).FirstOrDefault();

            if (user == null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "User With this username does not exist " }
                };
            }
         
            var creater = new Passhash();
            var Pass = creater.gethash(ar.Password);
            bool result = false ;
            if(Pass == user.PasswordHash)
                result= true ;
            // var f1 = Builders<AppUser>.Filter.Eq("PasswordHash",Pass);
            // var f2 = Builders<AppUser>.Filter.Eq("UserName",ar.Username);
            // var result = _AppUser.Find<AppUser>(AppUser => AppUser.UserName == Pass).FirstOrDefault();

            AuthenticationResult response = new AuthenticationResult();

            if (result == false)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "Username/password Combination are wrong" }
                };
            }

            return await GenerateAuthenticationResultForUserAsync(user);
        }

        private async Task<AuthenticationResult> GenerateAuthenticationResultForUserAsync(AppUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            var userR = _AppUser.Find<AppUser>(x => x.Id == user.Id).FirstOrDefault();
            var userrole = userR.Roles;
           
            var claimlist = new List<Claim> {
                 new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                 new Claim("id", userR.Id),
                 new Claim("name", userR.UserName),
                 new Claim("role", userrole.FirstOrDefault())
                
            };
            
            var claims = new List<IdentityUserClaim>();
            for (int i = 0; i < claimlist.Count; i++)
            {
                claims.Add(new IdentityUserClaim(claimlist[i]));
            }



            foreach (var userRole in userrole)
            {
                Claim claim = new Claim(ClaimTypes.Role, userRole);
                claims.Add(new IdentityUserClaim(claim));
            }



            var filter = Builders<AppUser>.Filter.Eq("Id", userR.Id);
            var userClaims = _AppUser.Find<AppUser>(filter).FirstOrDefault();
            var CurrentClaims = userClaims.Claims;
            CurrentClaims.AddRange(claims);
           
            var update = Builders<AppUser>.Update.Set("Claims", CurrentClaims);
          
            await _AppUser.UpdateOneAsync(filter, update);


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claimlist),
                Expires = DateTime.Now.Add(_jwtSettings.TokenLifetime),
                SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var creater = new Passhash();
            
            var refreshToken = new RefreshToken
            {
                Token= creater.gethash(token.Id),
                JwtId = token.Id,
                AppUserID = user.Id,
                Created = DateTime.Now,
                Expires = DateTime.Now.AddMonths(6)
            };
            await _refresh.InsertOneAsync(refreshToken);
            

            return new AuthenticationResult
            {   Username = user.UserName,
                Success = true,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token,
               
            };
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                   jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                       StringComparison.InvariantCultureIgnoreCase);
        }
        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var tokenValidationParameters = _tokenValidationParameters.Clone();
                tokenValidationParameters.ValidateLifetime = false;
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken)
        {
            var validatedToken = GetPrincipalFromToken(token);

            if (validatedToken == null)
            {
                return new AuthenticationResult { Errors = new[] { "Invalid Token" } };
            }

            var expiryDateUnix =
                long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);

            if (expiryDateTimeUtc > DateTime.Now)
            {
                return new AuthenticationResult { Errors = new[] { "This token hasn't expired yet" } };
            }

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            var storedRefreshToken = _refresh.Find<RefreshToken>(x => x.Token == refreshToken).First();

            if (storedRefreshToken == null)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token does not exist" } };
            }

            if (DateTime.Now > storedRefreshToken.Expires)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token has expired" } };
            }

            if (storedRefreshToken.IsExpired)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token has been invalidated" } };
            }

            if (storedRefreshToken.Used)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token has been used" } };
            }

            if (storedRefreshToken.JwtId != jti)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token does not match this JWT" } };
            }

            storedRefreshToken.Used = true;
            var filter = Builders<RefreshToken>.Filter.Eq("Id", storedRefreshToken.Id);
            var update = Builders<RefreshToken>.Update.Set("Used", true);

            await _refresh.UpdateOneAsync(filter, update);
            
            var user = _AppUser.Find<AppUser>(validatedToken.Claims.Single(x => x.Type == "id").Value).First();
           
            return await GenerateAuthenticationResultForUserAsync(user);
        }

        private async Task<bool> AddClaims(string UserId)
        {
            var userR = _AppUser.Find<AppUser>(x => x.Id == UserId).FirstOrDefault();
          
            Claim x = new Claim("edituser.view", "true");
            IdentityUserClaim z = new IdentityUserClaim(x);
            userR.Claims.Add(z);
            var filter = Builders<AppUser>.Filter.Eq("Id", UserId);
            var update = Builders<AppUser>.Update.Set("Claims", z);
            await _AppUser.UpdateOneAsync(filter, update);
           
            return true;
        }

        public async Task<AuthenticationResult> AuthenticateGuest(GuestUser ar)
        {
             AuthenticationResult response = new AuthenticationResult();
             return await GenerateAuthenticationResultForGuestAsync(ar);
        }
        private async Task<AuthenticationResult> GenerateAuthenticationResultForGuestAsync(GuestUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            var userR = _gU.Find<GuestUser>(x => x.UserName == user.UserName).FirstOrDefault();
            var userrole = userR.Roles;

            var claimlist = new List<Claim> {
                 new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                 new Claim("id", userR.Id),
                 new Claim("name", userR.UserName),
                 new Claim("role", userrole.FirstOrDefault())
                
            };
            
            var claims = new List<IdentityUserClaim>();
            for (int i = 0; i < claimlist.Count; i++)
            {
                claims.Add(new IdentityUserClaim(claimlist[i]));
            }



            foreach (var userRole in userrole)
            {
                Claim claim = new Claim(ClaimTypes.Role, userRole);
                claims.Add(new IdentityUserClaim(claim));
            }



            var filter = Builders<GuestUser>.Filter.Eq("Id", userR.Id);
            var userClaims = _gU.Find<GuestUser>(filter).FirstOrDefault();
            var CurrentClaims = userClaims.Claims;
            CurrentClaims.AddRange(claims);
           
            var update = Builders<GuestUser>.Update.Set("Claims", CurrentClaims);
          
            await _gU.UpdateOneAsync(filter, update);


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claimlist),
                Expires = DateTime.Now.Add(_jwtSettings.TokenLifetime),
                SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new AuthenticationResult
            {   Username = user.UserName,
                Success = true,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = "",
               
            };
            

        }
    }
}
