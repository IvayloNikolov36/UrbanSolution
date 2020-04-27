namespace UrbanSolution.Services.Utilities
{
    using UrbanSolution.Models.Enums;

    public static class EnumExtensions
    {
        public static string ToFriendlyName(this AdminActivityType adminActivityType)
        {
            switch (adminActivityType)
            {
                case AdminActivityType.AddedToRole:
                    return " has been added to role ";
                case AdminActivityType.RemovedFromRole:
                    return " has been removed from role ";
                case AdminActivityType.LockedUser:
                    return " has been locked";
                case AdminActivityType.UnlockedUser:
                    return " has been unlocked";
                default:
                    return null;
            }
        }
    }
}
