using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplicanJWTapi.Models;

namespace WebApplicanJWTapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : Controller
    {
        private IConfiguration _config;

        public IdentityController(IConfiguration config)
        {
            _config = config;
        }

        //private UserLogin AutenticationUser(UserLogin user)
        //{
        //    UserLogin userLogin = null;

        //    if (user.Username.ToLower() == "mrmy")
        //    {
        //        userLogin= new UserLogin { Username = user.Username ,Password=user.Password};
        //    }
        //    return userLogin;

        //}

        private bool AutenticationUser(UserLogin user)
        {
            if (user.Username=="admin" && user.Password=="admin") { 
             return true;
            }
            return false;
        }
        private string GenericToken(UserLogin userLogin)
        {
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"] ?? "Test"));
            var credentials=new SigningCredentials(securitykey,SecurityAlgorithms.HmacSha256);

            //var claims = new[]
            //{
            //    new Claim(JwtRegisteredClaimNames.Sub,userLogin.Username),

            //};
            var cliam = new Claim[] { 
            new Claim("Id","1"),
            new Claim("FirstName","Mohammad"),
            new Claim("LastName","Yousefi")

            };

            var token = new JwtSecurityToken(
                _config["JWT:Issuer"],
                _config["JET:Audience"],
                cliam,
                expires:DateTime.Now.AddMinutes(15),//important
                signingCredentials:credentials

                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(UserLogin userLogin)
        {
            if (userLogin == null ||! AutenticationUser(userLogin)) {
                return Unauthorized();

            }

            var token= GenericToken(userLogin);

            return Ok(new {Token=token});
        }
    }
}
