using System.Collections.Generic;
using System.Linq;
using Flight_Tracker_System.Models;
using Flight_Tracker_System.ViewModels;
using Xunit;

namespace FlightTrackerTests;

public class RouteVisualisationViewModelTests
{
    private static List<Airport> MakeAirports() =>
    [
        new Airport { IataCode = "CPH", Name = "Copenhagen Airport", City = "Copenhagen" },
        new Airport { IataCode = "AAR", Name = "Aarhus Airport",     City = "Aarhus" },
        new Airport { IataCode = "STN", Name = "London Stansted",    City = "London" },
    ];

    private static List<Flight> MakeFlights() =>
    [
        new Flight { DepartureAirport = "CPH", ArrivalAirport = "AAR" },
        new Flight { DepartureAirport = "CPH", ArrivalAirport = "STN" },
        new Flight { DepartureAirport = "AAR", ArrivalAirport = "CPH" },
    ];

    [Fact]
    public void InitialState_AllAirportsVisible()
    {
        var vm = new RouteVisualisationViewModel(MakeAirports(), MakeFlights());

        Assert.Equal(3, vm.FilteredAirports.Count);
    }

    [Fact]
    public void SearchText_FiltersByAirportName()
    {
        var vm = new RouteVisualisationViewModel(MakeAirports(), MakeFlights());

        vm.SearchText = "Copenhagen";

        Assert.Single(vm.FilteredAirports);
        Assert.Equal("CPH", vm.FilteredAirports[0].IataCode);
    }

    [Fact]
    public void SearchText_FiltersByIataCode()
    {
        var vm = new RouteVisualisationViewModel(MakeAirports(), MakeFlights());

        vm.SearchText = "STN";

        Assert.Single(vm.FilteredAirports);
        Assert.Equal("STN", vm.FilteredAirports[0].IataCode);
    }

    [Fact]
    public void SearchText_FiltersByCity()
    {
        var vm = new RouteVisualisationViewModel(MakeAirports(), MakeFlights());

        vm.SearchText = "London";

        Assert.Single(vm.FilteredAirports);
        Assert.Equal("STN", vm.FilteredAirports[0].IataCode);
    }

    [Fact]
    public void SelectedAirport_SetsDestinationAirports()
    {
        var airports = MakeAirports();
        var vm = new RouteVisualisationViewModel(airports, MakeFlights());

        vm.SelectedAirport = airports[0]; // CPH departs to AAR and STN

        Assert.Equal(2, vm.DestinationAirports.Count);
        Assert.Contains(vm.DestinationAirports, a => a.IataCode == "AAR");
        Assert.Contains(vm.DestinationAirports, a => a.IataCode == "STN");
    }

    [Fact]
    public void ClearSelection_ResetsAirportAndSearchText()
    {
        var airports = MakeAirports();
        var vm = new RouteVisualisationViewModel(airports, MakeFlights());
        vm.SelectedAirport = airports[0];
        vm.SearchText = "something";

        vm.ClearSelectionCommand.Execute(null);

        Assert.Null(vm.SelectedAirport);
        Assert.Equal(string.Empty, vm.SearchText);
    }
}

public class FlightInfoViewModelTests
{
    [Fact]
    public void InitialState_ShowsAllFlights()
    {
        var vm = new FlightInfoViewModel();

        Assert.True(vm.MatchingFlightCount > 0);
        Assert.True(vm.HasMatchingFlights);
        Assert.False(vm.HasNoMatchingFlights);
    }

    [Fact]
    public void FilterByStatus_ReducesResults()
    {
        var vm = new FlightInfoViewModel();
        int allCount = vm.MatchingFlightCount;

        // pick a real status that isn't the first option ("All statuses")
        string firstRealStatus = vm.StatusOptions.Skip(1).First();
        vm.SelectedStatus = firstRealStatus;

        Assert.True(vm.MatchingFlightCount <= allCount);
        Assert.All(vm.FilteredFlights, f => Assert.Equal(firstRealStatus, f.Status));
    }

    [Fact]
    public void ResetStatusFilter_RestoresAllFlights()
    {
        var vm = new FlightInfoViewModel();
        int allCount = vm.MatchingFlightCount;

        vm.SelectedStatus = vm.StatusOptions.Skip(1).First();
        vm.SelectedStatus = vm.StatusOptions.First(); // "All statuses"

        Assert.Equal(allCount, vm.MatchingFlightCount);
    }

    [Fact]
    public void FilterByAirport_ShowsOnlyDeparturesFromThatAirport()
    {
        var vm = new FlightInfoViewModel();

        // pick a real airport (skip "All airports" at index 0)
        var airport = vm.Airports.Skip(1).First();
        vm.SelectedAirport = airport;

        Assert.All(vm.FilteredFlights, f =>
            Assert.Equal(airport.IataCode, f.DepartureAirport));
    }

    [Fact]
    public void FilteredFlights_SortedByScheduledDeparture()
    {
        var vm = new FlightInfoViewModel();

        var departures = vm.FilteredFlights.Select(f => f.ScheduledDeparture).ToList();
        var sorted = departures.OrderBy(d => d).ToList();

        Assert.Equal(sorted, departures);
    }
}
