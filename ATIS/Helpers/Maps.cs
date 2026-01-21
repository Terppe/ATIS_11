using ATIS.Infrastructure;
using ATIS.Models;
using ATIS.Services;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Tiling;

namespace ATIS.Helpers;

public interface Maps
{
    //private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    //public Tbl87GeographicViewModel ViewModel { get; }
    //private Tbl87Geographic SingleGeographic { get; set; }
    //private Tbl69FiSpecies SingleSpecies { get; set; }
    protected TblCountry SingleCountry { get; set; }
    //private static readonly AtisDbContext Context = new();
    //private string _search;

    ////  private readonly int _count = 0;

    //private double centerOf1LongX = 0;
    //private double centerOf1LatiY = 0;
    //private int zoom = 0;

    public Task<Map> CreateMapAsync(double centerOf1LongX, double centerOf1LatiY, int zoom, Tbl87Geographic graphSelect)
    {
        var map = new Mapsui.Map();
        map.Layers.Add(OpenStreetMap.CreateTileLayer());

        if (graphSelect.Latitude != 0)
        {
            //    //var long1 = Convert.ToDouble(Longitude1Geographic.Text);
            //    //var lat1 = Convert.ToDouble(Latitude1Geographic.Text);
            //    //var long2 = Convert.ToDouble(Longitude2Geographic.Text);
            //    //var lat2 = Convert.ToDouble(Latitude2Geographic.Text);

            //    //var long3 = Convert.ToDouble(Longitude3Geographic.Text);
            //    //var lat3 = Convert.ToDouble(Latitude3Geographic.Text);
            //    //var long4 = Convert.ToDouble(Longitude4Geographic.Text);
            //    //var lat4 = Convert.ToDouble(Latitude4Geographic.Text);

            //    //var res = Convert.ToDouble(ZoomLevelGeographic.Text);
            //    //var lineStringLayer = CreateLineStringLayer(long1, lat1,  long2,  lat2, CreateLineStringStyle());
            //    //map.Layers.Add(lineStringLayer);
            //    //var lineStringLayer1 = CreateLineStringLayer(long3, lat3, long4, lat4, CreateLineStringStyle());
            //    //map.Layers.Add(lineStringLayer1);

            var centerOf2 = new MPoint(centerOf1LongX, centerOf1LatiY);
            var sphericalMercatorCoordinate2 = SphericalMercator.FromLonLat(centerOf2.X, centerOf2.Y).ToMPoint();
            map.Layers.Add(CreatePointLayer(centerOf1LongX, centerOf1LatiY));
      //      map.Home = n => n.CenterOnAndZoomTo(sphericalMercatorCoordinate2, n.Resolutions[(int)zoom]);

            return Task.FromResult(map);
        }
        {
            //aus TblCountriesList Daten
            SingleCountry = AppData.Data.TblCountries.FirstOrDefault(x => x.Name == graphSelect.Country);

            if (SingleCountry == null)
            {
                SingleCountry = AppData.Data.TblCountries.FirstOrDefault(x => x.Name == graphSelect.Continent);
                if (SingleCountry != null)
                {
                    centerOf1LongX = SingleCountry.Longitude;
                    centerOf1LatiY = SingleCountry.Latitude;
                    zoom = (int)SingleCountry.ZoomLevel;
                }
            }
            else
            {
                centerOf1LongX = SingleCountry.Longitude;
                centerOf1LatiY = SingleCountry.Latitude;
                zoom = (int)SingleCountry.ZoomLevel;
            }

        }
        var centerOf = new MPoint(centerOf1LongX, centerOf1LatiY);
        var sphericalMercatorCoordinate1 = SphericalMercator.FromLonLat(centerOf.X, centerOf.Y).ToMPoint();
        map.Layers.Add(CreatePointLayer(centerOf1LongX, centerOf1LatiY));
      //  map.Home = n => n.CenterOnAndZoomTo(sphericalMercatorCoordinate1, n.Resolutions[(int)zoom]);

        return Task.FromResult(map);
    }
    private MemoryLayer CreatePointLayer(double centerOf1LongX, double centerOf1LatiY)
    {
        return new MemoryLayer
        {
            Name = "Points",
            Features = [new PointFeature(SphericalMercator.FromLonLat(centerOf1LongX, centerOf1LatiY).ToMPoint())]
            // Style = SymbolStyles.CreatePinStyle(symbolScale: 0.7)
        };
    }



    
}
