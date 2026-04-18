using Airline.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Data
{
    public static class DbSeeder
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AirlineDbContext>();

            // Ждём, пока БД будет готова
            if (context.Database.GetPendingMigrations().Any())
            {
                await context.Database.MigrateAsync();
            }

            // Если рейсы уже есть — ничего не делаем
            if (await context.Flights.AnyAsync())
                return;

            // === ТЕСТОВЫЕ ДАННЫЕ ===

            // 1. Самолёты
            var aircrafts = new[]
            {
                new Aircraft { TailNumber = "RA-73001", Model = "Boeing 737-800", TotalSeats = 180, TechnicalStatus = "Active" },
                new Aircraft { TailNumber = "RA-96012", Model = "Airbus A320", TotalSeats = 160, TechnicalStatus = "Active" },
                new Aircraft { TailNumber = "RA-89005", Model = "Sukhoi Superjet 100", TotalSeats = 98, TechnicalStatus = "Active" }
            };
            await context.Aircrafts.AddRangeAsync(aircrafts);
            await context.SaveChangesAsync();

            // 2. Рейсы (на ближайшие 7 дней)
            var today = DateTime.Today;
            var flights = new[]
            {
                new Flight
                {
                    FlightNumber = "SU-1001",
                    DepartureTime = today.AddHours(8),
                    ArrivalTime = today.AddHours(10),
                    DepartureAirportCode = "SVO",
                    ArrivalAirportCode = "LED",
                    AvailableSeats = 45,
                    Status = "Scheduled"
                },
                new Flight
                {
                    FlightNumber = "SU-1002",
                    DepartureTime = today.AddHours(14),
                    ArrivalTime = today.AddHours(16),
                    DepartureAirportCode = "SVO",
                    ArrivalAirportCode = "LED",
                    AvailableSeats = 12,
                    Status = "Scheduled"
                },
                new Flight
                {
                    FlightNumber = "SU-2001",
                    DepartureTime = today.AddHours(10),
                    ArrivalTime = today.AddHours(13),
                    DepartureAirportCode = "SVO",
                    ArrivalAirportCode = "KZN",
                    AvailableSeats = 78,
                    Status = "Scheduled"
                },
                new Flight
                {
                    FlightNumber = "SU-3001",
                    DepartureTime = today.AddDays(1).AddHours(9),
                    ArrivalTime = today.AddDays(1).AddHours(11),
                    DepartureAirportCode = "LED",
                    ArrivalAirportCode = "SVO",
                    AvailableSeats = 33,
                    Status = "Scheduled"
                }
            };
            await context.Flights.AddRangeAsync(flights);
            await context.SaveChangesAsync();

            // 3. Тестовый пользователь-пассажир (если нужно)
            // (Опционально, можно добавить позже через регистрацию)
        }
    }
}