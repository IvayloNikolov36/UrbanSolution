using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace UrbanSolution.Models
{
    public class User : IdentityUser
    {
        [Required]
        [StringLength(30, MinimumLength = 6)]                          //TODO: constants in class
        public string FullName { get; set; }      

        [Range(14, 80)]
        public int Age { get; set; }
        
        //public GenderType Gender { get; set; }

        //public DateTime BirthDate { get; set; }

    }
}
