namespace BACKEND_READ_FILE.Models
{
    public class Passenger
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }

    public class FlightInfo
    {
        public string FlightNumber { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string FareClass { get; set; } = string.Empty;
        public string DepartureTimeAndPlace { get; set; } = string.Empty;
        public string ArrivalTimeAndPlace { get; set; } = string.Empty;
    }

    public class ParsedTicket
    {
        public string? FileName { get; set; }
        public string? BookingCode { get; set; }
        public string? ContactPhone { get; set; }
        public string? ContactEmail { get; set; }
        public string? BookingDate { get; set; }
        public string? BookerName { get; set; }
        public List<Passenger> Passengers { get; set; } = new();
        public List<FlightInfo> Flights { get; set; } = new();
    }

    public class ParsePathRequest
    {
        public string Path { get; set; } = string.Empty;
    }
}
