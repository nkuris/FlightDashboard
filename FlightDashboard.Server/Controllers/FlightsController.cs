using FlightDashboard.Server.Interfaces;
using FlightDashboard.Server.Models;
using FlightDashboard.Server.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace FlightDashboard.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private readonly ILogger<FlightsController> _logger;
        private readonly IFlightService _flightService;
        private readonly IHubContext<FlightHub> _hubContext;

        public FlightsController(
            IFlightService flightService,
            ILogger<FlightsController> logger,
            IHubContext<FlightHub> hubContext)
        {
            _logger = logger;
            _flightService = flightService;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<List<FlightDTO>> GetFlightsAsync()
        {
            List<FlightDTO> res = new List<FlightDTO    >();
            try
            {
                List<Flight> flights = await _flightService.GetFlightsAsync();
                res = flights.Select(f => new FlightDTO(f)).ToList();
                
            }
            catch (Exception ex)
            {
                res = null;
            }
            return res;
        }

        [HttpPut]
        public async Task<Flight> UpdateFlightAsync([FromBody] Flight entity)
        {
            var updatedFlight = await _flightService.UpdateFlightAsync(entity);
            await _hubContext.Clients.All.SendAsync("FlightUpdated", updatedFlight);
            return updatedFlight;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<bool> DeleteFlightAsync(string id)
        {
            var result = await _flightService.DeleteFlightAsync(id);
            if (result)
            {
                await _hubContext.Clients.All.SendAsync("FlightDeleted", id);
            }
            return result;
        }

        [HttpGet("{id}")]
        public async Task<Flight> GetFlightByIdAsync(string id)
        {
            return await _flightService.GetFlightByIdAsync(id);
        }

        [HttpPost]
        public async Task<FlightDTO> AddFlightAsync([FromBody] FlightCreate flightCreate)
        {
            Flight newFlight = await _flightService.AddFlightAsync(flightCreate);
            FlightDTO newFlightDto = new FlightDTO(newFlight);
            await _hubContext.Clients.All.SendAsync("FlightAdded", newFlightDto);
            return newFlightDto;
        }
    }
}
