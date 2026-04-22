using System;
using System.Collections.Generic;
using System.Linq;
using Flight_Tracker_System.Models;
using Flight_Tracker_System.ViewModels;
using LiveChartsCore.SkiaSharpView;
using Xunit;

namespace FlightTrackerTests;

public class AnalyticsViewModelTests
{
    private static List<Flight> MakeFlights() =>
    [
        new Flight { AirlineName = "SAS",       DepartureAirport = "CPH", ArrivalAirport = "AAR", ScheduledDeparture = new DateTime(2024, 6, 1, 8,  0, 0) },
        new Flight { AirlineName = "SAS",       DepartureAirport = "CPH", ArrivalAirport = "AAR", ScheduledDeparture = new DateTime(2024, 6, 1, 10, 0, 0) },
        new Flight { AirlineName = "Ryanair",   DepartureAirport = "AAR", ArrivalAirport = "STN", ScheduledDeparture = new DateTime(2024, 6, 2, 12, 0, 0) },
        new Flight { AirlineName = "Ryanair",   DepartureAirport = "AAR", ArrivalAirport = "STN", ScheduledDeparture = new DateTime(2024, 6, 2, 14, 0, 0) },
        new Flight { AirlineName = "Ryanair",   DepartureAirport = "AAR", ArrivalAirport = "STN", ScheduledDeparture = new DateTime(2024, 6, 3, 9,  0, 0) },
        new Flight { AirlineName = "Lufthansa", DepartureAirport = "CPH", ArrivalAirport = "FRA", ScheduledDeparture = new DateTime(2024, 6, 3, 14, 0, 0) },
    ];

    [Fact]
    public void AirlineSeries_RankedByFlightCount()
    {
        var vm = new AnalyticsViewModel(MakeFlights(), []);

        var series = (ColumnSeries<int>)vm.AirlineSeries[0];
        var values = series.Values!.ToArray();
        var labels = vm.AirlineXAxes[0].Labels!.ToArray();

        Assert.Equal("Ryanair", labels[0]);
        Assert.Equal(3, values[0]);
        Assert.Equal("SAS", labels[1]);
        Assert.Equal(2, values[1]);
    }

    [Fact]
    public void RouteSeries_RankedByFlightCount()
    {
        var vm = new AnalyticsViewModel(MakeFlights(), []);

        var series = (ColumnSeries<int>)vm.RouteSeries[0];
        var values = series.Values!.ToArray();
        var labels = vm.RouteXAxes[0].Labels!.ToArray();

        Assert.Equal("AAR → STN", labels[0]);
        Assert.Equal(3, values[0]);
    }

    [Fact]
    public void FlightsPerDaySeries_GroupedByDate()
    {
        var vm = new AnalyticsViewModel(MakeFlights(), []);

        var series = (LineSeries<int>)vm.FlightsPerDaySeries[0];
        var values = series.Values!.ToArray();
        var labels = vm.DayXAxes[0].Labels!.ToArray();

        Assert.Equal(3, labels.Length); // 3 distinct dates
        Assert.Equal(2, values[0]);     // June 1: 2 flights
        Assert.Equal(2, values[1]);     // June 2: 2 flights
        Assert.Equal(2, values[2]);     // June 3: 2 flights
    }

    [Fact]
    public void AllSeries_EmptyWhenNoFlights()
    {
        var vm = new AnalyticsViewModel([], []);

        Assert.Empty(((ColumnSeries<int>)vm.AirlineSeries[0]).Values!);
        Assert.Empty(((ColumnSeries<int>)vm.RouteSeries[0]).Values!);
        Assert.Empty(((LineSeries<int>)vm.FlightsPerDaySeries[0]).Values!);
    }
}
