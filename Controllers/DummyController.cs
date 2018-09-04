using Microsoft.AspNetCore.Mvc;

namespace TestAuth.Controllers
{
    [Route("[controller]/[action]")]
    public class DummyController : Controller
    {
        public IActionResult Message()
        {
            return Ok("Dummy response");
        }
    }
}