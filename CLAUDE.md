# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

University homework assignment (AOOP course, SDU) — a Flight Tracker System for Sønderborg Airport. Desktop GUI built with .NET 10 and Avalonia Framework, following MVVM. Flight data is loaded from `flights.json`.

## Build & Run

```bash
# Build
dotnet build

# Run
dotnet run --project "Flight Tracker System/Flight Tracker System.csproj"

# Run tests (once test project exists)
dotnet test

# Run a single test class
dotnet test --filter "FullyQualifiedName~ClassName"
```

## Architecture

**Pattern:** MVVM (Model-View-ViewModel) enforced throughout.

**Key layers:**
- `Views/` — Avalonia XAML (`.axaml`) + code-behind (`.axaml.cs`). Code-behind is minimal; logic belongs in ViewModels.
- `ViewModels/` — All UI state and business logic. ViewModels inherit `ViewModelBase` (which extends CommunityToolkit's `ObservableObject`).
- `Models/` — Data structures (Flight, Airport, Route, etc.). Currently empty — needs to be populated.
- `Assets/` — Static resources.

**ViewLocator** (`ViewLocator.cs`) resolves ViewModel → View automatically by convention: `FooViewModel` maps to `FooView`. Register new views by following this naming convention; no extra wiring needed.

**Bindings:** Compiled bindings are enabled by default (`AvaloniaUseCompiledBindingsByDefault=true`). Avalonia's default data annotation validation is disabled to avoid conflict with CommunityToolkit.

## Three Required Views

The UI must support three views (multi-view navigation):

1. **Route Visualization** — map with flight connections from a selected airport (use Mapsui v5+).
2. **Airport Flight Info** — flight list for a selected airport, filterable by flight status.
3. **Analytics** — three LINQ-powered charts (use LiveCharts2).

Plus: data export (at least one format for flight data, one for analytics).

## Required Libraries (not yet added to project)

| Library | Purpose |
|---|---|
| `Mapsui` v5+ | Map rendering and route visualization |
| `LiveCharts2` | Analytics charts |
| `xUnit` | Unit tests |
| `Avalonia.Headless` | Headless Avalonia testing for ViewModel/UI tests |

Add via: `dotnet add package <PackageName>`

## Data

Flight data lives in `flights.json` (file not yet present — needs to be created or provided). Models should reflect the structure of that file.

## Testing Requirements

Tests must cover ViewModel logic and LINQ query results. Use xUnit + Avalonia.Headless. Create a separate test project in the solution.

## Grading Notes

For full credit, the project also requires:
- A `README.md` in the root with project structure diagram, setup instructions, and component documentation.
- A UI mockup (can be a photo of a hand-drawn sketch).
- Working LINQ analytics queries (must be explainable).
