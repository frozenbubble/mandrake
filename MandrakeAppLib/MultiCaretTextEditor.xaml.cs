using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using Mandrake.Model.Document;
using Mandrake.Sample.Client.Event;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
    [Export(typeof(IOTAwareContext))]
    public partial class MultiCaretTextEditor : UserControl, IOTAwareContext //cannot create instance when it's otaware
    {
        public Dictionary<Guid, ColoredCaret> ColoredCursors { get; set; }
        public bool IsUpdatedByUser { get; set; }

        public event EventHandler<DocumentChangeEventArgs> DocumentChanged;
        public event EventHandler<CaretPositionChangedEventArgs> CaretPositionChanged;

        public MultiCaretTextEditor()
        {
            InitializeComponent();

            Editor.Document.Changed += Document_Changed;
            Editor.TextArea.Caret.PositionChanged += Caret_PositionChanged;

            IsUpdatedByUser = true;
            ColoredCursors = new Dictionary<Guid, ColoredCaret>();
        }

        void Caret_PositionChanged(object sender, EventArgs e)
        {
            if (!this.IsUpdatedByUser) return;

            var s = sender as Caret;
            var args = new CaretPositionChangedEventArgs(s.Offset);
            if (CaretPositionChanged != null) CaretPositionChanged(sender, args);
        }

        void Document_Changed(object sender, DocumentChangeEventArgs e)
        {
            if (!this.IsUpdatedByUser) return;

            var visualPosition = Editor.TextArea.TextView.GetVisualPosition(Editor.TextArea.Caret.Position, VisualYPosition.LineTop);
            if (DocumentChanged != null) DocumentChanged(sender, e);
        }

        public void MoveCaret(Guid id, int offset)
        {
            if (!ColoredCursors.ContainsKey(id)) return;

            Editor.Dispatcher.BeginInvoke(new Action(() => 
            {
                //Editor.TextArea.Caret.PositionChanged -= Caret_PositionChanged;

                var visualPosition = Editor.TextArea.TextView.GetVisualPosition(Editor.TextArea.Caret.Position, VisualYPosition.LineTop);
                ColoredCursors[id].Position = visualPosition;

                //Editor.TextArea.Caret.PositionChanged += Caret_PositionChanged;
            }));

        }

        public void RegisterCursor(Guid id, string name)
        {
            //int fontsize = Convert.ToInt32(Editor.TextArea.FontSize);
            ColoredCursors.Add(id, new ColoredCaret(CursorCanvas, new Point(0, 0), id, name, 16));
        }


        public void InsertText(string text, int position)
        {
            using (Editor.DeclareChangeBlock()) 
            {
                Editor.Document.Changed -= Document_Changed;
                Editor.Document.Insert(position, text);
                Editor.Document.Changed += Document_Changed;
            } 
        }

        public void RemoveText(int position, int length)
        {
            Editor.Document.Changed -= Document_Changed;
            using (Editor.DeclareChangeBlock()) Editor.Document.Remove(position, length);
            Editor.Document.Changed += Document_Changed;
        }
    }
}
