using System.ComponentModel;
using System.Diagnostics;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI;

//    Tbl81ImageEditControl Skriptdatum:  13.09.2026  10:32    

namespace ATIS.Views.Modules.Tbl81Image;

public sealed partial class Tbl81ImageEditControl : UserControl
{
    public Tbl81ImageViewModel? ViewModel => DataContext as Tbl81ImageViewModel;

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl81ImageEditControl()
    {
        InitializeComponent();
        SelectedTabIndex = 0;
        Loaded += Tbl81ImageEditControl_Loaded;
        Unloaded += Tbl81ImageEditControl_Unloaded;
    }

    // ---------------------------------------------------------
    //      DEPENDENCY PROPERTY: ITEM
    // ---------------------------------------------------------

    public Models.Tbl81Image? Item
    {
        get => (Models.Tbl81Image?)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    public static readonly DependencyProperty ItemProperty =
        DependencyProperty.Register(
            nameof(Item),
            typeof(Models.Tbl81Image),
            typeof(Tbl81ImageEditControl),
            new PropertyMetadata(null, OnItemChanged));

    private static void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (Tbl81ImageEditControl)d;
        var item = e.NewValue as Models.Tbl81Image;

        control.ApplyItem(item);
    }

    // ---------------------------------------------------------                    
    // APPLY ITEM
    // ---------------------------------------------------------                   

    private void ApplyItem(Models.Tbl81Image? newItem)
    {
        // ⭐ WICHTIG:
        // KEIN DataContext-Wechsel!
        // KEIN Boss-Workflow!
        // KEIN Parent-Workflow!
        //
        // Bindings im XAML zeigen direkt auf Item.XXX
        // Deshalb bleibt diese Methode LEER.
    }

    // ---------------------------------------------------------
    // VALIDATION HELPER
    // ---------------------------------------------------------

    private void UpdateValidationState(TextBox box, bool hasError)
    {
        if (box == null)
            return;

        if (hasError)
        {
            VisualStateManager.GoToState(box, "Invalid", true);
            return;
        }

        if (box.FocusState == FocusState.Pointer || box.FocusState == FocusState.Keyboard)
            VisualStateManager.GoToState(box, "Focused", true);
        else
            VisualStateManager.GoToState(box, "Normal", true);
    }

    // ---------------------------------------------------------                   
    // ValidYear VALIDATION
    // ---------------------------------------------------------                   

