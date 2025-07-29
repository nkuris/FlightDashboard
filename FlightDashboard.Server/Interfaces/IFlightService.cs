using FlightDashboard.Server.Models;

namespace FlightDashboard.Server.Interfaces
{
    public interface IFlightService
    {
        public Task<List<Flight>> GetFlightsAsync();
       
        public Task<Flight> UpdateFlightAsync(Flight entity);
        public Task<bool> DeleteFlightAsync(string id);

        public Task<Flight> GetFlightByIdAsync(string id);
        public Task<Flight> AddFlightAsync(FlightCreate flightCreate);

        


    }
}
