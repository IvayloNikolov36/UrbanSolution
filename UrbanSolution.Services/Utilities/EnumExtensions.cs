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
                    return "User added to role";
                case AdminActivityType.RemovedFromRole:
                    return "User removed from role";
                default:
                    return null;
            }
        }
    }
}
