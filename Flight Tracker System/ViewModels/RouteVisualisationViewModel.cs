using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flight_Tracker_System.Models;

namespace Flight_Tracker_System.ViewModels;

public partial class RouteVisualisationViewModel : ViewModelBase
{
    private readonly List<Airport> _allAirports;
    private readonly List<Flight> _allFlights;

    public ObservableCollection<Airport> FilteredAirports { get; } = [];

    [ObservableProperty]
    private Airport? _selectedAirport;

    [ObservableProperty]
    private string _searchText = string.Empty;

    public List<Airport> DestinationAirports { get; private set; } = [];

    public RouteVisualisationViewModel(List<Airport> airports, List<Flight> flights)
    {
        _allAirports = airports;
        _allFlights = flights;
        RefreshFilteredAirports();
    }

    [RelayCommand]
    private void ClearSelection()
    {
        SelectedAirport = null;
        SearchText = string.Empty;
    }

    partial void OnSearchTextChanged(string value)
    {
        RefreshFilteredAirports();
    }

    private void RefreshFilteredAirports()
    {
        FilteredAirports.Clear();

        var results = string.IsNullOrWhiteSpace(SearchText)
            ? _allAirports
            : _allAirports.Where(a =>
                a.Name.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase) ||
                a.IataCode.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase) ||
                a.City.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase));

        foreach (var airport in results)
            FilteredAirports.Add(airport);
    }

    partial void OnSelectedAirportChanged(Airport? value)
    {
        if (value is null) { DestinationAirports = []; return; }

        var destCodes = _allFlights
            .Where(f => f.DepartureAirport == value.IataCode)
            .Select(f => f.ArrivalAirport)
            .Distinct()
            .ToHashSet();

        DestinationAirports = _allAirports.Where(a => destCodes.Contains(a.IataCode)).ToList();
        OnPropertyChanged(nameof(DestinationAirports));
    }
}
