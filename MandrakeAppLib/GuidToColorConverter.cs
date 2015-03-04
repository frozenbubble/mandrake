using Mandrake.Client.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Mandrake.Samples.Client.Util
{
    public class GuidToColorConverter: IValueConverter
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
}
