namespace ATIS.Views.Base;

public interface IBasePageLifecycle
{
    Task OnPageLoadedAsync();
}
