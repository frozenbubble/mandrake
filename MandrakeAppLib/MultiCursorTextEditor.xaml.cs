using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using Mandrake.Model.Document;
using Mandrake.Sample.Client.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mandrake.Client.View
{
    /// <summary>
    /// Interaction logic for MultiCursorTextEditor.xaml
    /// </summary>
    public partial class MultiCursorTextEditor : UserControl, IOTAwareContext //cannot create instance when it's otaware
    {
        public Dictionary<Guid, ColoredCursor> ColoredCursors { get; set; }
        public bool IsUpdatedByUser { get; set; }

        public event EventHandler<DocumentChangeEventArgs> DocumentChanged;
        public event EventHandler<CaretPositionChangedEventArgs> CaretPositionChanged;

        public MultiCursorTextEditor()
        {
            InitializeComponent();
            
            //Editor.Document.Changed += Document_Changed;
            //Editor.TextArea.Caret.PositionChanged += Caret_PositionChanged;

            IsUpdatedByUser = true;
            ColoredCursors = new Dictionary<Guid, ColoredCursor>();
        }

        void Caret_PositionChanged(object sender, EventArgs e)
        {
            var s = sender as Caret;
            var args = new CaretPositionChangedEventArgs(s.Offset);
            if (CaretPositionChanged != null) CaretPositionChanged(sender, args);
        }

        void Document_Changed(object sender, DocumentChangeEventArgs e)
        {
            var visualPosition = Editor.TextArea.TextView.GetVisualPosition(Editor.TextArea.Caret.Position, VisualYPosition.LineTop);
            if (DocumentChanged != null) DocumentChanged(sender, e);
        }

        public void OnCursorMoved(Guid id, int offset)
        {
            var visualPosition = Editor.TextArea.TextView.GetVisualPosition(Editor.TextArea.Caret.Position, VisualYPosition.LineTop);
            ColoredCursors[id].Position = visualPosition;
        }

        public void RegisterCursor(Guid id, string name)
        {
            int fontsize = Convert.ToInt32(Editor.TextArea.FontSize);
            ColoredCursors.Add(id, new ColoredCursor(CursorCanvas, new Point(0, 0), id, name, fontsize));
        }


        public void InsertText(string text, int position)
        {
            //throw new NotImplementedException();
        }

        public void RemoveText(int position, int length)
        {
            //throw new NotImplementedException();
        }
    }
}
