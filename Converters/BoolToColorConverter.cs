using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfControlExtensions.Converters
{
    /// <summary>
    /// Default colors are Black(true) and Red(false).
    /// They can be set using ConverterParameter "Green,Yellow" or "Green" or ",Yellow"
    /// </summary>
    public class BoolToSolidColorBrushConverter : ConverterBase<BoolToSolidColorBrushConverter>
    {
        public BoolToSolidColorBrushConverter() { }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var v = (bool)value;

                if (parameter is string colors)
                {
                    var colorsArray = colors.Split(',');
                    if(v)
                    {
                        if (colorsArray.Length > 0)
                            return (SolidColorBrush)new BrushConverter().ConvertFromString(colorsArray[0]) ?? new SolidColorBrush(Colors.Black);
                        return new SolidColorBrush(Colors.Black);
                    }
                    else
                    {
                        if (colorsArray.Length > 1)
                            return (SolidColorBrush)new BrushConverter().ConvertFromString(colorsArray[1]) ?? new SolidColorBrush(Colors.Red);
                        return new SolidColorBrush(Colors.Red);
                    }
                }
                else
                {
                    if (v)
                        return new SolidColorBrush(Colors.Red);
                    else
                        return new SolidColorBrush(Colors.Black);
                }
            }
            catch
            {
                return value;
            }
        }
    }
}
