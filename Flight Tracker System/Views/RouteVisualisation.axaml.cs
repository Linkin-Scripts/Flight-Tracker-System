using Avalonia.Controls;
using Mapsui.Tiling;

namespace Flight_Tracker_System.Views;

public partial class RouteVisualisation : UserControl
{
    public RouteVisualisation()
    {
        InitializeComponent();
        MyMapControl.Map?.Layers.Add(OpenStreetMap.CreateTileLayer());
    }
}