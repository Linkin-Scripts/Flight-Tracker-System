using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Flight_Tracker_System.Data;
using Flight_Tracker_System.Models;

namespace Flight_Tracker_System.ViewModels;

public partial class FlightInfoViewModel : ViewModelBase
{
	private const string AllStatuses = "All statuses";
	private static readonly Airport AllAirports = new()
	{
		Name = "All airports"
	};
	private readonly List<Flight> allFlights;

	[ObservableProperty]
	private ObservableCollection<Airport> airports = [];

	[ObservableProperty]
	private ObservableCollection<string> statusOptions = [AllStatuses];

	[ObservableProperty]
	private ObservableCollection<Flight> filteredFlights = [];

	[ObservableProperty]
	private Airport? selectedAirport;

	[ObservableProperty]
	private string selectedStatus = AllStatuses;

	[ObservableProperty]
	private int matchingFlightCount;

	[ObservableProperty]
	private bool hasMatchingFlights;

	[ObservableProperty]
	private bool hasNoMatchingFlights;

	public FlightInfoViewModel()
	{
		FlightData flightData = JsonParser.Deserialise<FlightData>();
		allFlights = flightData.Flights;

		Airports = new ObservableCollection<Airport>(new[] { AllAirports }
			.Concat(flightData.Airports)
			.OrderBy(airport => airport.Name)
			.ThenBy(airport => airport.IataCode));

		StatusOptions = new ObservableCollection<string>(
			new[] { AllStatuses }
				.Concat(flightData.Flights
					.Select(flight => flight.Status)
					.Where(status => !string.IsNullOrWhiteSpace(status))
					.Distinct(StringComparer.OrdinalIgnoreCase)
					.OrderBy(status => status)));

		SelectedAirport = AllAirports;
		UpdateFilteredFlights(flightData.Flights);
	}

	partial void OnSelectedAirportChanged(Airport? value)
	{
		RefreshFlights();
	}

	partial void OnSelectedStatusChanged(string value)
	{
		RefreshFlights();
	}

	private void RefreshFlights()
	{
		UpdateFilteredFlights(allFlights);
	}

	private void UpdateFilteredFlights(IEnumerable<Flight> flights)
	{
		IEnumerable<Flight> filtered = flights;

		if (SelectedAirport is not null && !ReferenceEquals(SelectedAirport, AllAirports))
		{
			filtered = filtered.Where(flight =>
				string.Equals(flight.DepartureAirport, SelectedAirport.IataCode, StringComparison.OrdinalIgnoreCase));
		}

		if (!string.Equals(SelectedStatus, AllStatuses, StringComparison.OrdinalIgnoreCase))
		{
			filtered = filtered.Where(flight =>
				string.Equals(flight.Status, SelectedStatus, StringComparison.OrdinalIgnoreCase));
		}

		List<Flight> orderedFlights = filtered
			.OrderBy(flight => flight.ScheduledDeparture)
			.ToList();

		FilteredFlights = new ObservableCollection<Flight>(orderedFlights);
		MatchingFlightCount = orderedFlights.Count;
		HasMatchingFlights = orderedFlights.Count > 0;
		HasNoMatchingFlights = orderedFlights.Count == 0;
	}
}
