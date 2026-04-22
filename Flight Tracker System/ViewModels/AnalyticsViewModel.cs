using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Flight_Tracker_System.Models;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace Flight_Tracker_System.ViewModels;

public partial class AnalyticsViewModel : ViewModelBase
{
    //Store data from JSON
    private readonly List<Flight> _flights;
    private readonly List<Airport> _airports;
    //Chart 1: Top Airlines
    public ISeries[] AirlineSeries { get; private set; } = [];
    public string[] AirlineLabels { get; private set; } = [];
    //Chart 2: Busiest Routes
    public ISeries[] RouteSeries { get; private set; } = [];
    public string[] RouteLabels { get; private set; } = [];
    //Chart 3: Flights per Day
    public ISeries[] FlightsPerDaySeries { get; private set; } = [];
    public string[] DayLabels { get; private set; } = [];
    //Constructor gets data from MainWindowViewModel
    public AnalyticsViewModel(List<Flight> flights, List<Airport> airports)
    {
        _flights = flights;
        _airports = airports;

        LoadAnalytics();
    }
    //Runs all analytics
    private void LoadAnalytics()
    {
        GenerateAirlineAnalytics();
        GenerateRouteAnalytics();
        GenerateFlightsPerDayAnalytics();
    }
    //1. Top Airlines
    private void GenerateAirlineAnalytics()
    {
    //Group flights by airline and count them
        var result = _flights
            .GroupBy(f => f.AirlineName)
            .Select(g => new
            {
                Airline = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToList();

    //Labels for X-axis
        AirlineLabels = result.Select(x => x.Airline).ToArray();

    //Values for chart
        AirlineSeries = new ISeries[]
        {
            new ColumnSeries<int>
            {
                Values = result.Select(x => x.Count).ToArray()
            }
        };

        OnPropertyChanged(nameof(AirlineSeries));
        OnPropertyChanged(nameof(AirlineLabels));
    }
 // 2. Busiest Routes
    private void GenerateRouteAnalytics()
    {
        // Group by route (CPH → AMS etc.)
        var result = _flights
            .GroupBy(f => $"{f.DepartureAirport} → {f.ArrivalAirport}")
            .Select(g => new
            {
                Route = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToList();
        RouteLabels = result.Select(x => x.Route).ToArray();
        RouteSeries = new ISeries[]
        {
            new ColumnSeries<int>
            {
                Values = result.Select(x => x.Count).ToArray()
            }
        };
        OnPropertyChanged(nameof(RouteSeries));
        OnPropertyChanged(nameof(RouteLabels));
    }
    //3. Flights per Day
    private void GenerateFlightsPerDayAnalytics()
    {
    //Group by date (ignore time)
        var result = _flights
            .GroupBy(f => f.ScheduledDeparture.Date)
            .Select(g => new
            {
                Date = g.Key,
                Count = g.Count()
            })
            .OrderBy(x => x.Date)
            .ToList();
        DayLabels = result.Select(x => x.Date.ToShortDateString()).ToArray();
        FlightsPerDaySeries = new ISeries[]
        {
            new LineSeries<int>
            {
                Values = result.Select(x => x.Count).ToArray()
            }
        };
        OnPropertyChanged(nameof(FlightsPerDaySeries));
        OnPropertyChanged(nameof(DayLabels));
    }
}