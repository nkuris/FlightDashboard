using FlightDashboard.Server.Services;

namespace FlightDashboard.Server.Models
{
    public class Flight
    {
        public int Id { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public string DepartureAirport { get; set; } = string.Empty;
        public string ArrivalAirport { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        // Additional properties can be added as needed

        public Flight()
        {
            // Default constructor for serialization
        }

       
        public Flight(int id, string flightNumber, string departureAirport, string arrivalAirport, DateTime departureTime,
            DateTime arrivalTime, string status)
        {
            Id = id;
            FlightNumber = flightNumber;
            DepartureAirport = departureAirport;
            ArrivalAirport = arrivalAirport;
            DepartureTime = departureTime;
            ArrivalTime = arrivalTime;
        }
    }

    public class FlightDTO
    {
        public int Id { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public string DepartureAirport { get; set; } = string.Empty;
        public string ArrivalAirport { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public string Status { get; set; }

        public FlightDTO() { }

        public FlightDTO(Flight flight)
        {
            Id = flight.Id;
            FlightNumber = flight.FlightNumber;
            DepartureAirport = flight.DepartureAirport;
            ArrivalAirport = flight.ArrivalAirport;
            DepartureTime = flight.DepartureTime;
            ArrivalTime = flight.ArrivalTime;
            Status = FlightService.GetFlightStatus(flight.DepartureTime, DateTime.UtcNow);
        }
    }

    

    public class FlightSearchCriteria
    {
        public string? FlightNumber { get; set; }
        public string? DepartureAirport { get; set; }
        public string? ArrivalAirport { get; set; }
        public DateTime? DepartureTimeFrom { get; set; }
        public DateTime? DepartureTimeTo { get; set; }
        public DateTime? ArrivalTimeFrom { get; set; }
        public DateTime? ArrivalTimeTo { get; set; }
        public string? Status { get; set; }
    }

    public class FlightUpdate
    {
        public int Id { get; set; }
        public string? FlightNumber { get; set; }
        public string? DepartureAirport { get; set; }
        public string? ArrivalAirport { get; set; }
        public DateTime? DepartureTime { get; set; }
        public DateTime? ArrivalTime { get; set; }
        public string? Status { get; set; }
    }
    public class FlightCreate
    {
        public string FlightNumber { get; set; } = string.Empty;
        public string DepartureAirport { get; set; } = string.Empty;
        public string ArrivalAirport { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class FlightDelete
    {
        public int Id { get; set; }
    }

    public class FlightSearchResult
    {
        public List<Flight> Flights { get; set; } = new List<Flight>();
        public int TotalCount { get; set; }
    }
}
