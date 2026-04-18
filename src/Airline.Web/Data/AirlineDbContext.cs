using Airline.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Airline.Web.Data
{
    public class AirlineDbContext : IdentityDbContext<AirlineUser>
    {
        public AirlineDbContext(DbContextOptions<AirlineDbContext> options)
            : base(options)
        {
        }

        public DbSet<Flight> Flights { get; set; }
        public DbSet<Aircraft> Aircrafts { get; set; }
        public DbSet<Passenger> Passengers { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Airport> Airports { get; set; }
		public DbSet<Seat> Seats { get; set; } = null!;
       
	   protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<Ticket>().Property(t => t.Price).HasPrecision(18, 2);
    modelBuilder.Entity<Airport>().HasKey(a => a.Code);

    // ЯВНАЯ НАСТРОЙКА СВЯЗИ 1: Аэропорт вылета
    modelBuilder.Entity<Flight>()
        .HasOne(f => f.DepartureAirport)
        .WithMany()
        .HasForeignKey(f => f.DepartureAirportCode)
        .HasPrincipalKey(a => a.Code)
        .OnDelete(DeleteBehavior.Restrict);

    // ЯВНАЯ НАСТРОЙКА СВЯЗИ 2: Аэропорт прилёта
    modelBuilder.Entity<Flight>()
        .HasOne(f => f.ArrivalAirport)
        .WithMany()
        .HasForeignKey(f => f.ArrivalAirportCode)
        .HasPrincipalKey(a => a.Code)
        .OnDelete(DeleteBehavior.Restrict);

modelBuilder.Entity<Seat>()
    .HasOne(s => s.Flight)
    .WithMany()
    .HasForeignKey(s => s.FlightId)
    .OnDelete(DeleteBehavior.Cascade);
   // Связь Ticket -> Flight
modelBuilder.Entity<Ticket>()
    .HasOne(t => t.Flight)
    .WithMany() // У рейса может быть много билетов
    .HasForeignKey(t => t.FlightId)
    .OnDelete(DeleteBehavior.Cascade);

// Связь Ticket -> Passenger
modelBuilder.Entity<Ticket>()
    .HasOne(t => t.Passenger)
    .WithMany() // У пассажира может быть много билетов
    .HasForeignKey(t => t.PassengerId)
    .OnDelete(DeleteBehavior.Restrict);

    modelBuilder.Entity<Flight>().HasIndex(f => f.FlightNumber).IsUnique();
    modelBuilder.Entity<Airport>().HasIndex(a => a.Code).IsUnique();

    // Seed-данные
    modelBuilder.Entity<Airport>().HasData(
        new Airport { Code = "SVO", Name = "Шереметьево", City = "Москва", Country = "Россия" },
        new Airport { Code = "LED", Name = "Пулково", City = "Санкт-Петербург", Country = "Россия" },
        new Airport { Code = "KZN", Name = "Кольцово", City = "Казань", Country = "Россия" }
    );
}
    }
}