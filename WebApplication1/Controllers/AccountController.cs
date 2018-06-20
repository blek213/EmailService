using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost("Register")]
        public async Task<JsonResult> Register(string name, string email, string password)
        {
            IdentityUser User = new IdentityUser { Email = email, UserName = name };

            var roleUser = new IdentityRole { Name = "user" };

            await _roleManager.CreateAsync(roleUser);
   
            var result =  _userManager.CreateAsync(User, password);
      
            if (result != null)
            {
                var identity = GetIdentityRegister(name, password);
                if (identity == null)
                {
                    AddToRoleAsyncFunc(User, roleUser);
                    identity = SendClaimsInRegister(name);

                }
            }

            return Json(System.Net.HttpStatusCode.Accepted);
        }

        public void AddToRoleAsyncFunc(IdentityUser User, IdentityRole roleUser)
        {
            _userManager.AddToRoleAsync(User, roleUser.Name);
        }

        private ClaimsIdentity GetIdentityRegister(string username, string password)
        {
            List<IdentityUser> identityUsers = _userManager.Users.ToList();

            IdentityUser user = identityUsers.FirstOrDefault(p => p.UserName == username);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType,username),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType,"user")
                };

                ClaimsIdentity claimsIdentity =
              new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                  ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            return null;
        }

        public ClaimsIdentity SendClaimsInRegister(string username)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType,username),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType,"user")
                };

            ClaimsIdentity claimsIdentity =
          new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
              ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }

    }
}
