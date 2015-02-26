using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mandrake.Sample.Client.Event
{
    public class CaretPositionChangedEventArgs: EventArgs
    {
        public int Offset { get; set; }

        public CaretPositionChangedEventArgs(int offset)
        {
            this.Offset = offset;
        }
    }

    public class TextSelectionChangedEventArgs: EventArgs
    {
        public int SelectionStart { get; set; }
        public int SelectionEnd { get; set; }

        public TextSelectionChangedEventArgs(int startPosition, int endPosition)
        {
            SelectionStart = startPosition;
            SelectionEnd = endPosition;
        }
    }
}
