namespace UrbanSolutionUtilities.Extensions
{
    using System;
    using UrbanSolution.Models;
    using static UrbanSolutionUtilities.WebConstants;   

    public static class EnumExtensions
    {
        public static string ToFriendlyName(this IssueType type)
        {
            switch (type)
            {
                case IssueType.Other:
                case IssueType.Sidewalks:
                    return type.ToString();
                case IssueType.DangerousBuildings:
                    return "Dangerous buildings";
                case IssueType.DangerousTrees:
                    return "Dangerous trees";
                case IssueType.GreenSpaces:
                    return "Green spaces";
                case IssueType.ParkingZones:
                    return "Parking zones";
                case IssueType.PublicTransport:
                    return "Public transport";
                case IssueType.DamagedRoads:
                    return "Damaged roads";
                case IssueType.StreetLighting:
                    return "Street Lighting";
                default:
                    return "";
            }
        }

        public static Tuple<double, double, double> GetMapPositions(this RegionType region)
        {
                switch (region)
                { 
                    case RegionType.All:
                    case RegionType.Central:
                        return new Tuple<double, double, double>(CenterRegLat, CenterRegLong, CenterRegZoom);
                    case RegionType.North:
                        return new Tuple<double, double, double>(NorthRegLat, NorthRegLong, NorthRegZoom);
                    case RegionType.South:
                        return new Tuple<double, double, double>(SouthRegLat, SouthRegLong, SouthRegZoom);
                    case RegionType.Eastern:
                        return new Tuple<double, double, double>(EasternRegLat, EasternRegLong, EasternRegZoom);
                    case RegionType.Western:
                        return new Tuple<double, double, double>(WesternRegLat, WesternRegLong, WesternRegZoom);
                    case RegionType.Thracia:
                        return new Tuple<double, double, double>(ThraciaRegLat, ThraciaRegLong, ThraciaRegZoom);
                    default:
                        return null;
                }
        }
        
    }
}
