using System;

namespace UrbanSolution.Services.Models
{
    public class UrbanIssuesListingServiceModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string IssuePictureUrl { get; set; }

        public bool HasResolved { get; set; }

        public DateTime PublishedOn { get; set; }

        public string Publisher { get; set; }

        public bool IsApproved { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

    }
}
