namespace UrbanSolution.Web.Models.Areas.Admin
{
    using System.ComponentModel.DataAnnotations;

    public class LockUserInputModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public int LockDays { get; set; }
    }
}
