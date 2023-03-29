using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Ring.Play.Interface.Controllers
{
    [ApiController]
    [Route("api/Test")]
    [Authorize]
    public class TestController : ControllerBase
    {
        
        [HttpGet]
        [Route("List")]
        public string List()
        {
            return "ok";
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public string Login(string Username)
        {
            var claims = new Claim[] {
                                     new Claim(JwtRegisteredClaimNames.Iss,"test"),
                                     new Claim(JwtRegisteredClaimNames.Aud, "test"),
                                     // new Claim(ClaimTypes.Role, "Manager") 角色
                                   };

            SecurityToken securityToken = new JwtSecurityToken(
                        signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes("abc324effdsfre34reefds=")), SecurityAlgorithms.HmacSha256),
                        expires: DateTime.Now.AddDays(2),//过期时间
                        claims: claims
                    );
            //生成jwt令牌
            return new JwtSecurityTokenHandler().WriteToken(securityToken);

        }
    }
}
