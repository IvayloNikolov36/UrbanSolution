namespace UrbanSolution.Web.Areas.Admin.Models
{
    using System.ComponentModel.DataAnnotations;

    public class UnlockUserInputModel
    {
        [Required]
        public string UserId { get; set; }
    }
}
