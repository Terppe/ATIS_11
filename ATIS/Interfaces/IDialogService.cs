
namespace ATIS.Interfaces;

public interface IDialogService
{
    Task ShowInfoAsync(string titleKey, string messageKey);
    Task ShowErrorAsync(string titleKey, string messageKey);
    Task<bool> ShowYesNoAsync(string titleKey, string messageKey);
    Task<bool> ShowOkCancelAsync(string titleKey, string messageKey);
    Task<bool> ShowConfirmAsync(string titleKey, string messageKey);
    Task<bool> ShowDetailDialogAsync(FrameworkElement content, string titleKey);
}
