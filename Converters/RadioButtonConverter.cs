using System;
using System.Windows.Data;

namespace WpfControlExtensions.Converters
{
    /// <summary>
    /// <RadioButton IsChecked="{Binding Path=YourEnumProperty, Converter={StaticResource ComparisonConverter}, ConverterParameter={x:Static local:YourEnumType.Enum1}}" />
    /// </summary>
    public class RadioButtonConverter : ConverterBase<RadioButtonConverter>
    {
        public RadioButtonConverter() { }

        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value?.Equals(parameter);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value?.Equals(true) == true ? parameter : Binding.DoNothing;
        }
    }
}
