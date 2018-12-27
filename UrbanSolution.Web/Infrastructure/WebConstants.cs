namespace UrbanSolution.Web.Infrastructure
{
    public class WebConstants
    {
        public const string AdminArea = "Admin";
        public const string ManagerArea = "Manager";
        public const string BlogArea = "Blog";
        public const string EventsArea = "Events";

        public const string AdminRole = "Administrator";
        public const string ManagerRole = "Manager";
        public const string BlogAuthorRole = "BlogAuthor";
        public const string EventCreatorRole = "EventCreator";

        public const string AdminUserName = "administrator";
        public const string ManagerUserName = "manager";
        public const string BlogAuthorUserName = "blogger";
        public const string EventCreatorUserName = "eventCreator";

        public const string AdminFullName = "System Administrator";
        public const string ManagerFullName = "Regional Manager";
        public const string BlogAuthorFullName = "Blog Author";
        public const string EventCreatorFullName = "Event Creator";

        public const int UserDefaultAge = 20;

        public const string AdminEmail = "admin@example.com";
        public const string ManagerEmail = "manager@example.com";
        public const string RegionalManagerEmail = "{0}.2019@abv.bg";
        public const string BlogAuthorEmail = "blogAuthor.2019@abv.bg";
        public const string EventCreatorEmail = "event.creator.2019@abv.bg";

        public const string DefaultAdminPassword = "adminA123";
        public const string DefaultManagerPassword = "managerM123{0}";
        public const string DefaultBlogAuthorPassword = "bloggerB123";
        public const string DefaultEventCreatorPassword = "eventCreator123";

        public const string TempDataSuccessMessageKey = "SuccessMessage";
        public const string TempDataErrorMessageKey = "ErrorMessage";
        public const string TempDataInfoMessageKey = "InfoMessage";

        public const string UserAddedToRoleSuccess = "User {0} successfully added to role {1}.";
        public const string UserRemovedFromRoleSuccess = "User {0} successfully removed from role {1}.";
        public const string UserIsNotSetToRole = "User {0} is not set to role {1}.";
        public const string UserAlreadyInRole = "User {0} has been already {1}.";

        public const string ResolvedUploaded = "Successfully uploaded resolved issue.";
        public const string ResolvedDeleted = "Resolved issuesuccessfully deleted.";
        public const string ResolvedUpdated = "Resolved issue successfully updated.";

        public const string MapCenterLatitude = "42.138893";
        public const string MapCenterLongitude = "24.741101";
        public const int MapCenterZoom = 13;

        public const string IssueUploaded = "Successfully uploaded issue.";
        public const string IssueApprovedSuccess = "Successfully approved issue.";
        public const string IssueUpdateSuccess = "Successfully updated issue.";
        public const string IssueDeleteSuccess = "Successfully deleted issue.";
        public const string IssueNotFound = "Issue with this id can't be found!";
        public const string CantChangeIssue = "You do not have permissions to this issue!";

        public const string ViewDataIssueId = "issueId";
        public const string ViewDataUserId = "userId";
        public const string ViewDataManagerId = "managerId";
        public const string ViewDateTitle = "title";
        public const string ViewDateApprovedIssuesTitle = "Approved issues";
        public const string ViewDateResolvedIssuesTitle = "Resolved issues";

        public const string EventCreationSuccess = "Event successfylly created.";

        public const string CurrentTownEn = "Plovdiv";
        public const string CurrentTownBg = "Пловдив";

        public const string MarkerNotPlaced =
            "No coordinates. Find the place on the map and get the address of the place.";

        public const string NoAddressSet =
            "No address is set! Please, Find the place on the map and press Get Address button";

        public const int PictureUploadFileLength = 5500000;


        public const string NoIssuesToShow = "There is no issues to show";


        //Google Map Regions Coordinates

        public const double CenterRegLat = 42.14431437;
        public const double CenterRegLong = 24.74481583;
        public const double CenterRegZoom = 12.7;

        public const double SouthRegLat = 42.11668869;
        public const double SouthRegLong = 24.73846436;
        public const double SouthRegZoom = 13.7;

        public const double WesternRegLat = 42.1308213;
        public const double WesternRegLong = 24.70705032;
        public const double WesternRegZoom = 15;

        public const double NorthRegLat = 42.15882258;
        public const double NorthRegLong = 24.73846436;
        public const double NorthRegZoom = 16;

        public const double EasternRegLat = 42.14749628;
        public const double EasternRegLong = 24.77811813;
        public const double EasternRegZoom = 16;

        public const double ThraciaRegLat = 42.13273087;
        public const double ThraciaRegLong = 24.78721619;
        public const double ThraciaRegZoom = 14;
    }
}
