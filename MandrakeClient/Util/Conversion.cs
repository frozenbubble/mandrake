using Mandrake.Client.View;
using Mandrake.Samples.Client.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Mandrake.Samples.Client.Util
{
    public class GuidToColorConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var id = (Guid)value;

            return new SolidColorBrush(ColoredItem.GenerateColor(id));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("Mapping from Guid to Color is not bijective.");
        }
    }

    public class GuidToAlignmentConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var id = (Guid)value;

            if (id == MainViewModel.Current.Id) return HorizontalAlignment.Left;

            else return HorizontalAlignment.Right;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("Mapping from Guid to Margin is not bijective.");
        }
    }

    public class WidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double percentage = Format(parameter.ToString());

            return 10d + (double.Parse(value.ToString()) * percentage);
        }

        private double Format(string param)
        {
            var splitted = param.Split('.');
            var formattedParameter = 0d;

            if (splitted.Length == 1) formattedParameter = double.Parse(param);

            else if (splitted.Length == 2) formattedParameter = double.Parse(splitted[0] + "," + splitted[1]);

            else throw new FormatException("The given number is not recognizable");

            return formattedParameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("This operation is not suppored.");
        }
    }

    public class GuidToVisibilityConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var id = (Guid)value;

            if ( (id == MainViewModel.Current.Id && parameter.ToString().Equals("Left"))
                || (id != MainViewModel.Current.Id && parameter.ToString().Equals("Right")))
            {
                return Visibility.Visible;
            }

            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("This operation is not suppored.");
        }
    }
}
