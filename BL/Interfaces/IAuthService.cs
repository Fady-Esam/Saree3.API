using Saree3.API.Models;

namespace Saree3.API.BL.Interfaces
{
    public interface IAuthService
    {
        Task<APIResponse> RegisterCustomerWithEmailAndPassword(RegisterModel RegisterModel);
        Task<APIResponse> RegisterRiderWithEmailAndPassword(RegisterModel RegisterModel);
        Task<APIResponse> LoginWithEmailAndPassword(LogInModel LogInModel);

        //Task<ApiResponse> LoginWithFacebook(string accessToken);
        //Task<ApiResponse> LoginWithApple(string idToken);
        //ApiResponse LoginWithGoogle(string idToken);
    }
}
