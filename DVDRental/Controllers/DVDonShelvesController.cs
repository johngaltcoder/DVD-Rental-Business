using Microsoft.AspNetCore.Mvc;
using DVDRental.Models;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;

namespace DVDRental.Controllers
{
    [Authorize(Roles = "User,Assistant, Manager")]
    public class DVDonShelvesController: Controller
    {

        private readonly ApplicationDbContext _context;

        public DVDonShelvesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string searchString)
        {
            var results = from dt in _context.Set<DVDTitle>()
                          join cm in _context.Set<CastMember>()
                          on dt.DVDNumber equals cm.DVDNumber
                          join dc in _context.Set<DVDCopy>()
                          .Where(c => _context.Loan.Any(l => (c.CopyNumber == l.CopyNumber && l.DateReturned != null)))
                          on dt.DVDNumber equals dc.DVDNumber
                          join a in _context.Set<Actor>().
                          Where(x => x.ActorSurname.Contains(searchString))
                          on cm.ActorNumber equals a.ActorNumber
                          group new { dt, cm, dc } by new { dt.DVDNumber, dt.title, a.ActorSurname }
                          into grp
                          select new DVDOnShelves
                          {
                              DVDNumber = grp.Key.DVDNumber,
                              Dvd_count = grp.Count(),
                              ActorSurname = grp.Key.ActorSurname,
                              Title = grp.Key.title,
                          };

            ViewData["results"] = results;
            return View();
        }
    }
}
