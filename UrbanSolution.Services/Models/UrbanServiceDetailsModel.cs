namespace UrbanSolution.Services.Models
{
    using Mapping;
    using System;
    using UrbanSolution.Models;
    
    public class UrbanServiceDetailsModel : IMapFrom<UrbanService>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public DateTime OfferedSince { get; set; }

        public string PictureUrl { get; set; }

    }
}
