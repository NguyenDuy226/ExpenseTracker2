using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.API.DTO
{
    public class transactionDTO
    {
        [Required]
        public string Name {get; set;} = string.Empty;
        [Required]
        public DateTime Date {get; set;}
        [Required]
        [Range(0.1, Double.MaxValue)]
        public long Amount {get; set;}
        [Required]
        public int categoryId {get; set;}

    }
}