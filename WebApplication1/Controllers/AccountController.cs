using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebApplication1.services;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _emailService = emailService;
        }

        [HttpPost("Register")]
        public async Task Register(string name, string email, string password)
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

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(User);

                    var getUserId = User.Id;

                    var callbackUrl = "http://localhost:51245/html/confirmation.html?UserId=" + User.Id + "&UserCode=" + code;

                    string subject = "localhost notification";
                    string message = "Hello, new member. You had been applied to Strateix project. Follow the link to confirm your data ";

                    try
                    {
                        await _emailService.SendEmailAsync(email, subject, message, callbackUrl);

                    }
                    catch (Exception exc)
                    {
                        var a = exc;
                    }

                    //IdentityMessage oumessage;
                    //emailService.SendAsync(oumessage);

                }
            }

        }
        
        [HttpGet("")]
        public JsonResult HandleUserData(string userid, string code)
        {
            return Json("");
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
