namespace UrbanSolution.Models
{
    using MappingTables;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using static Utilities.DataConstants;

    public class Event
    {
        public int Id { get; set; }

        [Required]
        [StringLength(EventTitleMaxLength, MinimumLength = EventTitleMinLength)]
        public string Title { get; set; }

        [Required]
        [StringLength(EventDescriptionMaxLength, MinimumLength = EventDescriptionMinLength)]
        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int CloudinaryImageId { get; set; }

        public CloudinaryImage CloudinaryImage { get; set; }

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
