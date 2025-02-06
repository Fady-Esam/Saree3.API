using Microsoft.AspNetCore.Mvc;
namespace Saree3.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Hello From Saree3 App");
        }
    }
}
