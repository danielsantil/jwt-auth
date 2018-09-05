using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TestAuth.Controllers
{
    [Route("[controller]/[action]")]
    public class DummyController : Controller
    {
        [Authorize]
        public IActionResult Secure()
        {
            return Ok("JWT works!");
        }

        public IActionResult Unsecure()
        {
            return Ok("Unsecure gateway");
        }
    }
}