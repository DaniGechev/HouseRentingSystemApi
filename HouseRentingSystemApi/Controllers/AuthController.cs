using HouseRentingSystemApi.Models.Auth;
using HouseRentingSystemApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace HouseRentingSystemApi.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost("/login")]
        [Produces(typeof(AuthResult))]
        public async Task<IActionResult> Login([FromBody] AuthModel model)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(BuildValidationResult());
            }

            var result = await authService.LoginAsync(model);

            return StatusCode(result.Code, result);
        }

        [HttpPost("/register")]
        [Produces(typeof(AuthResult))]
        public async Task<IActionResult> Register([FromBody] AuthModel model)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(BuildValidationResult());
            }

            var result = await authService.RegisterAsync(model);

            return StatusCode(result.Code, result);
        }

        private AuthResult BuildValidationResult()
        {
            var allErrors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToArray();

            return new AuthResult
            {
                Code = 400,
                Message = string.Join(Environment.NewLine, allErrors)
            };
        }
    }
}
