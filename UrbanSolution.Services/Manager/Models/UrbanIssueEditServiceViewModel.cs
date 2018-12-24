namespace UrbanSolution.Services.Manager.Models
{
    using AutoMapper;
    using Mapping;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using UrbanSolution.Models;
   
    public class UrbanIssueEditServiceViewModel : IMapFrom<UrbanIssue>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Publisher { get; set; }

        [Display(Name = "Published on")]
        public DateTime PublishedOn { get; set; }

        public string Description { get; set; }

        [Display(Name = "Street")]
        public string AddressStreet { get; set; }

        public RegionType Region { get; set; }

        public IEnumerable<SelectListItem> Regions { get; set; }

        public IEnumerable<SelectListItem> IssueTypes { get; set; }

        public IssueType Type { get; set; }

        public bool IsApproved { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<UrbanIssue, UrbanIssueEditServiceViewModel>()
                .ForMember(x => x.Publisher, m => m.MapFrom(u => u.Publisher.UserName));
        }
    }
}
