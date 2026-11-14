using ATIS.Interfaces;

namespace ATIS.Infrastructure;

public static class AppData
{
    public static IDataService Data
    {
        get
        {
            if (App.Services != null)
            {
                return App.Services.GetRequiredService<IDataService>();
            }
            throw new InvalidOperationException("Services are not available.");
        }
    }
}
