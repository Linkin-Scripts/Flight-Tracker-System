using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Avalonia.Controls;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Tiling;
using NetTopologySuite.Geometries;
using Flight_Tracker_System.Models;
using Flight_Tracker_System.ViewModels;

namespace Flight_Tracker_System.Views;

public partial class RouteVisualisation : UserControl
{
    public RouteVisualisation()
    {
        InitializeComponent();
        // Creates the map
        MyMapControl.Map.Layers.Add(OpenStreetMap.CreateTileLayer());
        // we are assigning the data context of the view to the viewmodel.
        // this has to be done like this because the view gets contructed first.
        // then we subscribe to its propertychanged event so the view gets notified whenever a property changes in the view model.
        DataContextChanged += (_, _) =>
        {
            if (DataContext is RouteVisualisationViewModel vm)
                vm.PropertyChanged += OnViewModelPropertyChanged;
        };
    }
    // PropertyChanged fires for every property change on the ViewModel. 
    // This line ignores everything except SelectedAirport which is the only property care when the user picks a different airport.         
    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(RouteVisualisationViewModel.SelectedAirport)) return;

        var vm = (RouteVisualisationViewModel)DataContext!;
        if (vm.SelectedAirport is not null)
            UpdateRouteLayer(vm.SelectedAirport, vm.DestinationAirports);
        else
            UpdateRouteLayer(new() {Name = "null"}, []);
    }

    private void UpdateRouteLayer(Airport origin, List<Airport> destinations)
    {
        var map = MyMapControl.Map;


        var existing = map.Layers.FirstOrDefault(l => l.Name == "Routes");
        if (existing != null) map.Layers.Remove(existing);
        if (origin.Name == "null" && destinations.Count == 0)
            return;


        // The coordinates in flights.json are regular GPS longitude/latitude (WGS84). 
        // Mapsui's map uses Web Mercator (EPSG:3857), which is measured in metres. SphericalMercator.FromLonLat converts between the two. 
        // ox/oy are the origin airport's position in map units.
        var (ox, oy) = SphericalMercator.FromLonLat(origin.Longitude, origin.Latitude);

        // Creates the little orange circle in the map for the departure airport (the selected one) 
        var features = new List<IFeature>();

        features.Add(new PointFeature(new MPoint(ox, oy))
        {
            Styles = { new SymbolStyle { Fill = new Brush(Color.Orange), SymbolScale = 0.7 } }
        });


        // Creates the destination airports dots and the lines connecting them with the origin airport
        foreach (var dest in destinations)
        {
            var (dx, dy) = SphericalMercator.FromLonLat(dest.Longitude, dest.Latitude);

            // Destination pin (blue)
            features.Add(new PointFeature(new MPoint(dx, dy))
            {
                Styles = { new SymbolStyle { Fill = new Brush(Color.CornflowerBlue), SymbolScale = 0.5 } }
            });

            // Route line
            features.Add(new GeometryFeature
            {
                Geometry = new LineString(new[] { new Coordinate(ox, oy), new Coordinate(dx, dy) }),
                Styles = { new VectorStyle { Line = new Pen(Color.CornflowerBlue, 1.5f) } }
            });
        }
        // this is the thing that saves all the features in memory. Naming it routes allows us to reference it above to delete it.
        map.Layers.Add(new MemoryLayer { Name = "Routes", Features = features });

        // Centre the map on the selected airport. The 1000 is the zoom level
        MyMapControl.Map.Navigator.CenterOnAndZoomTo(new MPoint(ox, oy), 10000);
        // tells the view to update itself in order for us to see the layer
        MyMapControl.ForceUpdate();
    }
}
