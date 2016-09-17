using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tracker.Admin.Web.Data;
using Tracker.Admin.Web.Models;
using Microsoft.EntityFrameworkCore;
using Tracker.Admin.Web.Models.LocationViewModels;

namespace Tracker.Admin.Web.Controllers
{
    public class LocationController : Controller
    {

        private readonly TrackerDbContext _context;

        public LocationController(TrackerDbContext context)
        {
            _context = context;
        }

        // GET: Locations
        public async Task<IActionResult> Index()
        {
            return View(await _context.Location.ToListAsync());
        }

        // GET: Locations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Location location = await _context.Location.SingleAsync(m => m.Id == id);
            if (location == null)
            {
                return NotFound();
            }

            return View(location);
        }

        // GET: Locations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Locations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Location location)
        {
            if (ModelState.IsValid)
            {
                _context.Location.Add(location);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(location);
        }

        // GET: Locations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Location location = await _context.Location.SingleAsync(m => m.Id == id);
            if (location == null)
            {
                return NotFound();
            }
            return View(location);
        }

        // POST: Locations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Location location)
        {
            if (ModelState.IsValid)
            {
                _context.Update(location);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(location);
        }

        // GET: Locations/Delete/5
        [ActionName("Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Location location = await _context.Location.SingleAsync(m => m.Id == id);
            if (location == null)
            {
                return NotFound();
            }

            return View(location);
        }

        // POST: Locations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var location = await _context.Location.SingleAsync(m => m.Id == id);
            _context.Location.Remove(location);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [ActionName("People")]
        public async Task<IActionResult> People(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int locationId = id ?? 0;

            var location = await _context.Location.SingleAsync(m => m.Id == locationId);

            var peopleinLocation = (from p in _context.Person
                join pc in _context.PersonCard on p.Id equals pc.PersonId
                join c in _context.Card on pc.CardId equals c.Id
                where _context.Movement
                          .OrderByDescending(m => m.SwipeTime)
                          .FirstOrDefault(m => string.Equals(m.CardId, c.Uid, StringComparison.OrdinalIgnoreCase)) != null && _context.Movement
                          .OrderByDescending(m => m.SwipeTime)
                          .FirstOrDefault(m => string.Equals(m.CardId, c.Uid, StringComparison.OrdinalIgnoreCase)).LocationId == locationId
                select new
                {
                    p.Id,
                    p.FirstName,
                    p.LastName,
                    CardUid = c.Uid.ToLower(),
                    SwipeTime = _context.Movement
                                    .Where(m => string.Equals(m.CardId, c.Uid, StringComparison.OrdinalIgnoreCase))
                                    .Max(m => m.SwipeTime)
                });

            var model = new LocationPeopleViewModel
            {
                LocationId = locationId,
                LocationName = location.Name,
                People = peopleinLocation.Select(p => MapToPersonViewModel(p)).ToList()
            };

           return View(model);
        }

        private static PersonInLocationViewModel MapToPersonViewModel(dynamic personMovement)
        {
            return new PersonInLocationViewModel
            {
                Id = personMovement.Id,
                FullName = $"{personMovement.FirstName} {personMovement.LastName}",
                CardUid = personMovement.CardUid,
                SwipeTime = personMovement.SwipeTime
            };
        }
    }
}