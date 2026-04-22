using System.Collections.Generic;
using Flight_Tracker_System.Data;
using Flight_Tracker_System.Models;

namespace Flight_Tracker_System.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public List<Flight> Flights { get; set; }
    public List<Airport> Airports { get; set; }

    public RouteVisualisationViewModel RouteVisualisationVM { get; set; }
    public FlightInfoViewModel FlightInfoVM { get; set; }
    public AnalyticsViewModel AnalyticsVM { get; set; }

    public MainWindowViewModel()
    {
        var data = JsonParser.Deserialise<FlightData>();
        Flights = data.Flights;
        Airports = data.Airports;

        RouteVisualisationVM = new RouteVisualisationViewModel(Airports, Flights);
        FlightInfoVM = new FlightInfoViewModel();
        AnalyticsVM = new AnalyticsViewModel(Flights, Airports);
    }
}