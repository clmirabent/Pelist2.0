using System.Security.Claims;
using Films.Context;
using Films.Models;
using Films.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Films.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace Films.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly FilmsDbContext _context;

        private readonly ICloudinaryService _cloudinaryService;

        public AuthenticationController(FilmsDbContext context, ICloudinaryService cloudinaryService)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
        }
        

        public IActionResult Login()
        {
            if (TempData["SignUpSuccess"] != null)
            {
                ViewBag.SignUpSuccess = TempData["SignUpSuccess"];
            }
            if (TempData["LoginError"] != null)
            {
                ViewBag.LoginError = TempData["LoginError"];
            }
            return View();
        }
        
        public IActionResult SignUP()
        {
            if (TempData["SignUpError"] != null)
            {
                ViewBag.SignUpError = TempData["SignUpError"];
            }
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
          
            if (ModelState.IsValid)
            {
                // verify if user exists
                
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user == null)
                {
                    TempData["LoginError"] = "Este email no existe. Debes crear una cuenta.";
                    return RedirectToAction("Login");
                }

                // verify is password is correct
                bool isValid = PasswordHelper.VerifyPasswordHash(model.Password, user.PasswordHash, user.PasswordSalt);

                if (!isValid)
                {
                    TempData["LoginError"] =  "Contraseña incorrecta.";
                    return RedirectToAction("Login");
                }
                
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("UserId", user.IdUser.ToString()),
                    new Claim("ImageUrl", user.Image)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

      
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUP(SignUpViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                {
                    // Save the error in TempData and redirect to the same page
                    TempData["SignUpError"] = "Este email ya está registrado.";
                    return RedirectToAction("SignUP");
                }

                PasswordHelper.CreatePasswordHash(model.Password, out byte[] passwordHash, out byte[] passwordSalt);

                // Upload image to Cloudinary
                string? imageUrl;
                if (model.ProfileImage != null)
                {
                    imageUrl = await _cloudinaryService.UploadImageAsync(model.ProfileImage);
                }
                else
                {
                    imageUrl =
                        "https://res.cloudinary.com/duc5qq3mn/image/upload/v1744890605/profile-icon-design-free-vector_m86jfn.jpg";
                } 

                var user = new User
                {
                    Username = model.UserName,
                    Email = model.Email,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    Image = imageUrl,
                    AboutMe = model.AboutMe,
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync(); 
       
                TempData["SignUpSuccess"] = "¡Tu cuenta fue creada con éxito!";
                return RedirectToAction("Login");
            }

            return View(model);
        }

    }
    }
