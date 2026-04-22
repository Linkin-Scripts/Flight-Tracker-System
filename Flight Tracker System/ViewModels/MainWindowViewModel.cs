using Flight_Tracker_System.Data;
using Flight_Tracker_System.Models;

namespace Flight_Tracker_System.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public RouteVisualisationViewModel routeVisualisationViewModel { get; }
    public FlightInfoViewModel flightInfoViewModel { get; }
    public AnalyticsViewModel analyticsViewModel { get; }

    public MainWindowViewModel()
    {
        FlightData data = JsonParser.Deserialise<FlightData>();

        routeVisualisationViewModel = new(data.Airports, data.Flights);
        flightInfoViewModel = new();
        analyticsViewModel = new();
    }
}
