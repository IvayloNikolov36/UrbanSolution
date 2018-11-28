using System;
using UrbanSolution.Models;

namespace Infrastructure
{
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
    }
}
