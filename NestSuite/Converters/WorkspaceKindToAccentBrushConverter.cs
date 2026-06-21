using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace NestSuite.Converters;

public class WorkspaceKindToAccentBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is NestSuiteWorkspaceKind kind
            ? kind switch
            {
                NestSuiteWorkspaceKind.NoteNest => new SolidColorBrush(Color.FromRgb(0x4A, 0x90, 0xD9)),
                NestSuiteWorkspaceKind.IdeaNest => new SolidColorBrush(Color.FromRgb(0xE8, 0xA0, 0x20)),
                NestSuiteWorkspaceKind.ChatNest  => new SolidColorBrush(Color.FromRgb(0x4C, 0xAF, 0x50)),
                NestSuiteWorkspaceKind.Temp => new SolidColorBrush(Color.FromRgb(0xA0, 0xA0, 0xA8)),
                _ => Brushes.Transparent
            }
            : (object)Brushes.Transparent;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
