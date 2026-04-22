using Flight_Tracker_System.Views;

namespace Flight_Tracker_System.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public RouteVisualisationViewModel routeVisualisationViewModel { get; } 
    public FlightInfoViewModel flightInfoViewModel { get; }
    public AnalyticsViewModel analyticsViewModel { get; } 

    private RouteVisualisation _routeVisualisation { get; } 
    private FlightInfo _flightInfo { get; }
    private Analytics _analytics { get; }

    public MainWindowViewModel()
    {
        routeVisualisationViewModel = new();
        flightInfoViewModel = new();
        analyticsViewModel = new();

        _routeVisualisation = new(){ DataContext = routeVisualisationViewModel};
        _flightInfo = new() { DataContext = flightInfoViewModel };
        _analytics = new() { DataContext = analyticsViewModel };
    }

}
