using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flight_Tracker_System.Data;
using Flight_Tracker_System.Models;

namespace Flight_Tracker_System.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public List<Flight> Flights { get; set; }
    public List<Airport> Airports { get; set; }

    public RouteVisualisationViewModel RouteVisualisationVM { get; set; }
    public FlightInfoViewModel FlightInfoVM { get; set; }
    public AnalyticsViewModel AnalyticsVM { get; set; }

    private readonly List<int> _history = [0];
    private int _historyIndex = 0;
    private bool _navigatingHistory = false;

    [ObservableProperty]
    private int selectedTabIndex = 0;

    public bool CanGoBack => _historyIndex > 0;
    public bool CanGoForward => _historyIndex < _history.Count - 1;

    public MainWindowViewModel()
    {
        var data = JsonParser.Deserialise<FlightData>();
        Flights = data.Flights;
        Airports = data.Airports;

        RouteVisualisationVM = new RouteVisualisationViewModel(Airports, Flights);
        FlightInfoVM = new FlightInfoViewModel();
        AnalyticsVM = new AnalyticsViewModel(Flights, Airports);
    }

    partial void OnSelectedTabIndexChanged(int value)
    {
        if (_navigatingHistory) return;

        // Trim forward history when user navigates to a new tab manually
        if (_historyIndex < _history.Count - 1)
            _history.RemoveRange(_historyIndex + 1, _history.Count - _historyIndex - 1);

        // Only push if different from current
        if (_history[_historyIndex] != value)
        {
            _history.Add(value);
            _historyIndex++;
        }

        NotifyNavigation();
    }

    [RelayCommand(CanExecute = nameof(CanGoBack))]
    private void GoBack()
    {
        if (!CanGoBack) return;
        _navigatingHistory = true;
        _historyIndex--;
        SelectedTabIndex = _history[_historyIndex];
        _navigatingHistory = false;
        NotifyNavigation();
    }

    [RelayCommand(CanExecute = nameof(CanGoForward))]
    private void GoForward()
    {
        if (!CanGoForward) return;
        _navigatingHistory = true;
        _historyIndex++;
        SelectedTabIndex = _history[_historyIndex];
        _navigatingHistory = false;
        NotifyNavigation();
    }

    private void NotifyNavigation()
    {
        OnPropertyChanged(nameof(CanGoBack));
        OnPropertyChanged(nameof(CanGoForward));
        GoBackCommand.NotifyCanExecuteChanged();
        GoForwardCommand.NotifyCanExecuteChanged();
    }
}
