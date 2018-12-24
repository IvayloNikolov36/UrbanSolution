
namespace UrbanSolution.Models
{
    using Enums;
    using System;   

    public class ManagerLog
    {
        public int Id { get; set; }
        
        public string  ManagerId{ get; set; }

        public User Manager { get; set; }

        public ManagerActivityType Activity { get; set; }

        public DateTime DateTime { get; set; }
    }
}
