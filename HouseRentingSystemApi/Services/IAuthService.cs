using HouseRentingSystemApi.Models.Auth;

namespace HouseRentingSystemApi.Services
{
    public interface IAuthService
    {
        Task<AuthResult> RegisterAsync(AuthModel model);

        Task<AuthResult> LoginAsync(AuthModel model);
    }
}
