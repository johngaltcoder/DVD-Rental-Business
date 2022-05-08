using Microsoft.AspNetCore.Mvc;
using DVDRental.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace DVDRental.Controllers
{
    [Authorize(Roles = "Manager,Assistant")]
    public class MemberDetails: Controller
    {

        private readonly ApplicationDbContext  _context;

        public MemberDetails(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var ApplicationDbContext = _context.Member.OrderBy(m=>m.MemberFirstName).Include(m => m.MembershipCategory).ToList();
            ApplicationDbContext.ForEach(m=>m.LoanCount = _context.Loan.Where(l=>l.MemberNumber == m.MemberNumber && l.DateReturned == null).ToList().Count);
            return View(ApplicationDbContext.ToList());
        }
    }
}
