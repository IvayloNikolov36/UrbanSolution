namespace UrbanSolution.Web.Models.Areas.Admin
{
    using System.ComponentModel.DataAnnotations;

    public class UnlockUserInputModel
    {
        [Required]
        public string UserId { get; set; }
    }
}
