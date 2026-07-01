using Microsoft.VisualBasic;

namespace ExpenseTracker.API.Models
{
    public class Transaction
    {
        public int Id {get; set;}
        public string Name {get; set;} = string.Empty;
        public DateTime Date {get; set;}
        public long Amount {get; set;}
        public string AppUserId {get; set;} = string.Empty;
        public AppUser? appUser {get; set;}
        public int categoryId {get; set;}
        public Category? category {get; set;}

    }
}