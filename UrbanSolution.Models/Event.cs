using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UrbanSolution.Models.MappingTables;
using UrbanSolution.Models.Utilities;

namespace UrbanSolution.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required]
        [StringLength(DataConstants.EventTitleMaxLength, MinimumLength = DataConstants.EventTitleMinLength)]
        public string Title { get; set; }

        [Required]
        [StringLength(DataConstants.EventDescriptionMaxLength, MinimumLength = DataConstants.EventTitleMinLength)]
        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        [Url]
        public string PictureUrl { get; set; }

        [Required]
        public string Address { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string CreatorId { get; set; }

        public User Creator { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public ICollection<EventUser> Participants { get; set; } = new List<EventUser>();

    }
}
