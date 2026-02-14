
namespace ATIS.Models;

public class CountryEditModel : ObservableObject
{
    private string _name = "";
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    private string _regex = "";
    public string Regex
    {
        get => _regex;
        set => SetProperty(ref _regex, value);
    }

    private string _zoomLevel = "";
    public string ZoomLevel
    {
        get => _zoomLevel;
        set => SetProperty(ref _zoomLevel, value);
    }

    private string _latitude = "";
    public string Latitude
    {
        get => _latitude;
        set => SetProperty(ref _latitude, value);
    }

    private string _longitude = "";
    public string Longitude
    {
        get => _longitude;
        set => SetProperty(ref _longitude, value);
    }
}
