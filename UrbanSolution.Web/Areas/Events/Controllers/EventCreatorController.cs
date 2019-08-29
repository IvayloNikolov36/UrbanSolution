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
    public class EventCreatorController : Controller
    {
        private readonly IEventService events;
        private readonly UserManager<User> userManager;

        public EventCreatorController( IEventService events, UserManager<User> userManager)
        {
            this.events = events;
            this.userManager = userManager;
        }

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
                .WithSuccess(string.Empty, EventCreationSuccess);
        }

        public IActionResult Edit(int id)
        {
            var model = this.events.GetAsync<EventEditServiceModel>(id);

            return this.ViewOrNotFound(model);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidateEventIdExistsAttribute))]
        [ValidateModelState]
        public async Task<IActionResult> Edit(int id, EventEditServiceModel model)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            var isEdited = await this.events.EditAsync(
                id, user.Id, model.Title, model.Description,model.StartDate, model.EndDate, 
                model.Address, model.Latitude, model.Longitude);

            if (!isEdited)
            {
                return this.RedirectToAction("Details", "Events", new {Area = "Events", id})
                    .WithDanger(string.Empty, CantEditEvent);
            }

            return this.RedirectToAction("Details", "Events", new { Area = "Events", id })
                .WithSuccess(string.Empty, EditEventSuccess); 
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