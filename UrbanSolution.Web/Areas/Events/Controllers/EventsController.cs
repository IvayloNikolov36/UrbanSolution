using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrbanSolution.Web.Areas.Events.Models;
using UrbanSolution.Web.Infrastructure;

namespace UrbanSolution.Web.Areas.Events.Controllers
{
    [Area(WebConstants.EventsArea)]
    [Authorize(Roles = WebConstants.EventCreatorRole)]
    public class EventsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult Create(EventCreateFormModel model)
        {
            return this.RedirectToAction(nameof(Index));
        }
    }
}