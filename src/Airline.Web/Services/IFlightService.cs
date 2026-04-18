using Airline.Web.Dtos;
using Airline.Web.Models;

namespace Airline.Web.Services
{
    public interface IFlightService
    {
        Task<List<FlightSearchDto>> SearchFlightsAsync(string fromCode, string toCode, DateTime date);
        Task<Ticket> BookTicketAsync(int flightId, string passengerName, string seatNumber);
    }
}