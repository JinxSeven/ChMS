using ChMS.Modules.Auth.Application.Services;
using ChMS.Modules.Auth.Core.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace ChMS.Modules.Auth.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(AuthService authService) : ControllerBase
    {
        private readonly AuthService _auth = authService;

        [HttpPost("signup")]
        public async Task<IActionResult> Signup(SignUpRequest signUpRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = await _auth.Signup(signUpRequest);
            return CreatedAtAction(
                actionName: "GetUserById",
                controllerName: "User",
                routeValues: new { id = userId },
                value: new { id = userId }
            );
        }

        [HttpPost("signin")]
        public async Task<IActionResult> Signin(SignInRequest req)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (res, refreshToken) = await _auth.Signin(req.Email, req.Password);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7),
                Path = "/",
                IsEssential = true,
            };
            Response.Cookies.Append("refresh-token-chms", refreshToken, cookieOptions);

            return Ok(res);
        }
    }
}
