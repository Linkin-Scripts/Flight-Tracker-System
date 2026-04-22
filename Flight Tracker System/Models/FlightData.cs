using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Flight_Tracker_System.Models;

public class FlightData
{
    [JsonPropertyName("airports")]
    public List<Airport> Airports { get; set; } = [];

    [JsonPropertyName("flights")]
    public List<Flight> Flights { get; set; } = [];
}
