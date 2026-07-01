using ExpenseTracker.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.API.Data
{
    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole, string>
    {
        public AppDbContext (DbContextOptions<AppDbContext> options) : base(options){
            
        }
        public DbSet<Transaction> Transactions {get; set;}
        public DbSet<Category> Categories {get; set;}
    }
}