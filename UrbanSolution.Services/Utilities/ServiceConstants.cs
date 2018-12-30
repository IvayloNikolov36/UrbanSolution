namespace UrbanSolution.Services.Utilities
{
    public class ServiceConstants
    {
        public const int IssuesPageSize = 8;
        public const int BlogArticlesPageSize = 2;
        public const int EventsPageSize = 2;

        public const string ImageUploadPath = "{0}\\images\\upload";

        public const string CloudinaryGetImageUrl = "{0}.jpg";

        public const long PictureUploadFileLength = 5500000;

        public const string MessageForImageUploadingRestrictions =
            "Your file submission should be a '.jpg' file with no more than 5.5mb size";

        public const string PictureExtension = ".jpg";
    }
}
