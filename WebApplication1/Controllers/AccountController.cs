using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NETCore.Encrypt.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Security.Claims;
using WebApplication1.Entity;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IConfiguration _configuration;

        public AccountController(DatabaseContext databaseContext, IConfiguration configuration)
        {
            _databaseContext = databaseContext;
            _configuration = configuration;
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                string hashedPassword = DoMD5HashedString(model.Password);

                User user = _databaseContext.Users.SingleOrDefault(x => x.Username.ToLower()
                    == model.Username.ToLower() && x.Password == hashedPassword);


                if (user != null)
                {
                    if (user.Locked)
                    {
                        ModelState.AddModelError(nameof(model.Username), "Kullanıcı kilitlenmiş");
                        return View(model);
                    }

                    List<Claim> claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.FullName ?? string.Empty),
                        new Claim(ClaimTypes.Role,user.Role),
                        new Claim("Username", user.Username)
                    };

                    ClaimsIdentity identity = new ClaimsIdentity(claims,
                            CookieAuthenticationDefaults.AuthenticationScheme);

                    ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    return RedirectToAction("Index", "Home");


                }
                else
                {
                    ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı");
                }


            }
            return View(model);
        }

        private string DoMD5HashedString(string k)
        {
            string md5Salt = _configuration.GetValue<string>("AppSettings:MD5Salt");
            string salted = k + md5Salt;
            string hashed = salted.MD5();
            return hashed;
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (_databaseContext.Users.Any(x => x.Username.ToLower() == model.Username.ToLower()))
                {
                    ModelState.AddModelError(nameof(model.Username), "Kullanıcı Adı alımış");
                    View(model);
                }

                string hashedPassword = DoMD5HashedString(model.Password);

                User user = new()
                {
                    Username = model.Username,
                    Password= hashedPassword
                };

                _databaseContext.Users.Add(user);
                int affectedRowCount = _databaseContext.SaveChanges();
                if (affectedRowCount==0)
                {
                    ModelState.AddModelError("", "Kullanıcı eklenemedi");
                }
                else
                {
                    return RedirectToAction(nameof(Login));
                }

            }
            return View(model);

        }


        public IActionResult Profile()
        {
            ProfileInfoLoader();

            return View();
        }

        private void ProfileInfoLoader()
        {
            Guid userid = new(User.FindFirstValue(ClaimTypes.NameIdentifier));
            User user = _databaseContext.Users.SingleOrDefault(x => x.Id == userid);

            ViewData["Fullname"] = user.FullName;
        }

        [HttpPost]
        public IActionResult ProfileChangeFullname( [Required][StringLength(50)] string? fullname)
        {

            if (ModelState.IsValid) {
                Guid userid = new(User.FindFirstValue(ClaimTypes.NameIdentifier));
                
                User user=_databaseContext.Users.SingleOrDefault(x => x.Id == userid);
               
                user.FullName = fullname;
                _databaseContext.SaveChanges();

                return RedirectToAction(nameof(Profile));
               
            }
            ProfileInfoLoader();

            return View("Profile");
        }

        [HttpPost]
        public IActionResult ProfileChangePassword([Required][MinLength(6)][MaxLength(16)] string? password)
        {

            if (ModelState.IsValid)
            {
                Guid userid = new(User.FindFirstValue(ClaimTypes.NameIdentifier));

                User user = _databaseContext.Users.SingleOrDefault(x => x.Id == userid);


                string hashedPassword = DoMD5HashedString(password);

                user.Password = hashedPassword;
                _databaseContext.SaveChanges();

                ViewData["result"] = "PasswordChanged";

            }
            ProfileInfoLoader();
            return View("Profile");
        }



        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
            
        }

    }
}
