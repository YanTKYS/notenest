using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace NoteNest.Converters;

public class MarkerTypeToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var key = value?.ToString() switch
        {
            "TODO"  => "TodoBrush",
            "FIXME" => "FixmeBrush",
            "NOTE"  => "NoteBrush",
            _       => null
        };
        return key != null && Application.Current.Resources[key] is Brush b
            ? b
            : new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
