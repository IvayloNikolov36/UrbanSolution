namespace UrbanSolution.Web.Areas.Events.Controllers
{
    using Infrastructure.Extensions;
    using Infrastructure.Filters;
    using Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Services.Events;
    using UrbanSolution.Services.Events.Models;
    using static Infrastructure.WebConstants;

    [Area(EventsArea)]
    [Authorize(Roles = EventCreatorRole)]
    public class EventsController : Controller
    {
        private readonly IEventService events;
        private readonly UserManager<User> userManager;

        public EventsController( IEventService events, UserManager<User> userManager)
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
            var model = this.GetEventDateTimeProperties();
            
            return this.View(model);
        }

        [HttpPost]
        [ValidateModelState]
        public async Task<IActionResult> Create(EventCreateFormModel model)
        {
            var creator = await this.userManager.GetUserAsync(this.User);

            var eventId = await this.events.CreateAsync(model.Title, model.Description, model.StartDate,
                model.EndDate, model.PictureFile, model.Address, model.Latitude, model.Longitude, creator.Id);

            return this.RedirectToAction("Details", "Events", new { Area = "Events", id = eventId })
                .WithSuccess("", EventCreationSuccess);
        }

        public IActionResult Edit(int id)
        {
            var model = this.events.GetAsync<EventEditServiceModel>(id);

            return this.ViewOrNotFound(model);
        }

        [HttpPost]                                      
        [ValidateModelState]
        public async Task<IActionResult> Edit(int id, EventEditServiceModel model)
        {
            //TODO: use ValidateIdExistsFilter

            var user = await this.userManager.GetUserAsync(this.User);

            var isEdited = await this.events.EditAsync(
                id, user.Id, model.Title, model.Description,model.StartDate, model.EndDate, 
                model.Address, model.Latitude, model.Longitude);

            if (!isEdited)
            {
                return this.RedirectToAction("Details", "Events", new {Area = "Events", id})
                    .WithDanger("", CantEditEvent);
            }

            return this.RedirectToAction("Details", "Events", new { Area = "Events", id })
                .WithSuccess("", EditEventSuccess); 
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var eventModel = await this.events.GetAsync<EventDetailsServiceModel>(id);

            var user = await this.userManager.GetUserAsync(this.User);

            this.ViewData[ViewDataUsernameKey] = user.UserName;

            return this.ViewOrNotFound(eventModel);
        }

        private EventCreateFormModel GetEventDateTimeProperties()
        {
            var model = new EventCreateFormModel
            {
                StartDate = DateTime.UtcNow.AddDays(1).AddMinutes(15),
                EndDate = DateTime.UtcNow.AddDays(1).AddHours(1).AddMinutes(15)
            };

            return model;
        }
    }
}