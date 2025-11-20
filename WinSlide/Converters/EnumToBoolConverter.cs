using System.Globalization;
using System.Windows.Data;

namespace WinSlide.Converters;

public class EnumToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter is null)
        {
            return false;
        }

        return value.ToString() == parameter.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter is null || !(bool)value)
        {
            return Binding.DoNothing;
        }

        return Enum.Parse(targetType, parameter.ToString()!);
    }
}
