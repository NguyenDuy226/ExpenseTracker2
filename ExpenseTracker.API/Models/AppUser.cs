using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.API.Models
{
    public class AppUser : IdentityUser
    {
        public string Name {get; set;}= string.Empty;
        List<Transaction> Transactions = [];
        
    }
}