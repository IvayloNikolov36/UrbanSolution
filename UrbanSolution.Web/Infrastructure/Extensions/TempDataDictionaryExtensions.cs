namespace UrbanSolution.Web.Infrastructure.Extensions
{
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using static UrbanSolutionUtilities.WebConstants;

    public static class TempDataDictionaryExtensions
    {
        public static void AddSuccessMessage(this ITempDataDictionary tempData, string successMessage)
        {
            tempData[TempDataSuccessMessageKey] = successMessage;
        }

        public static void AddErrorMessage(this ITempDataDictionary tempData, string errorMessage)
        {
            tempData[TempDataErrorMessageKey] = errorMessage;
        }

        public static void AddInfoMessage(this ITempDataDictionary tempData, string infoMessage)
        {
            tempData[TempDataInfoMessageKey] = infoMessage;
        }
    }
}
