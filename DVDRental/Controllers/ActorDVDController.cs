using Microsoft.AspNetCore.Mvc;
using DVDRental.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace DVDRental.Controllers
{
    [Authorize(Roles = "User,Assistant, Manager")]
    public class ActorDVDController : Controller
    {

        private readonly ApplicationDbContext  _context;

        public ActorDVDController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string searchString)
        {
            var ApplicationDbContext = _context.CastMember.Include(d=>d.Actor).Include(d=>d.DVDTitle);

            var actors = from a in ApplicationDbContext select a;
            if (!String.IsNullOrEmpty(searchString))
            {
                actors = actors.Where(d => d.Actor.ActorSurname.Contains(searchString));
            }
            return View(actors.ToList());
        }
    }
}
