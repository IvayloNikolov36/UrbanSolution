using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace UrbanSolution.Web.Components
{
    [ViewComponent(Name = "DynamicMenu")]
    public class DynamicMenuViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {

            return this.View();
        }
    }
}
