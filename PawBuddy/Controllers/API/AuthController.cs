using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PawBuddy.Models.ApiModels;

namespace PawBuddy.Controllers.API
{
    [Route("api/AuthController")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AuthController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet("hello")]
        [Authorize] 
        public ActionResult Hello()
        {
            return Ok($"Hello, {User.Identity.Name}!");
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginModel loginRequest)
        {
            var result = await _signInManager.PasswordSignInAsync(
                loginRequest.Email, 
                loginRequest.Password, 
                isPersistent: false, 
                lockoutOnFailure: false
            );

            if (result.Succeeded)
            {
                return Ok(new { message = "Login successful", user = loginRequest.Email });
            }

            return BadRequest("Erro no login. Verifique o utilizador e a password.");
        }

        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return NoContent();
        }
    }
}