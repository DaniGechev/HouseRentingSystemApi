using HouseRentingSystemApi.Constants;
using HouseRentingSystemApi.Data.Entities;
using HouseRentingSystemApi.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HouseRentingSystemApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IConfiguration config;

        public AuthService(UserManager<AppUser> userManager, IConfiguration config)
        {
            this.userManager = userManager;
            this.config = config;
        }

        public async Task<AuthResult> RegisterAsync(AuthModel model)
        {
            var existingUser = await userManager.FindByEmailAsync(model.Email);

            if (existingUser != null)
            {
                return PopulateResult(400, null, "User Already exists");
            }

            var role = string.IsNullOrWhiteSpace(model.Role)
                ? RoleConstants.Client
                : model.Role.Trim();

            if (RoleConstants.All.Contains(role) == false)
            {
                return PopulateResult(
                    400,
                    null,
                    $"Invalid role. Allowed roles are: {string.Join(", ", RoleConstants.All)}");
            }

            var newUser = new AppUser()
            {
                Email = model.Email,
                UserName = model.Username
            };

            var result = await userManager.CreateAsync(newUser, model.Password);

            if (result.Succeeded == false)
            {
                return PopulateResult(
                    400,
                    null,
                    result.Errors.Select(e => e.Description).ToArray());
            }

            await userManager.AddToRoleAsync(newUser, role);

            return PopulateResult(200, null, "User registered Successfully");
        }

        public async Task<AuthResult> LoginAsync(AuthModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return PopulateResult(401, null, "Invalid email or password");
            }

            var passwordValid = await userManager.CheckPasswordAsync(user, model.Password);

            if (passwordValid == false)
            {
                return PopulateResult(401, null, "Invalid email or password");
            }

            var token = await GenerateJwtToken(user);

            return PopulateResult(200, token, "User logged in successfully");
        }

        private async Task<string> GenerateJwtToken(AppUser user)
        {
            var jwtSection = config.GetSection("Jwt");
            var key = jwtSection["Key"]!;

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName!)
            };

            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(
                int.Parse(jwtSection["ExpiresMinutes"]!)
            );

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private AuthResult PopulateResult(int code, string? token = null, params string[] messages)
        {
            var result = new AuthResult();
            result.Code = code;
            result.Message = string.Join(Environment.NewLine, messages);
            if (token != null)
            {
                result.Token = token;
            }

            return result;
        }
    }
}
