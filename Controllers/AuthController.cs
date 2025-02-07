using Microsoft.AspNetCore.Mvc;

namespace Saree3.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            
            return Ok("Data Success");
        
        }

        private void SetRefreshTokenInCookie(string refreshToken, DateTime expiresOn)
        {
            var cookie = new CookieOptions
            {
                HttpOnly = true,
                Expires = expiresOn.ToLocalTime()
            };

            Response.Cookies.Append("refToken", refreshToken, cookie);


        }

    }
}
