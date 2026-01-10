using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.WebUI.Controllers
{
    [Route("Error")]
    public class ErrorController : Controller
    {
        [Route("AccessDenied")]
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [Route("PageNotFound")]
        public IActionResult PageNotFound(int code)
        {
            return View();
        }

        [Route("GeneralError")]
        public IActionResult GeneralError()
        {
            return View();
        }
    }
}
