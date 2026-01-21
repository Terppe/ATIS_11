using ATIS.Interfaces;

namespace ATIS.Infrastructure;

public static class AppData
{
    public static IDataService Data =>
        App.Services.GetRequiredService<IDataService>();
}
