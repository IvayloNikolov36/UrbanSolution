using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UrbanSolution.Models;
using UrbanSolution.Services.Events;
using UrbanSolution.Services.Events.Models;
using UrbanSolution.Web.Areas.Events.Models;
using UrbanSolution.Web.Infrastructure;
using UrbanSolution.Web.Infrastructure.Extensions;
using UrbanSolution.Web.Infrastructure.Filters;

namespace UrbanSolution.Web.Areas.Events.Controllers
{
    [Area(WebConstants.EventsArea)]
    [Authorize(Roles = WebConstants.EventCreatorRole)]
    public class EventsController : Controller
    {
        private readonly IEventService events;
        private readonly UserManager<User> userManager;

        public EventsController(IEventService events, UserManager<User> userManager)
        {
            this.events = events;
            this.userManager = userManager;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(int page = 1)
        {
            var allEvents = await this.events.AllAsync<EventsListingServiceModel>(page);
            var totalEvents = await this.events.TotalCountAsync();

            var model = new EventsListingViewModel
            {
                Events = allEvents,
                TotalEvents = totalEvents,
                CurrentPage = page
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new EventCreateFormModel
            {
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(1).AddHours(1)
            };
            
            return this.View();
        }

        [HttpPost]
        //[ValidateModelState]
        public async Task<IActionResult> Create(EventCreateFormModel model)
        {
            var creatorId = (await this.userManager.GetUserAsync(this.User)).Id;

            var latitude = double.Parse(model.Latitude.Trim(), CultureInfo.InvariantCulture);
            var longitude = double.Parse(model.Longitude.Trim(), CultureInfo.InvariantCulture);

            await this.events.CreateAsync(model.Title, model.Description, model.StartDate, model.EndDate, model.PictureUrl, model.Address, latitude, longitude, creatorId);

            this.TempData.AddSuccessMessage(WebConstants.EventCreationSuccess);

            return this.RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var eventModel = await this.events.GetAsync<EventDetailsServiceModel>(id);
            
            return this.View(eventModel);
        }
    }
}