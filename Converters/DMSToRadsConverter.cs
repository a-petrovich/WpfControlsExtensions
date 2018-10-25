using System;

namespace WpfControlExtensions.Converters
{
    public class DMSToRadsConverter : ConverterBase<DMSToRadsConverter>
    {
        public DMSToRadsConverter() { }

        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                var Dms = (string)value;

                if (string.IsNullOrWhiteSpace(Dms))
                    return "";

                var NSEW = Dms.Substring(0, 1);


                var D = Dms.Substring(1, 3);
                var M = Dms.Substring(5, 2);
                var S = Dms.Substring(8, 4);

                if (S.Contains(".")) S = S.Replace(".", ",");

                if (!Double.TryParse(D, out double d)) return "wrong format";
                if (!Double.TryParse(M, out double m)) return "wrong format";
                if (!Double.TryParse(S, out double s)) return "wrong format";

                var degree = (d + (m / 60) + (s / 3600)) * (NSEW.Equals("N") || NSEW.Equals("E") ? 1 : -1);

                return ((Math.PI / 180) * degree).ToString().Replace(",", ".");
            }
            catch (Exception)
            {
                return "wrong format";
            }
        }
    }

    public class GeoAngle
    {
        public bool IsNegative { get; set; }
        public int Degrees { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }
        public int Milliseconds { get; set; }
        public bool? IsNS { get; set; }

        public static GeoAngle FromDouble(double angleInDegrees)
        {
            //ensure the value will fall within the primary range [-180.0..+180.0]
            while (angleInDegrees < -180.0)
                angleInDegrees += 360.0;

            while (angleInDegrees > 180.0)
                angleInDegrees -= 360.0;

            var result = new GeoAngle();

            //switch the value to positive
            result.IsNegative = angleInDegrees < 0;
            angleInDegrees = Math.Abs(angleInDegrees);

            //gets the degree
            result.Degrees = (int)Math.Floor(angleInDegrees);
            var delta = angleInDegrees - result.Degrees;

            //gets minutes and seconds
            var seconds = (int)Math.Floor(3600.0 * delta);
            result.Seconds = seconds % 60;
            result.Minutes = (int)Math.Floor(seconds / 60.0);
            delta = delta * 3600.0 - seconds;

            //gets fractions
            result.Milliseconds = (int)(10.0 * delta);

            return result;
        }

        public override string ToString()
        {
            var degrees = this.IsNegative
                ? -this.Degrees
                : this.Degrees;

            return string.Format(
                "{0}° {1:00}' {2:00}\"",
                degrees,
                this.Minutes,
                this.Seconds);
        }

        public string ToString(string format)
        {
            switch (format)
            {
                case "NS":
                    return string.Format(
                        "{0}{1:000}°{2:00}'{3:00}.{4:0}\"",
                        this.IsNegative ? 'S' : 'N',
                        this.Degrees,
                        this.Minutes,
                        this.Seconds,
                        this.Milliseconds)
                        ;

                case "WE":
                    return string.Format(
                        "{0}{1:000}°{2:00}'{3:00}.{4:0}\"",
                        this.IsNegative ? 'W' : 'E',
                        this.Degrees,
                        this.Minutes,
                        this.Seconds,
                        this.Milliseconds);

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
