namespace UrbanSolution.Models
{
    using System;
    using Enums;

    public class AdminLog
    {
        public int Id { get; set; }

        public string AdminId { get; set; }

        public User Admin { get; set; }

        public string EditedUserId { get; set; }

        public User EditedUser { get; set; }

        public AdminActivityType Activity { get; set; }

        public string ForRole { get; set; } 

        public DateTime CreatedOn { get; set; }

    }
}
