using System.Security.Claims;
using ExpenseTracker.API.Data;
using ExpenseTracker.API.DTO;
using ExpenseTracker.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.API.Controller
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class transactionController : ControllerBase
    {   
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<AppUser> _userManager;
        public transactionController (AppDbContext appDbContext, UserManager<AppUser> userManager)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
        }
       
        private string getID()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId == null) return "Khong tim thay user";
            return userId;            
        }

        //GET /transactions
        [HttpGet]
        public async Task<IActionResult> getTransaction()
        {
            var userId =  getID();
            var transactions = await _appDbContext.Transactions
            .Where(t => t.AppUserId==userId)
            .ToListAsync();
            
            return Ok(transactions);
        }
        //POST /transactions
        [HttpPost]
        public async Task<IActionResult> postTransaction(transactionDTO dto)
        {
            var userId = getID();
            //khong tim thay category
            var checkCategory = await _appDbContext.Categories.FindAsync(dto.categoryId);
            if(checkCategory == null) return BadRequest();
            var transaction = new Transaction
            {
                Name = dto.Name,
                Date = dto.Date,
                Amount = dto.Amount,
                categoryId = dto.categoryId,
                AppUserId = userId
            };
            _appDbContext.Transactions.Add(transaction);
            await _appDbContext.SaveChangesAsync();
            return Ok();
        }

        //PUT /transactions/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> putById(transactionDTO dto, int id)
        {
            var userId = getID();
            var transaction = await _appDbContext.Transactions.FindAsync(id);
            if(transaction == null || transaction.AppUserId != userId) return NotFound();
            var checkCategory = await _appDbContext.Categories.FindAsync(dto.categoryId);
            if(checkCategory == null) return BadRequest();
            //put
            transaction.Name = dto.Name;
            transaction.Date = dto.Date;
            transaction.Amount = dto.Amount;
            transaction.categoryId = dto.categoryId;
            await _appDbContext.SaveChangesAsync();
            return Ok();
        }

        //DELETE /transactions/{id
        [HttpDelete("{id}")]
        public async Task<IActionResult> deleteById (int id)
        {
            var userId = getID();
            var transaction = await _appDbContext.Transactions.FindAsync(id);
            if(transaction == null || transaction.AppUserId != userId) return NotFound();

            _appDbContext.Transactions.Remove(transaction); 
            await _appDbContext.SaveChangesAsync();
            return Ok();
        }
        
        //GET /transactions/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> getById (int id)
        {
            var userId = getID();
            var transaction = await _appDbContext.Transactions.FindAsync(id);
            if(transaction == null || transaction.AppUserId != userId) return NotFound();

            return Ok(transaction);
        }
   
       //GET /transactions/summary?year=2025&month=1
       [HttpGet("summary")]
       public async Task<IActionResult> getSummary ([FromQuery] int year, [FromQuery] int month)
        {
            var userId = getID();
            var transactions = await _appDbContext.Transactions
            .Include(t => t.category)
            .Where(t => t.AppUserId == userId && t.Date.Year == year && t.Date.Month == month)
            .ToListAsync();

            //Tong thu, chi, so du
            long totalThu = 0;
            long totalChi = 0;
            foreach(var i in transactions)
            {
                if(i.category?.Type == "Thu") totalThu += i.Amount;
                else totalChi += i.Amount;
            }
            long totalBalance = totalThu - totalChi;
            
            //Khoan chi tieu lon nhat
            var allExpense = transactions
            .Where(t => t.category?.Type == "Chi")
            .GroupBy(t => t.category?.Name);
            long maxAmount = 0;
            string maxExpense = "";
            foreach(var group in allExpense)
            {
                long totalAmount = 0;
                foreach(var i in group)
                {
                    totalAmount += i.Amount;
                }
                if(totalAmount > maxAmount)
                {
                    maxAmount = totalAmount;
                    maxExpense = group.Key ?? "";
                }
            }
            if(maxExpense == "") maxExpense = "Không có chi tiêu trong thời điểm này";
            return Ok(new
            {
                Year = year,
                Month = month,
                TotalThu = totalThu,
                TotalChi = totalChi,
                TotalBalance = totalBalance,
                MaxExpense = maxExpense
            });
        } 
    
        [HttpGet("summary/monthly")]
        public async Task<IActionResult> getMonthlySummary ([FromQuery] int year)
        {
            var userId = getID();
            var transactions = await _appDbContext.Transactions
            .Include(t => t.category)
            .Where(t => t.AppUserId == userId && t.Date.Year == year)
            .OrderByDescending(t => t.Date)
            .ToListAsync();

            var monthlyTransaction = transactions.GroupBy(t => t.Date.Month).ToList();
            long [] totalThu = new long[12];
            long [] totalChi = new long[12];
            foreach(var group in monthlyTransaction)
            {
                long monthlyThu = 0;
                long monthlyChi = 0;
                int month = group.Key - 1;
                foreach(var i in group)
                {
                    if(i.category?.Type == "Thu") monthlyThu += i.Amount;
                    else monthlyChi += i.Amount;
                }
                totalChi[month] = monthlyChi;
                totalThu[month] = monthlyThu;
            }

            return Ok(new 
            {
                Year = year,
                MonthlyThu = totalThu,
                MonthlyChi = totalChi
            });
        }


    }
}