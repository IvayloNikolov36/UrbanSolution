using System;
using UrbanSolution.Models.Enums;

namespace UrbanSolution.Models
{
    public class AdminLog
    {
        public int Id { get; set; }

        public string AdminId { get; set; }

        public User Admin { get; set; }

        public string EditedUserId { get; set; }

        public User EditedUser { get; set; }

        public ActivityType Activity { get; set; }

        public DateTime DateTime { get; set; }

    }
}
