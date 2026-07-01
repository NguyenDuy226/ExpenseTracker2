using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.API.DTO
{
    public class loginDTO
    {
        [Required]
        [EmailAddress]
        [MinLength(6)]
        public string Email {get; set;} = string.Empty;
        [Required]
        [MinLength(6)]
        public string Password {get; set;} = string.Empty;

    }
}