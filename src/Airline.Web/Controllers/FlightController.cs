using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Airline.Web.Dtos;
using Airline.Web.Models;
using Airline.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Airline.Web.Controllers
{
    public class FlightController : Controller
    {
        private readonly IFlightService _flightService;
        private readonly Airline.Web.Data.AirlineDbContext _context;

        public FlightController(IFlightService flightService, Airline.Web.Data.AirlineDbContext context)
        {
            _flightService = flightService;
            _context = context;
        }

        [HttpGet]
        public IActionResult Search() => View();

        [HttpPost]
        public async Task<IActionResult> Search(string from, string to, DateTime date)
        {
            var flights = await _flightService.SearchFlightsAsync(from, to, date);
            return View("Results", flights);
        }

        public async Task<IActionResult> ViewSeats(int flightId)
        {
            var flight = await _context.Flights.FindAsync(flightId);
            if (flight == null) return NotFound();

            var seats = await _context.Seats
                .Where(s => s.FlightId == flightId)
                .ToListAsync();

            if (!seats.Any())
            {
                var newSeats = new List<Seat>();
                for (int row = 1; row <= 15; row++)
                {
                    foreach (var letter in new[] { "A", "B", "C", "D" })
                    {
                        newSeats.Add(new Seat
                        {
                            FlightId = flightId,
                            SeatNumber = $"{row}{letter}",
                            Status = "Available"
                        });
                    }
                }
                await _context.Seats.AddRangeAsync(newSeats);
                await _context.SaveChangesAsync();
                seats = newSeats;
            }

            ViewBag.FlightInfo = $"{flight.FlightNumber} ({flight.DepartureAirportCode} → {flight.ArrivalAirportCode})";
            return View(seats);
        }

        [HttpPost]
        public async Task<IActionResult> Book(int flightId, string passengerName, string seatNumber)
        {
            try
            {
                var seat = await _context.Seats
                    .FirstOrDefaultAsync(s => s.FlightId == flightId && s.SeatNumber == seatNumber);

                if (seat == null || seat.Status != "Available")
                {
                    TempData["Error"] = $"Место {seatNumber} уже занято!";
                    return RedirectToAction("ViewSeats", new { flightId });
                }

                seat.Status = "Booked";
                await _context.SaveChangesAsync();

                var ticket = await _flightService.BookTicketAsync(flightId, passengerName, seatNumber);
                TempData["Success"] = $"Успешно! Место {seatNumber} забронировано.";
                return RedirectToAction("ViewSeats", new { flightId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ошибка: {ex.Message}";
                return RedirectToAction("ViewSeats", new { flightId });
            }
        }
    }
}