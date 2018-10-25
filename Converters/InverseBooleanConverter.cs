using System;
using System.Windows.Data;

namespace WpfControlExtensions.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBooleanConverter : ConverterBase<InverseBooleanConverter>
    {
        #region IValueConverter Members

        public override object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a boolean");

            try
            {
                return !(bool)value;
            }
            catch (Exception)
            {
                return value;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
