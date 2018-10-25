using System;

namespace WpfControlExtensions.Converters
{
    public class BoolToVisibilityConverter : ConverterBase<BoolToVisibilityConverter>
    {
        public BoolToVisibilityConverter() { }

        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                var v = (bool)value;
                if (v)
                    return System.Windows.Visibility.Visible;
                else
                    return System.Windows.Visibility.Collapsed;
            }
            catch (Exception)
            {
                return value;
            }
        }

    }

    public class BoolToVisibilityConverterInverted : ConverterBase<BoolToVisibilityConverterInverted>
    {
        public BoolToVisibilityConverterInverted() { }

        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                var v = (bool)value;
                if (!v)
                    return System.Windows.Visibility.Visible;
                else
                    return System.Windows.Visibility.Collapsed;
            }
            catch (Exception)
            {
                return value;
            }
        }
    }
}
