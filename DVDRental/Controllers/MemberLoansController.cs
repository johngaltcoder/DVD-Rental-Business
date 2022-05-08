using Microsoft.AspNetCore.Mvc;
using DVDRental.Models;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;

namespace DVDRental.Controllers
{
    [Authorize(Roles = "Manager, Assistant")]
    public class MemberLoansController: Controller
    {
        private readonly ApplicationDbContext _context;

        public MemberLoansController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string searchString)
        {
            var results = _context.Member.Include(m => m.Loan)
                .ThenInclude(l=>l.DVDCopy)
                .ThenInclude(c=>c.DVDTitle)
                .Where(m=>m.Loan.All(l=>l.DateOut <= DateTime.UtcNow.AddDays(30)))
                .Where(m =>m.MemberLastName.Contains(searchString)).FirstOrDefault();
            ViewData["member"] = results;
            if(results == null)
            {
                ViewData["loans"] = new List<Loan>();
            }
            else
            {
                ViewData["loans"] = results.Loan;
            }
            return View();
        }
    }
}
