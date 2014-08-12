using Mandrake.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Mandrake.Host.View.Converters
{
    public class ConnectionPresenter : IValueConverter
    {
        private static readonly List<Color> Colors = new List<Color>();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            byte[] colBuff = new byte[3];
            Random rand = new Random();
            rand.NextBytes(colBuff);
            
            var info = value as SynchronizingConnection;
            var dto = new ConnectionDTO
            {
                Messages = info.ClientMessages,
                Color = Color.FromRgb(colBuff[0], colBuff[1], colBuff[2]),
                Id = rand.Next().ToString()
            };

            return dto;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public class ConnectionDTO
        {
            public string Id { get; set; }
            public int Messages { get; set; }
            public Color Color { get; set; }
        }
    }
}
