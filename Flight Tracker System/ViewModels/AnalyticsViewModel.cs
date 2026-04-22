using System;
using System.Collections.Generic;
using System.Linq;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Flight_Tracker_System.Models;

namespace Flight_Tracker_System.ViewModels;

public class AnalyticsViewModel : ViewModelBase
{
    private readonly List<Flight> _flights;
    private readonly List<Airport> _airports;

    // Chart 1
    private ISeries[] _airlineSeries = [];
    public ISeries[] AirlineSeries
    {
        get => _airlineSeries;
        set => SetProperty(ref _airlineSeries, value);
    }

    private IList<Axis> _airlineXAxes = [new Axis()];
    public IList<Axis> AirlineXAxes
    {
        get => _airlineXAxes;
        set => SetProperty(ref _airlineXAxes, value);
    }

    // Chart 2
    private ISeries[] _routeSeries = [];
    public ISeries[] RouteSeries
    {
        get => _routeSeries;
        set => SetProperty(ref _routeSeries, value);
    }

    private IList<Axis> _routeXAxes = [new Axis()];
    public IList<Axis> RouteXAxes
    {
        get => _routeXAxes;
        set => SetProperty(ref _routeXAxes, value);
    }

    // Chart 3
    private ISeries[] _flightsPerDaySeries = [];
    public ISeries[] FlightsPerDaySeries
    {
        get => _flightsPerDaySeries;
        set => SetProperty(ref _flightsPerDaySeries, value);
    }

    private IList<Axis> _dayXAxes = [new Axis()];
    public IList<Axis> DayXAxes
    {
        get => _dayXAxes;
        set => SetProperty(ref _dayXAxes, value);
    }

    public AnalyticsViewModel(List<Flight> flights, List<Airport> airports)
    {
        _flights = flights ?? new List<Flight>();
        _airports = airports ?? new List<Airport>();

        LoadAnalytics();
    }

    private void LoadAnalytics()
    {
        GenerateAirlineAnalytics();
        GenerateRouteAnalytics();
        GenerateFlightsPerDayAnalytics();
    }

    // 1. Top Airlines
    private void GenerateAirlineAnalytics()
    {
        var result = _flights
            .GroupBy(f => f.AirlineName ?? "Unknown")
            .Select(g => new { Airline = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToList();

        AirlineXAxes = [new Axis { Labels = result.Select(x => x.Airline).ToArray() }];

        AirlineSeries = new ISeries[]
        {
            new ColumnSeries<int>
            {
                Values = result.Select(x => x.Count).ToArray()
            }
        };
    }

    // 2. Busiest Routes
    private void GenerateRouteAnalytics()
    {
        var result = _flights
            .GroupBy(f => $"{f.DepartureAirport ?? "UNK"} → {f.ArrivalAirport ?? "UNK"}")
            .Select(g => new { Route = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToList();

        RouteXAxes = [new Axis { Labels = result.Select(x => x.Route).ToArray() }];

        RouteSeries = new ISeries[]
        {
            new ColumnSeries<int>
            {
                Values = result.Select(x => x.Count).ToArray()
            }
        };
    }

    // 3. Flights per Day
    private void GenerateFlightsPerDayAnalytics()
    {
        var result = _flights
            .GroupBy(f => f.ScheduledDeparture.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .OrderBy(x => x.Date)
            .ToList();

        DayXAxes = [new Axis { Labels = result.Select(x => x.Date.ToShortDateString()).ToArray() }];

        FlightsPerDaySeries = new ISeries[]
        {
            new LineSeries<int>
            {
                Values = result.Select(x => x.Count).ToArray()
            }
        };
    }
}