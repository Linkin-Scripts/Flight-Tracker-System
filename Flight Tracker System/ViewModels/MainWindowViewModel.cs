using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Flight_Tracker_System.Models;

namespace Flight_Tracker_System.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public List<Flight> Flights { get; set; }
    public List<Airport> Airports { get; set; }

    public AnalyticsViewModel AnalyticsVM { get; set; }

    public MainWindowViewModel()
    {
        Flights = LoadFlights();
        Airports = LoadAirports();

        AnalyticsVM = new AnalyticsViewModel(Flights, Airports);
    }

    private List<Flight> LoadFlights()
    {
        var json = File.ReadAllText("flights.json");
        return JsonSerializer.Deserialize<List<Flight>>(json) ?? new List<Flight>();
    }

    private List<Airport> LoadAirports()
    {
        var json = File.ReadAllText("airports.json");
        return JsonSerializer.Deserialize<List<Airport>>(json) ?? new List<Airport>();
    }
}