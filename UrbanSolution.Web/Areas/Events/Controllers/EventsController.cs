namespace UrbanSolution.Web.Controllers
{
    using Infrastructure.Extensions;
    using Infrastructure.Filters;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;    
    using Services.Events;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Web.Models.Areas.Events;
    using UrbanSolution.Web.Models.Common;
    using static UrbanSolutionUtilities.WebConstants;

    [Area(EventsArea)]
    [Authorize]
    public class EventsController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly IEventService events;

        public EventsController(UserManager<User> userManager, IEventService events)
        {
            this.userManager = userManager;
            this.events = events;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(int page = 1)
        {
            var allEvents = await this.events.AllAsync<EventsListingModel>(page);

            var totalEvents = await this.events.TotalCountAsync();

            var model = new EventsListingViewModel
            {
                Events = allEvents,
                PagesModel = new PagesModel
                {
                    TotalItems = totalEvents,
                    CurrentPage = page,
                    ItemsOnPage = EventsPageSize
                }
            };

            return View(model);
        }

        [HttpGet]
        [ServiceFilter(typeof(ValidateEventIdExistsAttribute))]
        public async Task<IActionResult> Participate(int id)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            var canParticipate = await this.events.Participate(id, user.Id);

            if (!canParticipate)
            {
                return this.RedirectToAction("Details", "Events", new { Area = "Events", id })
                    .WithDanger(string.Empty, AlreadyEventParticipant);
            }

            return this.RedirectToAction("Details", "Events", new { Area = "Events", id })
                .WithSuccess(string.Empty, SuccessParticipation);
        }

        [HttpGet]
        [ServiceFilter(typeof(ValidateEventIdExistsAttribute))]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var eventModel = await this.events.GetAsync<EventDetailsViewModel>(id);

            var user = await this.userManager.GetUserAsync(this.User);

            this.ViewData[ViewDataUsernameKey] = user.UserName;

            return this.ViewOrNotFound(eventModel);
        }
    }
}
