namespace UrbanSolution.Web.Infrastructure.Extensions
{
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using System.Linq;

    public static class ModelStateExtensions
    {
        public static string ErrorsAsString(this ModelStateDictionary modelStateDic)
        {
            return string.Join("; ", modelStateDic.Values
                                        .SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
        }
    }
}
