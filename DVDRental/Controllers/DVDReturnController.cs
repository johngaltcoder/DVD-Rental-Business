using Microsoft.AspNetCore.Mvc;
using DVDRental.Models;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System;
using Microsoft.AspNetCore.Authorization;

namespace DVDRental.Controllers
{
    [Authorize(Roles = "Manager,Assistant")]
    public class DVDReturnController: Controller
    {

        private readonly ApplicationDbContext _context;

        public DVDReturnController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int id)
        {
            Loan loan = _context.Loan.Where(l => l.CopyNumber == id).Include(l=>l.DVDCopy).ThenInclude(c=>c.DVDTitle).FirstOrDefault();
            if(loan != null)
            {
                if(loan.DateReturned == null)
                {
                    loan.DateReturned = DateTime.Now;
                    _context.Update(loan);
                    await _context.SaveChangesAsync();
                    TimeSpan? days = loan.DateReturned - loan.DateDue;
                    if(days.Value.Days > 0)
                    {
                        ViewData["message"] ="Dvd returned successfully your total cost is "+ days.Value.Days * loan.DVDCopy.DVDTitle.PenaltyCharge ;
                        return View();
                    }
                    ViewData["message"] = "Dvd returned successfully before due date";
                    return View();
                }else{
                    ViewData["message"] = "Dvd already returned";
                    return View();
                }
            }
            ViewData["message"] = "DVD dosen'te exist";
            return View();
        }

    }
}
