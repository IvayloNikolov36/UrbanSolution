using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UrbanSolution.Data;
using UrbanSolution.Models;
using UrbanSolution.Web.Infrastructure.Extensions;
using static UrbanSolution.Web.Infrastructure.WebConstants;

namespace UrbanSolution.Web.Infrastructure.Filters
{
    public class ValidateEventIdExistsAttribute : ActionFilterAttribute
    {
        private readonly UrbanSolutionDbContext db;

        public ValidateEventIdExistsAttribute(UrbanSolutionDbContext db)
        {
            this.db = db;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = context.Controller as Controller;
            if (controller == null)
            {
                return;
            }

            var id = context.ActionArguments
                .FirstOrDefault(a => a.Key.ToLower().Contains("id")).Value;
            if (id == null)
            {
                context.Result = controller.RedirectToAction("Index") //TODO: magic string
                    .WithDanger(string.Empty, EventNotFound);
            }
            else
            {
                var exists = this.db.Find<Event>(id) != null;

                if (!exists)
                {
                    context.Result = controller.RedirectToAction("Index")
                        .WithDanger(string.Empty, EventNotFound);
                }
            }

        }
    }
}
