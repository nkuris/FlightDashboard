using FlightDashboard.Server.Data;
using FlightDashboard.Server.Interfaces;
using FlightDashboard.Server.Models;

namespace FlightDashboard.Server.Services
{
    public class FlightService : IFlightService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<FlightService> _logger;
        private readonly FlightDbContext _dbContext;

        public FlightService(IConfiguration configuration, ILogger<FlightService> logger, FlightDbContext dbContext)
        {
            _configuration = configuration;
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<Flight> AddFlightAsync(FlightCreate flightCreate)
        {
            try
            {
                if (flightCreate == null)
                {
                    _logger.LogWarning("FlightCreate object is null");
                    throw new ArgumentNullException(nameof(flightCreate), "FlightCreate object cannot be null");
                }
                var flight = new Flight
                {
                    FlightNumber = flightCreate.FlightNumber,
                    DepartureAirport = flightCreate.DepartureAirport,
                    ArrivalAirport = flightCreate.ArrivalAirport,
                    DepartureTime = flightCreate.DepartureTime,
                    ArrivalTime = flightCreate.ArrivalTime,
                    
                };
                _logger.LogInformation("Adding new flight {@Flight}", flight);
                var result = await _dbContext.AddFlightAsync(flight);
                _logger.LogInformation("Flight added with Id {FlightId}", result.Id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding flight {@Flight}", flightCreate);
                throw;
            }
        }

        public async Task<bool> DeleteFlightAsync(string id)
        {
            try
            {
                _logger.LogInformation("Deleting flight with Id {FlightId}", id);
                if (!int.TryParse(id, out var flightId))
                {
                    _logger.LogWarning("Invalid flight id: {FlightId}", id);
                    return false;
                }

                var result = await _dbContext.DeleteFlightAsync(flightId);
                if (result)
                    _logger.LogInformation("Flight with Id {FlightId} deleted", flightId);
                else
                    _logger.LogWarning("Flight with Id {FlightId} not found for deletion", flightId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting flight with Id {FlightId}", id);
                throw;
            }
        }

        public static string GetFlightStatus(DateTime departureTime, DateTime currentTime)
        {
            TimeSpan timeUntilDeparture = departureTime - currentTime;
            TimeSpan timeSinceDeparture = currentTime - departureTime;

            if (timeUntilDeparture.TotalMinutes > 30)
            {
                return "Scheduled";
            }
            else if (timeUntilDeparture.TotalMinutes <= 30 && timeUntilDeparture.TotalMinutes > 0)
            {
                return "Boarding";
            }
            else if (timeSinceDeparture.TotalMinutes >= 0 && timeSinceDeparture.TotalMinutes <= 60)
            {
                return "Departed";
            }
            else if (timeSinceDeparture.TotalMinutes > 60)
            {
                return "Landed";
            }
            else
            {
                return "Invalid Time"; // Just in case something goes wrong with time logic
            }
        }
        public async Task<Flight> GetFlightByIdAsync(string id)
        {
            try
            {
                _logger.LogInformation("Getting flight by Id {FlightId}", id);
                if (string.IsNullOrEmpty(id))
                {
                    _logger.LogWarning("Flight id is null or empty");
                    return null;
                }

                if (!int.TryParse(id, out var flightId))
                {
                    _logger.LogWarning("Invalid flight id: {FlightId}", id);
                    return null;
                }

                var flight = await _dbContext.GetFlightByIdAsync(flightId);
                if (flight == null)
                    _logger.LogWarning("Flight with Id {FlightId} not found", flightId);
                else
                    _logger.LogInformation("Flight with Id {FlightId} retrieved", flightId);

                return flight;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting flight by Id {FlightId}", id);
                throw;
            }
        }

        public async Task<List<Flight>> GetFlightsAsync()
        {
            try
            {
                _logger.LogInformation("Getting all flights");
                var flights = await _dbContext.GetAllFlightsAsync();
                _logger.LogInformation("{Count} flights retrieved", flights.Count);
                return flights;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all flights");
                throw;
            }
        }

        public async Task<Flight> UpdateFlightAsync(Flight entity)
        {
            try
            {
                _logger.LogInformation("Updating flight with Id {FlightId}", entity.Id);
                var result = await _dbContext.UpdateFlightAsync(entity);
                if (result)
                    _logger.LogInformation("Flight with Id {FlightId} updated", entity.Id);
                else
                    _logger.LogWarning("Flight with Id {FlightId} not found for update", entity.Id);

                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating flight with Id {FlightId}", entity.Id);
                throw;
            }
        }
    }
}