    private void ValidYearBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(ValidYearBox, Item?.HasValidYearError ?? false);
    }

    private void ValidYearBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(ValidYearBox, Item?.HasValidYearError ?? false);
    }


    private void ComboImageMimeType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var cmb = sender as ComboBox;
        if (cmb == null)
        {
            return;
        }

        var text = ((ComboBox)sender).SelectedItem as string;
        switch (text)
        {
            case "":    //neu
                break;
            case "jpg":
                break;
            case "png":
                break;
            case "bmp":
                break;
            case "tif":
                break;
            case "tiff":
                break;
            case "gif":
                break;
            case "ico":
                break;
            case "jpeg":
                break;
            case "webp":
                break;
            case "wmf":
                break;
            case "wmv":
                break;
            case "mpg":
                break;
            case "mp4":
                break;
            case "avi":
                break;
            case "mov":
                break;
            case "swf":
                break;
            case "flv":
                break;
        }

        //var cmb = sender as ComboBox;
        //if (cmb != null)
        //{
        //    handle = !cmb.IsDropDownOpen;
        //}

        //Handle();
    }

    // ---------------------------------------------------------                   
    // DETAIL TAB VALIDATION
    // ---------------------------------------------------------                   
    private int SelectedTabIndex = 0;

    //private void DetailTabButton_Click(object sender, RoutedEventArgs e)
    //{
    //    if (sender is Button button &&
    //        button.Tag is string tagText &&
    //        int.TryParse(tagText, out int index))
    //    {
    //        SelectedTabIndex = index;
    //        UpdateTabButtonStates();
    //        UpdateDetailContentVisibility();
    //    }
    //}
    //private void SetTabButtonStyle(Button button, Border underline, bool isSelected)
    //{
    //    button.Background = new SolidColorBrush(Colors.Transparent);
    //    button.FontWeight = isSelected ? FontWeights.SemiBold : FontWeights.Normal;
    //    underline.Background = new SolidColorBrush(isSelected ? Colors.DodgerBlue : Colors.Transparent);
    //}

    private void DpShotDate_OnGotFocus(object sender, RoutedEventArgs e)
    {
        ShotDateBorder.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 103, 192));
        ShotDateBorder.BorderThickness = new Thickness(2);
    }

    private void DpShotDate_OnLostFocus(object sender, RoutedEventArgs e)
    {
        ShotDateBorder.BorderBrush = (Brush)Application.Current.Resources["AtisTextBoxBorderBrush"];
        ShotDateBorder.BorderThickness = new Thickness(1);
    }

    // ---------------------------------------------------------
    // Image/Video/Audio
    // ---------------------------------------------------------
    private async void Tbl81ImageEditControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (VideoPlayer.MediaPlayer == null)
            VideoPlayer.SetMediaPlayer(new MediaPlayer());

        VideoPlayer.MediaPlayer.MediaFailed -= MediaPlayer_MediaFailed;
        VideoPlayer.MediaPlayer.MediaFailed += MediaPlayer_MediaFailed;

        if (ViewModel is INotifyPropertyChanged npc)
        {
            npc.PropertyChanged -= ViewModel_PropertyChanged;
            npc.PropertyChanged += ViewModel_PropertyChanged;
        }

        await RefreshMediaAsync();
    }
    private void Tbl81ImageEditControl_Unloaded(object sender, RoutedEventArgs e)
    {
        if (ViewModel is INotifyPropertyChanged npc)
        {
            npc.PropertyChanged -= ViewModel_PropertyChanged;
        }

        if (VideoPlayer.MediaPlayer != null)
        {
            VideoPlayer.MediaPlayer.MediaFailed -= MediaPlayer_MediaFailed;
        }

        ClearVideoPlayer();
    }
    private async Task RefreshMediaAsync()
    {
        ClearVideoPlayer();

        var type = ViewModel.Item?.ImageMimeType;

        UpdateMediaTabByType(type);

        if (IsPlayableMediaType(type))
            await LoadMediaAsync();
    }
    private async void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        Debug.WriteLine($"[VM] PropertyChanged: {e.PropertyName}");

        if (e.PropertyName == nameof(ViewModel.Item))
        {
            Debug.WriteLine("[VM] Item changed -> RefreshMediaAsync");
            await RefreshMediaAsync();
        }
    }
    private void MediaPlayer_MediaFailed(MediaPlayer sender, MediaPlayerFailedEventArgs args)
    {
        Debug.WriteLine($"[VIDEO] MediaFailed: {args.Error}");
        Debug.WriteLine($"[VIDEO] MediaFailed message: {args.ErrorMessage}");
    }
    private async void SelectFileButton_Click(object sender, RoutedEventArgs e)
    {
        await ViewModel.SelectFileForCurrentItemAsync();
        await RefreshMediaAsync();
    }
    private async void DetailTabButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button &&
            int.TryParse(button.Tag?.ToString(), out int tabIndex))
        {
            SelectedTabIndex = tabIndex;
            UpdateTabButtonStates();
            UpdateDetailContentVisibility();

            if (SelectedTabIndex == 1 && IsPlayableMediaType(ViewModel.Item?.ImageMimeType))
            {
                if (VideoPlayer.Source == null)
                    await LoadMediaAsync();
            }
        }
    }
    private void UpdateTabButtonStates()
    {
        var activeBrush = new SolidColorBrush(Color.FromArgb(255, 0, 103, 192));
        var inactiveBrush = new SolidColorBrush(Colors.Transparent);

        ImageTabUnderline.Background = SelectedTabIndex == 0 ? activeBrush : inactiveBrush;
        VideoTabUnderline.Background = SelectedTabIndex == 1 ? activeBrush : inactiveBrush;
    }
    private void UpdateDetailContentVisibility()
    {
        ImageGrid.Visibility = SelectedTabIndex == 0
            ? Visibility.Visible
            : Visibility.Collapsed;

        MediaGrid.Visibility = SelectedTabIndex == 1
            ? Visibility.Visible
            : Visibility.Collapsed;
    }
    private void UpdateMediaTabByType(string? type)
    {
        if (IsPlayableMediaType(type))
        {
            SelectedTabIndex = 1;
        }
        else if (IsImageType(type))
        {
            SelectedTabIndex = 0;
        }
        else
        {
            SelectedTabIndex = 0;
        }

        UpdateTabButtonStates();
        UpdateDetailContentVisibility();
    }
    private static bool IsVideoType(string? type)
    {
        if (string.IsNullOrWhiteSpace(type))
            return false;

        switch (type.Trim().ToLowerInvariant())
        {
            case "mp4":
            case "avi":
            case "mov":
            case "mpg":
            case "wmv":
            case "flv":
            case "swf":
                return true;

            default:
                return false;
        }
    }

    private static bool IsImageType(string? type)
    {
        if (string.IsNullOrWhiteSpace(type))
            return false;

        switch (type.Trim().ToLowerInvariant())
        {
            case "jpg":
            case "jpeg":
            case "png":
            case "gif":
            case "bmp":
            case "tif":
            case "tiff":
            case "ico":
            case "webp":
            case "wmf":
                return true;

            default:
                return false;
        }
    }

    private static bool IsAudioType(string? type)
    {
        if (string.IsNullOrWhiteSpace(type))
            return false;

        switch (type.Trim().ToLowerInvariant())
        {
            case "mp3":
            case "wav":
            case "m4a":
                return true;
            default:
                return false;
        }
    }

    private static bool IsPlayableMediaType(string? type)
    {
        return IsVideoType(type) || IsAudioType(type);
    }
    private void ClearVideoPlayer()
    {
        try
        {
            VideoPlayer.MediaPlayer?.Pause();
        }
        catch
        {
        }

        VideoPlayer.Source = null;
    }
    private async Task LoadMediaAsync()
    {
        try
        {
            Debug.WriteLine("[MEDIA] Overlay ON");

            await Task.Delay(500); // nur Testweise

            Debug.WriteLine("[MEDIA] LoadMediaAsync gestartet");

            ClearVideoPlayer();

            var bytes = ViewModel.Item?.Filestream;
            var type = ViewModel.Item?.ImageMimeType;

            Debug.WriteLine($"[MEDIA] MimeType = {type}");
            Debug.WriteLine($"[MEDIA] Bytes = {bytes?.Length ?? 0}");

            if (bytes == null || bytes.Length == 0)
            {
                Debug.WriteLine("[MEDIA] Keine Bytes vorhanden.");
                return;
            }

            if (!IsPlayableMediaType(type))
            {
                Debug.WriteLine("[MEDIA] Dateityp ist kein abspielbares Medium.");
                return;
            }

            var extension = string.IsNullOrWhiteSpace(type) ? "bin" : type.Trim().ToLowerInvariant();
            var fileName = $"media_{Guid.NewGuid():N}.{extension}";
            var tempPath = Path.Combine(Path.GetTempPath(), fileName);

            await File.WriteAllBytesAsync(tempPath, bytes);

            Debug.WriteLine($"[MEDIA] Temp-Datei: {tempPath}");

            var storageFile = await StorageFile.GetFileFromPathAsync(tempPath);
            var mediaSource = Windows.Media.Core.MediaSource.CreateFromStorageFile(storageFile);
            VideoPlayer.Source = mediaSource;

            Debug.WriteLine($"[MEDIA] Source gesetzt: {VideoPlayer.Source != null}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[MEDIA] Fehler: {ex}");
            ClearVideoPlayer();
        }
        finally
        {
            Debug.WriteLine("[MEDIA] Overlay OFF");
        }
    }

    //private async Task LoadVideoAsync()
    //{
    //    try
    //    {
    //        Debug.WriteLine("[VIDEO] LoadVideoAsync gestartet");

    //        ClearVideoPlayer();

    //        var bytes = ViewModel.Item?.Filestream;
    //        var type = ViewModel.Item?.ImageMimeType;

    //        Debug.WriteLine($"[VIDEO] MimeType = {type}");
    //        Debug.WriteLine($"[VIDEO] Bytes = {bytes?.Length ?? 0}");

    //        if (bytes == null || bytes.Length == 0)
    //        {
    //            Debug.WriteLine("[VIDEO] Keine Bytes vorhanden.");
    //            return;
    //        }

    //        if (!IsVideoType(type))
    //        {
    //            Debug.WriteLine("[VIDEO] Dateityp ist kein Video.");
    //            return;
    //        }

    //        var extension = string.IsNullOrWhiteSpace(type) ? "mp4" : type.Trim().ToLowerInvariant();
    //        var fileName = $"video_{Guid.NewGuid():N}.{extension}";
    //        var tempPath = Path.Combine(Path.GetTempPath(), fileName);

    //        await File.WriteAllBytesAsync(tempPath, bytes);

    //        Debug.WriteLine($"[VIDEO] Temp-Datei: {tempPath}");

    //        var storageFile = await StorageFile.GetFileFromPathAsync(tempPath);
    //        var mediaSource = Windows.Media.Core.MediaSource.CreateFromStorageFile(storageFile);
    //        VideoPlayer.Source = mediaSource;

    //        Debug.WriteLine($"[VIDEO] Source gesetzt: {VideoPlayer.Source != null}");
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.WriteLine($"[VIDEO] Fehler: {ex}");
    //        ClearVideoPlayer();
    //    }
    //}

}
