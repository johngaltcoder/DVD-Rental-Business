using Microsoft.AspNetCore.Mvc;
using DVDRental.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace DVDRental.Controllers
{
    [Authorize(Roles = "Manager,Assistant")]
    public class DVDLoanController: Controller
    {

        private readonly ApplicationDbContext  _context;

        public DVDLoanController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string searchString)
        {
            var ApplicationDbContext = _context.Loan;

            if (!String.IsNullOrEmpty(searchString))
            {
                var loan = ApplicationDbContext.Where(l => l.CopyNumber == int.Parse(searchString)).Include(l=>l.DVDCopy)
                    .ThenInclude(dc=>dc.DVDTitle).Include(l=>l.Member).OrderBy(l=>l.DateOut).LastOrDefault();
                return View(loan);
            }
            return View();
        }
    }
}
