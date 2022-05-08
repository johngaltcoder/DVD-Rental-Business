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
    public class DVDTitlesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DVDTitlesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DVDTitles
        public async Task<IActionResult> Index()
        {
            var ApplicationDbContext = _context.DVDTitle.Include(d => d.DVDCategory).Include(d => d.Producer).Include(d => d.Studio);
            return View(await ApplicationDbContext.ToListAsync());
        }

        // GET: DVDTitles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dVDTitle = await _context.DVDTitle
                .Include(d => d.DVDCategory)
                .Include(d => d.Producer)
                .Include(d => d.Studio)
                .FirstOrDefaultAsync(m => m.DVDNumber == id);
            if (dVDTitle == null)
            {
                return NotFound();
            }

            return View(dVDTitle);
        }
        // GET: DVDTitles/Create
        public IActionResult Create()
        {
            ViewData["Category"] = new SelectList(_context.DVDCategory, "CategoryDescription", "CategoryDescription");
            ViewData["Producer"] = new SelectList(_context.Producer, "ProducerName", "ProducerName");
            ViewData["Studio"] = new SelectList(_context.Studio, "StudioName", "StudioName");
            ViewData["actors"] = new SelectList(_context.Actor, "ActorFirstName", "ActorFirstName");

            return View();
        }

        // POST: DVDTitles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DVDNumber,title,DateReleased,StandardCharge,PenaltyCharge,CategoryNumber,StudioNumber,ProducerNumber")] DVDTitle dVDTitle)
        {
            if (ModelState.ErrorCount <=1)
            {
                //Category
                String dvdCategory = HttpContext.Request.Form["DVDCategory.CategoryDescription"][0];

                dVDTitle.CategoryNumber = _context.DVDCategory.Where(d => d.CategoryDescription == dvdCategory).FirstOrDefault().CategoryNumber;
                //Producer 
                String pName = HttpContext.Request.Form["Producer.ProducerName"][0];
                int producerNumber = _context.Producer.Where(p=>p.ProducerName == pName).FirstOrDefault().ProducerNumber;
                if(HttpContext.Request.Form["Producer.ProducerName"][1] != "")
                {
                    pName = HttpContext.Request.Form["Producer.ProducerName"][1];
                    Producer newP = new() { ProducerName = pName};
                    await _context.AddAsync(newP);
                    await _context.SaveChangesAsync();
                    Producer oldP = _context.Producer.Where(p=>p.ProducerName == pName).FirstOrDefault(); 

                    producerNumber = oldP.ProducerNumber;
                }

                //Studio
                String sName = HttpContext.Request.Form["Studio.StudioName"][0];
                int studioNumber = _context.Studio.Where(p=>p.StudioName== sName).FirstOrDefault().StudioNumber;
                if(HttpContext.Request.Form["Studio.StudioName"][1] != "")
                {
                    sName = HttpContext.Request.Form["Studio.StudioName"][1];
                    Studio newS = new() { StudioName = sName};
                    await _context.AddAsync(newS);
                    await _context.SaveChangesAsync();
                    Studio oldP = _context.Studio.Where(p=>p.StudioName == sName).FirstOrDefault(); 
                    studioNumber = oldP.StudioNumber;
                }
                dVDTitle.StudioNumber = studioNumber;
                dVDTitle.ProducerNumber= producerNumber;

                

                _context.Add(dVDTitle);
                await _context.SaveChangesAsync();

                //Actors
                if (HttpContext.Request.Form.ContainsKey("ActorList[0].ActorFirstName"))//Has actors
                {
                    int numActors = HttpContext.Request.Form.Count - 8;
                    int index = 0;
                    for(int i = 0; i < numActors; i++)
                    {
                        while(!HttpContext.Request.Form.ContainsKey("ActorList[" + index + "].ActorFirstName")) 
                        {
                            index++;
                        }
                        String actorName = HttpContext.Request.Form["ActorList[" + index + "].ActorFirstName"][0];
                        Actor actor = _context.Actor.Where(a=>a.ActorFirstName == actorName).FirstOrDefault();
                        if(actor != null)
                        {
                            //await _context.AddAsync(new CastMember { ActorNumber = actor.ActorNumber, DVDNumber = dVDTitle.DVDNumber });
                            _context.Database.ExecuteSqlRaw($"insert into CastMember(DVDNumber, ActorNumber) values({dVDTitle.DVDNumber}, {actor.ActorNumber});");
                            await _context.SaveChangesAsync();
                        }
                        index++;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Category"] = new SelectList(_context.DVDCategory, "CategoryDescription", "CategoryDescription", dVDTitle.DVDCategory);
            ViewData["Producer"] = new SelectList(_context.Producer, "ProducerName", "ProducerName", dVDTitle.Producer);
            ViewData["Studio"] = new SelectList(_context.Studio, "StudioName", "StudioName", dVDTitle.Studio);
            ViewData["actors"] = new SelectList(_context.Actor, "ActorFirstName", "ActorFirstName");
            return View(dVDTitle);
        }


        // GET: DVDTitles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dVDTitle = await _context.DVDTitle.FindAsync(id);
            if (dVDTitle == null)
            {
                return NotFound();
            }
            ViewData["CategoryNumber"] = new SelectList(_context.DVDCategory, "CategoryNumber", "CategoryNumber", dVDTitle.CategoryNumber);
            ViewData["ProducerNumber"] = new SelectList(_context.Producer, "ProducerNumber", "ProducerNumber", dVDTitle.ProducerNumber);
            ViewData["StudioNumber"] = new SelectList(_context.Studio, "StudioNumber", "StudioNumber", dVDTitle.StudioNumber);
            return View(dVDTitle);
        }

        // POST: DVDTitles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DVDNumber,title,DateReleased,StandardCharge,PenaltyCharge,CategoryNumber,StudioNumber,ProducerNumber")] DVDTitle dVDTitle)
        {
            if (id != dVDTitle.DVDNumber)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dVDTitle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DVDTitleExists(dVDTitle.DVDNumber))
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
            ViewData["CategoryNumber"] = new SelectList(_context.DVDCategory, "CategoryNumber", "CategoryNumber", dVDTitle.CategoryNumber);
            ViewData["ProducerNumber"] = new SelectList(_context.Producer, "ProducerNumber", "ProducerNumber", dVDTitle.ProducerNumber);
            ViewData["StudioNumber"] = new SelectList(_context.Studio, "StudioNumber", "StudioNumber", dVDTitle.StudioNumber);
            return View(dVDTitle);
        }

        // GET: DVDTitles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dVDTitle = await _context.DVDTitle
                .Include(d => d.DVDCategory)
                .Include(d => d.Producer)
                .Include(d => d.Studio)
                .FirstOrDefaultAsync(m => m.DVDNumber == id);
            if (dVDTitle == null)
            {
                return NotFound();
            }

            return View(dVDTitle);
        }

        // POST: DVDTitles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dVDTitle = await _context.DVDTitle.FindAsync(id);
            _context.DVDTitle.Remove(dVDTitle);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DVDTitleExists(int id)
        {
            return _context.DVDTitle.Any(e => e.DVDNumber == id);
        }
    }
}
