using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Media.Core;
using Windows.Storage.Streams;

namespace ATIS.Views.Modules.Tbl81Image
{
    public sealed partial class Tbl81ImagePreviewControl : UserControl
    {
        private double _zoomFactor = 1.0;
        private double _imagePixelWidth = 0;
        private double _imagePixelHeight = 0;
        private InMemoryRandomAccessStream? _mediaStream;

        public Tbl81ImagePreviewControl()
        {
            InitializeComponent();
        }

        public Models.Tbl81Image? Item
        {
            get => (Models.Tbl81Image?)GetValue(ItemProperty);
            set => SetValue(ItemProperty, value);
        }

        public static readonly DependencyProperty ItemProperty =
            DependencyProperty.Register(
                nameof(Item),
                typeof(Models.Tbl81Image),
                typeof(Tbl81ImagePreviewControl),
                new PropertyMetadata(null, OnItemChanged));

        private static void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (Tbl81ImagePreviewControl)d;
            var item = e.NewValue as Models.Tbl81Image;
            control.ApplyItem(item);
        }

        private async void ApplyItem(Models.Tbl81Image? item)
        {
            await LoadPreviewAsync(item);
        }

        private async Task LoadPreviewAsync(Models.Tbl81Image? item)
        {
            ClearPreview();

            if (item?.Filestream == null || item.Filestream.Length == 0)
                return;

            var mimeType = GetMimeType(item);

            if (IsImageMimeType(mimeType))
            {
                await LoadImageAsync(item);
            }
            else if (IsMediaMimeType(mimeType))
            {
                await LoadMediaAsync(item, mimeType);
            }
        }

        private void ClearPreview()
        {
            PreviewImage.Source = null;
            _imagePixelWidth = 0;
            _imagePixelHeight = 0;
            _zoomFactor = 1.0;
            ApplyZoom();

            if (_mediaPlayer != null)
            {
                _mediaPlayer.Source = null;
                _mediaPlayer.Dispose();
                _mediaPlayer = null;
            }

            VideoPlayer.SetMediaPlayer(null);

            _mediaStream?.Dispose();
            _mediaStream = null;

            ImageToolbar.Visibility = Visibility.Collapsed;
            MediaGrid.Visibility = Visibility.Collapsed;
            ImageScrollViewer.Visibility = Visibility.Collapsed;
        }
        private void ShowImage()
        {
            ImageToolbar.Visibility = Visibility.Visible;
            ImageScrollViewer.Visibility = Visibility.Visible;
            MediaGrid.Visibility = Visibility.Collapsed;
        }
        private void ShowMedia()
        {
            ImageToolbar.Visibility = Visibility.Collapsed;
            ImageScrollViewer.Visibility = Visibility.Collapsed;
            MediaGrid.Visibility = Visibility.Visible;
        }
        private async Task LoadImageAsync(Models.Tbl81Image item)
        {
            try
            {
                using var stream = new MemoryStream(item.Filestream!);
                using var randomAccessStream = stream.AsRandomAccessStream();

                var bitmap = new BitmapImage();
                await bitmap.SetSourceAsync(randomAccessStream);

                _imagePixelWidth = bitmap.PixelWidth;
                _imagePixelHeight = bitmap.PixelHeight;

                PreviewImage.Source = bitmap;
                ShowImage();

                await Task.Delay(50);
                FitToWindow();
            }
            catch
            {
                PreviewImage.Source = null;
            }
        }
        private Windows.Media.Playback.MediaPlayer? _mediaPlayer;

        private async Task LoadMediaAsync(Models.Tbl81Image item, string mimeType)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(mimeType))
                    return;

                _mediaStream?.Dispose();
                _mediaStream = new InMemoryRandomAccessStream();
                await _mediaStream.WriteAsync(item.Filestream!.AsBuffer());
                _mediaStream.Seek(0);

                _mediaPlayer?.Dispose();
                _mediaPlayer = new Windows.Media.Playback.MediaPlayer();

                var mediaSource = MediaSource.CreateFromStream(_mediaStream, mimeType);
                _mediaPlayer.Source = mediaSource;

                VideoPlayer.SetMediaPlayer(_mediaPlayer);
                ShowMedia();

                // Nur zum Testen:
              //  _mediaPlayer.Play();
            }
            catch
            {
                ShowMedia();
            }
        }
        private static bool IsImageMimeType(string? mimeType)
        {
            System.Diagnostics.Debug.WriteLine($"ImageMimeType={mimeType}");

            return !string.IsNullOrWhiteSpace(mimeType) &&
                   mimeType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsMediaMimeType(string? mimeType)
        {
            System.Diagnostics.Debug.WriteLine($"MediaMimeType={mimeType}");

            return !string.IsNullOrWhiteSpace(mimeType) &&
                   (mimeType.StartsWith("video/", StringComparison.OrdinalIgnoreCase) ||
                    mimeType.StartsWith("audio/", StringComparison.OrdinalIgnoreCase));
        }

        private string? GetMimeType(Models.Tbl81Image item)
        {
            var raw = item.ImageMimeType;

            if (string.IsNullOrWhiteSpace(raw))
                return null;

            var normalized = raw.Trim().ToLowerInvariant().TrimStart('.');

            var result = normalized switch
            {
                "jpg" => "image/jpeg",
                "jpeg" => "image/jpeg",
                "png" => "image/png",
                "bmp" => "image/bmp",
                "gif" => "image/gif",
                "tif" => "image/tiff",
                "tiff" => "image/tiff",

                "mp4" => "video/mp4",
                "wmv" => "video/x-ms-wmv",
                "avi" => "video/x-msvideo",
                "mov" => "video/quicktime",

                "mp3" => "audio/mpeg",
                "wav" => "audio/wav",
                "wma" => "audio/x-ms-wma",
                "m4a" => "audio/mp4",

                _ => null
            };

            System.Diagnostics.Debug.WriteLine($"RawType={raw}");
            System.Diagnostics.Debug.WriteLine($"ResolvedMimeType={result}");

            return result;
        }
        private void Zoom100Button_Click(object sender, RoutedEventArgs e)
        {
            _zoomFactor = 1.0;
            ApplyZoom();
        }

        private void FitButton_Click(object sender, RoutedEventArgs e)
        {
            FitToWindow();
        }

        private void ZoomInButton_Click(object sender, RoutedEventArgs e)
        {
            _zoomFactor *= 1.25;
            ApplyZoom();
        }

        private void ZoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            _zoomFactor /= 1.25;
            ApplyZoom();
        }

        private void ApplyZoom()
        {
            ImageScaleTransform.ScaleX = _zoomFactor;
            ImageScaleTransform.ScaleY = _zoomFactor;
        }

        private void FitToWindow()
        {
            if (PreviewImage.Source == null)
                return;

            if (_imagePixelWidth <= 0 || _imagePixelHeight <= 0)
                return;

            var availableWidth = ImageScrollViewer.ActualWidth;
            var availableHeight = ImageScrollViewer.ActualHeight;

            if (availableWidth <= 0 || availableHeight <= 0)
                return;

            var scaleX = availableWidth / _imagePixelWidth;
            var scaleY = availableHeight / _imagePixelHeight;

            _zoomFactor = Math.Min(scaleX, scaleY);
            ApplyZoom();
        }
    }
}
