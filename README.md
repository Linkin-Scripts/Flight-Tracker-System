# Flight Tracker System

A desktop flight tracker for Sønderborg Airport built with .NET 10 and Avalonia. Developed as a university homework assignment (AOOP course, SDU).

---

## Setup

**Prerequisites:** .NET 10 SDK

```bash
# Clone and enter the repo
git clone <repo-url>
cd Flight-Tracker-System

# Build
dotnet build

# Run
dotnet run --project "Flight Tracker System/Flight Tracker System.csproj"
```

Flight data is loaded from `Flight Tracker System/Data/flights.json` at startup.

---

## Project Structure

```
Flight-Tracker-System/
├── Flight Tracker System/
│   ├── Data/
│   │   ├── flights.json          # Flight and airport data source
│   │   └── JsonParser.cs         # Deserialises flights.json into models
│   ├── Models/
│   │   ├── Airport.cs            # Airport data (IATA code, name, city, country, coordinates)
│   │   ├── Flight.cs             # Flight data (number, airline, airports, times, status)
│   │   └── FlightData.cs         # Root object containing airports and flights lists
│   ├── ViewModels/
│   │   ├── ViewModelBase.cs                  # Base class (extends ObservableObject)
│   │   ├── MainWindowViewModel.cs            # App entry point VM; owns the other VMs + history navigation
│   │   ├── RouteVisualisationViewModel.cs    # Search, airport selection, route calculation
│   │   ├── FlightInfoViewModel.cs            # Airport/status filtering, flight list
│   │   └── AnalyticsViewModel.cs             # LINQ queries + chart series for all three charts
│   ├── Views/
│   │   ├── MainWindow.axaml          # Shell: nav bar (back/forward) + tab control
│   │   ├── RouteVisualisation.axaml  # View 1: map + search controls
│   │   ├── FlighInfo.axaml           # View 2: airport selector + flight list
│   │   └── Analytics.axaml           # View 3: three LiveCharts2 bar/line charts
│   ├── Styles/
│   │   └── AppStyles.axaml           # Shared brushes and visual tokens
│   ├── ViewLocator.cs                # Resolves FooViewModel → FooView by naming convention
│   └── Flight Tracker System.csproj
└── README.md
```

---

## Components

### Models

| Class | Description |
|---|---|
| `Airport` | Represents an airport: IATA code, name, city, country, latitude, longitude. |
| `Flight` | Represents a flight: number, airline, departure/arrival airports, scheduled times, aircraft type, status. |
| `FlightData` | Root deserialization target containing lists of airports and flights. |

### Data layer

`JsonParser` reads `Data/flights.json` using `System.Text.Json` and deserialises it into a typed model. Called once at startup in `MainWindowViewModel`.

### ViewModels

**`MainWindowViewModel`**
Loads all data and creates the three child ViewModels. Also manages navigation history: every tab switch is recorded in a list; `GoBack` / `GoForward` commands traverse that list without adding duplicate entries.

**`RouteVisualisationViewModel`**
Holds the search text and selected airport. Filters the airport list reactively as the user types. When an airport is selected it computes destination airports by joining flights on departure IATA code. The map is updated from the view's code-behind using Mapsui.

**`FlightInfoViewModel`**
Loads all flights and exposes two filters: selected airport and selected status. Both are observed with `partial void OnXChanged` hooks from CommunityToolkit; any change calls `RefreshFlights()`, which re-runs the LINQ filter and updates `FilteredFlights`, `MatchingFlightCount`, and the empty-state flags.

**`AnalyticsViewModel`**
Runs three LINQ queries at construction time and produces LiveCharts2 series:

| Chart | Query | Chart type |
|---|---|---|
| Top Airlines | Groups flights by airline name, counts, takes top 5 | Bar (`ColumnSeries`) |
| Busiest Routes | Groups by `DepartureAirport → ArrivalAirport`, counts, takes top 5 | Bar (`ColumnSeries`) |
| Flights Per Day | Groups by `ScheduledDeparture.Date`, counts, orders chronologically | Line (`LineSeries`) |

### Views

**`MainWindow`** — shell window. Contains a navigation bar with ← / → buttons bound to `GoBackCommand` / `GoForwardCommand`, and a `TabControl` whose `SelectedIndex` is two-way bound to `SelectedTabIndex` in the ViewModel.

**`RouteVisualisation`** — search box + airport dropdown + Clear button + a Mapsui `MapControl` that renders the selected airport and its routes.

**`FlightInfo`** — airport `ComboBox`, status `ComboBox`, a summary card showing the match count, and a scrollable flight list rendered with `ItemsControl`.

**`Analytics`** — three `CartesianChart` controls from LiveCharts2, each bound to its own series and axis properties on `AnalyticsViewModel`.

### Navigation history

Switching tabs pushes the tab index onto a `List<int>`. The `_historyIndex` pointer moves back or forward without modifying the list; navigating to a new tab after going back trims the forward entries (same behaviour as a browser). The ← / → buttons are disabled automatically via `CanGoBack` / `CanGoForward`.

---

## Dependencies

| Package | Version | Purpose |
|---|---|---|
| Avalonia | 11.3.12 | UI framework |
| CommunityToolkit.Mvvm | 8.2.1 | MVVM helpers (`ObservableProperty`, `RelayCommand`) |
| LiveChartsCore.SkiaSharpView.Avalonia | 2.0.1 | Analytics charts |
| Mapsui.Avalonia | 5.0.2 | Map rendering |
| Mapsui.Nts | 5.0.2 | Geometry support for route lines |
