using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MvcCubosExamenDDB.Models;
using MvcCubosExamenDDB.Services;
using System.Security.Claims;

namespace MvcCubosExamenDDB.Controllers {
    public class ManagedController : Controller {

        private ServiceCubos service;

        public ManagedController(ServiceCubos service) {
            this.service = service;
        }

        public IActionResult LogIn() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(string name, string password) {
            string? token = await this.service.GetToken(name, password);

            if (token == null) {
                ViewData["MENSAJE"] = "Usuario/Password incorrectos";
                return View();
            } else {
                HttpContext.Session.SetString("TOKEN", token);

                Usuario user = await this.service.GetPerfil();

                ClaimsIdentity identity = new ClaimsIdentity(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    ClaimTypes.Name,
                    ClaimTypes.Role
                );

                Claim claimId = new Claim(ClaimTypes.NameIdentifier, user.IdUsuario.ToString());
                Claim claimName = new Claim(ClaimTypes.Name, name);
                Claim claimRole = new Claim(ClaimTypes.Role, "USER");
                Claim claimImage = new Claim("IMAGEN", user.Imagen);
                Claim claimEmail = new Claim("Email", user.Email);

                identity.AddClaim(claimName);
                identity.AddClaim(claimId);
                identity.AddClaim(claimRole);
                identity.AddClaim(claimImage);
                identity.AddClaim(claimEmail);


                ClaimsPrincipal userPrincipal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    userPrincipal, new AuthenticationProperties {
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                    });
                return RedirectToAction("Perfil", "User", new { name = name });
            }

        }

        public async Task<IActionResult> LogOut() {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("TOKEN");
            return RedirectToAction("LogIn", "Managed");
        }
    }
}
