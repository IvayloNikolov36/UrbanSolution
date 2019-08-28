namespace UrbanSolution.Services.Utilities
{
    public class ServiceConstants
    {
        public const int IssuesPageSize = 6;
        public const int BlogArticlesPageSize = 2;
        public const int EventsPageSize = 2;

        public const string ImageUploadPath = "{0}\\images\\upload";

        public const string CloudinaryGetImageUrl = "{0}.jpg";

        public const long PictureUploadFileLength = 5500000;

        public const string MessageForImageUploadingRestrictions =
            "Your file submission should be a '.jpg' file with no more than 5.5mb size";

        public const string PictureExtension = ".jpg";

        public const string EventStartDateRestriction =
            "Start date of the event should be at least one day after creation!";

        public const string EventEndDateRestriction =
            "End date of the event should be at least one hour after start time!";

        public const string NoCoordinatesValidationError = "Please find the adress in the map.";

        public const string SortAsc = "ASC";
        public const string SortDesc = "DESC";
    }
}
