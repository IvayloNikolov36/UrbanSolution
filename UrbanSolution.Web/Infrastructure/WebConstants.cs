namespace UrbanSolution.Web.Infrastructure
{
    public class WebConstants
    {
        public const string AdminRole = "Administrator";
        public const string ManagerRole = "Manager";

        public const string AdminUserName = "administrator";
        public const string ManagerUserName = "manager";

        public const string DefaultAdminPassword = "adminA123";
        public const string DefaultManagerPassword = "managerM123";

        public const string AdminArea = "Admin";
        public const string ManagerArea = "Manager";

        public const string TempDataSuccessMessageKey = "SuccessMessage";
        public const string TempDataErrorMessageKey = "ErrorMessage";
        public const string TempDataInfoMessageKey = "InfoMessage";

        public const string UserAddedToRoleSuccess = "User {0} successfully added to role {1}.";
        public const string UserRemovedFromRoleSuccess = "User {0} successfully removed from role {1}.";
        public const string UserIsNotSetToRole = "User {0} is not set to role {1}.";
        public const string UserAlreadyInRole = "User {0} has been already {1}.";

        public const string IssueUploaded = "Successfully uploaded issue.";
        public const string ResolvedUploaded = "Successfully uploaded resolved.";

        public const string MapCenterLatitude = "42.138893";
        public const string MapCenterLongitude = "24.741101";

        public const string IssueApprovedSuccess = "Successfully approved issue.";
        public const string IssueUpdateSuccess = "Successfully updated issue.";
        public const string IssueDeleteSuccess = "Successfully deleted issue.";
        public const string IssueNotFound = "Issue with this id can't be found!";

        public const string ViewDataIssueId = "IssueId";
    }
}
