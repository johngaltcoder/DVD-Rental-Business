#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DVDRental.Models;

namespace DVDRental.Controllers
{
    public class LoansController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoansController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Loans
        public async Task<IActionResult> Index()
        {
            var ApplicationDbContext = _context.Loan.Include(l => l.DVDCopy).Include(l => l.LoanType).Include(l => l.Member);
            return View(await ApplicationDbContext.ToListAsync());
        }

        // GET: Loans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = await _context.Loan
                .Include(l => l.DVDCopy)
                .Include(l => l.LoanType)
                .Include(l => l.Member)
                .FirstOrDefaultAsync(m => m.LoanNumber == id);
            if (loan == null)
            {
                return NotFound();
            }

            return View(loan);
        }

        // GET: Loans/Create
        public IActionResult Create()
        {
            ViewData["CopyNumber"] = new SelectList(_context.DVDCopy, "CopyNumber", "CopyNumber");
            //ViewData["LoanTypeNumber"] = new SelectList(_context.LoanType, "LoanTypeNumber", "LoanTypeNumber");
            ViewData["LoanType"] = new SelectList(_context.LoanType, "Loantype", "Loantype");
            ViewData["MemberNumber"] = new SelectList(_context.Member, "MemberNumber", "MemberNumber");
            return View();
        }

        // POST: Loans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LoanNumber,Loantype,CopyNumber,MemberNumber")] Loan loan)
        {
            string loanTypeStr = HttpContext.Request.Form["LoanType.Loantype"];
            LoanType id = _context.LoanType.Where(l=>l.Loantype == loanTypeStr ).FirstOrDefault();

            Member      member = _context.Member.Where(l=>l.MemberNumber == loan.MemberNumber).Include(m=>m.MembershipCategory).FirstOrDefault();
            DVDCopy cat    = _context.DVDCopy.Where(l=>l.CopyNumber == loan.CopyNumber ).Include(c=>c.DVDTitle).ThenInclude(d=>d.DVDCategory).FirstOrDefault();

            int remainingLoanCount = _context.Loan.Where(l=> l.MemberNumber == loan.MemberNumber && l.DateReturned == null).Count();

            ViewData["CopyNumber"] = new SelectList(_context.DVDCopy, "CopyNumber", "CopyNumber", loan.CopyNumber);
            ViewData["LoanType"] = new SelectList(_context.LoanType, "Loantype", "Loantype", loan.LoanType);
            ViewData["MemberNumber"] = new SelectList(_context.Member, "MemberNumber", "MemberNumber", loan.MemberNumber);

            loan.DateOut = DateTime.Now;

            loan.DateDue = DateTime.Now.AddDays(id.LoanDuration);

            if(remainingLoanCount >= member.MembershipCategory.MembershipCategoryTotalLoans)
            {
                ModelState.AddModelError(string.Empty, "Member has too many DVD unreturned!");
                return View();
            }

            if (DateTime.Today.Year  - member.MemberDateOfBirth.Year < 18 && cat.DVDTitle.DVDCategory.AgeRestricted)
            {
                ModelState.AddModelError(string.Empty, "Member is underaged for this DVD");
                return View();
            }

            loan.LoanTypeNumber = id.LoanTypeNumber;
            if (ModelState.IsValid)
            {
                _context.Add(loan);
                await _context.SaveChangesAsync();
                ModelState.AddModelError("success", "Loan Created Successfully your total Price" + id.LoanDuration * cat.DVDTitle.StandardCharge);
                return View();
            }
            return View(loan);
        }

        // GET: Loans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = await _context.Loan.FindAsync(id);
            if (loan == null)
            {
                return NotFound();
            }
            ViewData["CopyNumber"] = new SelectList(_context.DVDCopy, "CopyNumber", "CopyNumber", loan.CopyNumber);
            ViewData["LoanTypeNumber"] = new SelectList(_context.LoanType, "LoanTypeNumber", "LoanTypeNumber", loan.LoanTypeNumber);
            ViewData["MemberNumber"] = new SelectList(_context.Member, "MemberNumber", "MemberNumber", loan.MemberNumber);
            return View(loan);
        }

        // POST: Loans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LoanNumber,DateOut,DateDue,DateReturned,LoanTypeNumber,CopyNumber,MemberNumber")] Loan loan)
        {
            if (id != loan.LoanNumber)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoanExists(loan.LoanNumber))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CopyNumber"] = new SelectList(_context.DVDCopy, "CopyNumber", "CopyNumber", loan.CopyNumber);
            ViewData["LoanTypeNumber"] = new SelectList(_context.LoanType, "LoanTypeNumber", "LoanTypeNumber", loan.LoanTypeNumber);
            ViewData["MemberNumber"] = new SelectList(_context.Member, "MemberNumber", "MemberNumber", loan.MemberNumber);
            return View(loan);
        }

        // GET: Loans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = await _context.Loan
                .Include(l => l.DVDCopy)
                .Include(l => l.LoanType)
                .Include(l => l.Member)
                .FirstOrDefaultAsync(m => m.LoanNumber == id);
            if (loan == null)
            {
                return NotFound();
            }

            return View(loan);
        }

        // POST: Loans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loan = await _context.Loan.FindAsync(id);
            _context.Loan.Remove(loan);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoanExists(int id)
        {
            return _context.Loan.Any(e => e.LoanNumber == id);
        }
    }
}
