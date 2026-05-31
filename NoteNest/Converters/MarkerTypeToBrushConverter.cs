using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace NoteNest.Converters;

public class MarkerTypeToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value?.ToString() switch
        {
            "TODO"  => new SolidColorBrush(Color.FromRgb(0xE6, 0x7E, 0x22)),
            "FIXME" => new SolidColorBrush(Color.FromRgb(0xE7, 0x4C, 0x3C)),
            "NOTE"  => new SolidColorBrush(Color.FromRgb(0x27, 0xAE, 0x60)),
            _       => new SolidColorBrush(Colors.Gray)
        };

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
