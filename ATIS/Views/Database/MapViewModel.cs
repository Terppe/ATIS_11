using System.ComponentModel;
using Mapsui;
using Mapsui.Tiling;

namespace ATIS.Views.Database;

public class MapViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    private Map _map;
    public Map Map
    {
        get => _map;
        set
        {
            _map = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Map)));
        }
    }

    public MapViewModel()
    {
        Map = new Map();
        Map.Layers.Add(OpenStreetMap.CreateTileLayer());
    }
}
