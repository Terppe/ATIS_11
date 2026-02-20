
namespace ATIS.Interfaces;

public interface IDialogService
{
    Task<bool?> ShowDialogAsync(FrameworkElement content, string title);
    Task<bool?> ShowConfirmAsync(string title, string message);
}
