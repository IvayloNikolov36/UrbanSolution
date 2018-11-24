using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace UrbanSolution.Web.Infrastructure.Extensions
{
    public static class TempDataDictionaryExtensions
    {
        public static void AddSuccessMessage(this ITempDataDictionary tempData, string successMessage)
        {
            tempData[WebConstants.TempDataSuccessMessageKey] = successMessage;
        }

        public static void AddErrorMessage(this ITempDataDictionary tempData, string errorMessage)
        {
            tempData[WebConstants.TempDataErrorMessageKey] = errorMessage;
        }

        public static void AddInfoMessage(this ITempDataDictionary tempData, string infoMessage)
        {
            tempData[WebConstants.TempDataInfoMessageKey] = infoMessage;
        }
    }
}
