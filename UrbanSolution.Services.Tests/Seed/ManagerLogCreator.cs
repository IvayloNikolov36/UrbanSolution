namespace UrbanSolution.Services.Tests.Seed
{
    using System;
    using System.Collections.Generic;
    using UrbanSolution.Models;
    using UrbanSolution.Models.Enums;

    public class ManagerLogCreator
    {
        public static List<ManagerLog> Create(string managerId)
        {
            var log = new ManagerLog
            {
                DateTime = new DateTime(2018, 12, 4),
                ManagerId = managerId
            };

            var secondLog = new ManagerLog
            {
                DateTime = new DateTime(2017, 11, 6),
                ManagerId = managerId,
                Activity = ManagerActivityType.EditedIssue
            };

            return new List<ManagerLog> { log, secondLog };
        }
    }
}
