using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace Stats.App.Converters;

public class GreaterThanZeroToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is float f)
            return f > 0 ? Visibility.Visible : Visibility.Collapsed;
        if (value is double d)
            return d > 0 ? Visibility.Visible : Visibility.Collapsed;
        if (value is int i)
            return i > 0 ? Visibility.Visible : Visibility.Collapsed;
        if (value is long l)
            return l > 0 ? Visibility.Visible : Visibility.Collapsed;

        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

public class StringNotEmptyToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is string s)
            return !string.IsNullOrWhiteSpace(s) ? Visibility.Visible : Visibility.Collapsed;

        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool b)
            return b ? Visibility.Visible : Visibility.Collapsed;

        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is Visibility v)
            return v == Visibility.Visible;

        return false;
    }
}
