namespace ATIS.Helpers;

class GetIcons
{
    private IconElement GetIcon(string imagePath)
    {
        return string.IsNullOrEmpty(imagePath)
            ? null
            : imagePath.ToLowerInvariant().EndsWith(".png") || imagePath.ToLowerInvariant().EndsWith(".jpg")
                ? new BitmapIcon() { UriSource = new Uri(imagePath, UriKind.RelativeOrAbsolute), ShowAsMonochrome = false }
                : new FontIcon()
                {
                    Glyph = imagePath
                };
    }
}
