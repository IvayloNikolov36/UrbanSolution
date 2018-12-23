namespace UrbanSolution.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Utilities;

    public class UrbanService
    {
        public int Id { get; set; }

        [Required,StringLength(DataConstants.UrbanServiceNameMaxLength, MinimumLength = DataConstants.UrbanServiceNameMinLength)]
        public string Name { get; set; }

        [Required, StringLength(DataConstants.UrbanServiceDescriptionMaxLength, MinimumLength = DataConstants.UrbanServiceDescriptionMinLenght)]
        public string Description { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public DateTime OfferedSince { get; set; }

        [Required]
        [Url]
        public string PictureUrl { get; set; }
    }
}
