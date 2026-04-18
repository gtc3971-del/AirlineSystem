using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Airline.Web.Data;
using Airline.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Airline.Web.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly AirlineDbContext _context;
        private readonly UserManager<AirlineUser> _userManager;

        public AdminController(AirlineDbContext context, UserManager<AirlineUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task<AirlineUser?> GetCurrentUserAsync() => await _userManager.GetUserAsync(User);

        // === РЕЙСЫ ===
        public async Task<IActionResult> Flights()
        {
            var user = await GetCurrentUserAsync();
            if (user == null || (user.UserRole != UserRole.Admin && user.UserRole != UserRole.Dispatcher)) return Forbid();
            return View(await _context.Flights.Include(f => f.DepartureAirport).Include(f => f.ArrivalAirport).ToListAsync());
        }

        public async Task<IActionResult> CreateFlight()
        {
            var user = await GetCurrentUserAsync();
            if (user == null || (user.UserRole != UserRole.Admin && user.UserRole != UserRole.Dispatcher)) return Forbid();
            ViewBag.Airports = await _context.Airports.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFlight(Flight flight)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Flights.AnyAsync(f => f.FlightNumber == flight.FlightNumber))
                {
                    ModelState.AddModelError("FlightNumber", "⚠️ Рейс с таким номером уже существует!");
                }
                else
                {
                    flight.Status = "Scheduled";
                    _context.Flights.Add(flight);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Рейс создан!";
                    return RedirectToAction(nameof(Flights));
                }
            }
            ViewBag.Airports = await _context.Airports.ToListAsync();
            return View(flight);
        }

        public async Task<IActionResult> EditFlight(int? id)
        {
            if (id == null) return NotFound();
            var flight = await _context.Flights.FindAsync(id);
            if (flight == null) return NotFound();
            ViewBag.Airports = await _context.Airports.ToListAsync();
            return View(flight);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFlight(int id, Flight flight)
        {
            if (id != flight.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Flights.Update(flight);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Flights));
            }
            ViewBag.Airports = await _context.Airports.ToListAsync();
            return View(flight);
        }

        public async Task<IActionResult> DeleteFlight(int? id)
        {
            if (id == null) return NotFound();
            var flight = await _context.Flights.FindAsync(id);
            return flight == null ? NotFound() : View(flight);
        }

        [HttpPost, ActionName("DeleteFlight")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight != null) { _context.Flights.Remove(flight); await _context.SaveChangesAsync(); }
            return RedirectToAction(nameof(Flights));
        }

        // === ПАРК ВС ===
        public async Task<IActionResult> Aircrafts()
        {
            if ((await GetCurrentUserAsync())?.UserRole != UserRole.Admin) return Forbid();
            return View(await _context.Aircrafts.ToListAsync());
        }

        public IActionResult CreateAircraft() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAircraft(Aircraft aircraft)
        {
            if (ModelState.IsValid)
            {
                _context.Aircrafts.Add(aircraft);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Aircrafts));
            }
            return View(aircraft);
        }

        public async Task<IActionResult> EditAircraft(int? id)
        {
            if (id == null) return NotFound();
            var a = await _context.Aircrafts.FindAsync(id);
            return a == null ? NotFound() : View(a);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAircraft(int id, Aircraft aircraft)
        {
            if (id != aircraft.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Aircrafts.Update(aircraft);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Aircrafts));
            }
            return View(aircraft);
        }

        public async Task<IActionResult> DeleteAircraft(int? id)
        {
            if (id == null) return NotFound();
            var a = await _context.Aircrafts.FindAsync(id);
            return a == null ? NotFound() : View(a);
        }

        [HttpPost, ActionName("DeleteAircraft")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAircraftConfirmed(int id)
        {
            var a = await _context.Aircrafts.FindAsync(id);
            if (a != null) { _context.Aircrafts.Remove(a); await _context.SaveChangesAsync(); }
            return RedirectToAction(nameof(Aircrafts));
        }

        // === ОТЧЁТЫ ===
        public async Task<IActionResult> Reports()
        {
            var user = await GetCurrentUserAsync();
            if (user?.UserRole != UserRole.Admin && user?.UserRole != UserRole.Dispatcher) return Forbid();
            return View();
        }

        public async Task<IActionResult> PassengerFlow(DateTime? start, DateTime? end)
        {
            var s = start ?? DateTime.Now.AddDays(-30);
            var e = end ?? DateTime.Now;
            var data = await _context.Tickets.Include(t => t.Flight)
                .Where(t => t.Flight != null && t.PurchaseDate >= s && t.PurchaseDate <= e)
                .GroupBy(t => new { Route = t.Flight!.DepartureAirportCode + " → " + t.Flight!.ArrivalAirportCode })
                .Select(g => new { Route = g.Key.Route, Sold = g.Count(), Revenue = Math.Round(g.Sum(t => t.Price), 2) })
                .OrderByDescending(x => x.Sold).ToListAsync();
            ViewBag.Start = s.ToString("yyyy-MM-dd");
            ViewBag.End = e.ToString("yyyy-MM-dd");
            return View(data);
        }

        public async Task<IActionResult> FlightLoad()
        {
            var flights = await _context.Flights.Where(f => f.DepartureTime >= DateTime.Now.AddDays(-30)).ToListAsync();
            var report = new List<object>();
            foreach (var f in flights)
            {
                var sold = await _context.Tickets.CountAsync(t => t.FlightId == f.Id);
                var total = f.AvailableSeats + sold;
                report.Add(new {
                    FlightNumber = f.FlightNumber,
                    Route = $"{f.DepartureAirportCode} → {f.ArrivalAirportCode}",
                    Departure = f.DepartureTime.ToString("dd.MM HH:mm"),
                    Sold = sold, Total = total,
                    Load = total > 0 ? Math.Round((decimal)sold / total * 100, 1) : 0
                });
            }
            return View(report);
        }
    }
}