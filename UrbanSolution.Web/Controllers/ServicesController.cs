using Microsoft.AspNetCore.Mvc;

namespace UrbanSolution.Web.Controllers
{
    public class ServicesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}