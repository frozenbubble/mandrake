using ICSharpCode.AvalonEdit;
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
    public partial class MultiCaretTextEditor : UserControl, IOTAwareContext
    {
        private Vector scrollOfset;

        public Dictionary<Guid, ColoredCaret> ColoredCursors { get; set; }

        public Dictionary<Guid, ColoredSelection> Selections { get; set; }
        public string SelectedText { get { return Editor.SelectedText; } }
        public bool IsUpdatedByUser { get; set; }

        public event EventHandler<DocumentChangeEventArgs> DocumentChanged;
        public event EventHandler<CaretPositionChangedEventArgs> CaretPositionChanged;
        public event EventHandler<TextSelectionChangedEventArgs> SelectionChanged;

        public MultiCaretTextEditor()
        {
            InitializeComponent();

            Editor.Document.Changed += Document_Changed;
            Editor.TextArea.Caret.PositionChanged += Caret_PositionChanged;
            Editor.TextArea.SelectionChanged += TextArea_SelectionChanged;
            Editor.TextArea.TextView.ScrollOffsetChanged += TextView_ScrollOffsetChanged;
            

            IsUpdatedByUser = true;
            ColoredCursors = new Dictionary<Guid, ColoredCaret>();
            Selections = new Dictionary<Guid, ColoredSelection>();
            scrollOfset = Editor.TextArea.TextView.ScrollOffset;
        }

        void TextView_ScrollOffsetChanged(object sender, EventArgs e)
        {
            var offset = ((TextView)sender).ScrollOffset;
            var v = Vector.Subtract(offset, scrollOfset);

            TranslateVisuals(v);
            scrollOfset = offset;
        }

        private void TranslateVisuals(Vector offset)
        {
            foreach (var id in ColoredCursors.Keys)
            {
                ColoredCursors[id].Translate(offset);
                Selections[id].Translate(offset);
            }
        }

        void TextArea_SelectionChanged(object sender, EventArgs e)
        {
            if (SelectionChanged != null)
            {
                var s = sender as TextArea;
                var startLocation = s.Selection.StartPosition.Location;
                var endLocation = s.Selection.EndPosition.Location;
                TextSelectionChangedEventArgs args = null;

                if (startLocation.Line == 0 && startLocation.Column == 0
                    && endLocation.Line == 0 && endLocation.Column == 0)
                {
                    args = new TextSelectionChangedEventArgs(0, 0);
                }
                else
                {
                    int start = Editor.Document.GetOffset(s.Selection.StartPosition.Location);
                    int end = Editor.Document.GetOffset(s.Selection.EndPosition.Location);

                    args = new TextSelectionChangedEventArgs(start, end);
                }

                SelectionChanged(sender, args);
            }
        }

        void Caret_PositionChanged(object sender, EventArgs e)
        {
            var s = sender as Caret;
            var args = new CaretPositionChangedEventArgs(s.Offset);

            if (CaretPositionChanged != null) CaretPositionChanged(sender, args);
        }

        void Document_Changed(object sender, DocumentChangeEventArgs e)
        {
            if (!this.IsUpdatedByUser) return;

            if (DocumentChanged != null)
            {
                if (e.RemovalLength > 0 && e.InsertionLength > 0)
                {
                    var e1 = new DocumentChangeEventArgs(e.Offset, e.RemovedText.Text, null);
                    var e2 = new DocumentChangeEventArgs(e.Offset, null, e.InsertedText.Text);

                    DocumentChanged(sender, e1);
                    DocumentChanged(sender, e2);
                }

                else DocumentChanged(sender, e);
            }

        }

        public void MoveCaret(Guid id, int offset)
        {
            if (!ColoredCursors.ContainsKey(id)) return;

            Editor.Dispatcher.BeginInvoke(new Action(() => 
            {
                var visualPosition = GetVisualPosition(offset);
                ColoredCursors[id].Position = visualPosition;
            }));

        }

        private Point GetVisualPosition(int offset)
        {
            var location = Editor.Document.GetLocation(offset);
            return GetVisualPosition(location);
        }

        private Point GetVisualPosition(TextLocation location)
        {
            return Editor.TextArea.TextView.GetVisualPosition(new TextViewPosition(location), VisualYPosition.LineTop);
        }

        public void RegisterClient(Guid id, string name)
        {
            ColoredCursors.Add(id, new ColoredCaret(CursorCanvas, new Point(0, 0), id, name, 16));
            Selections.Add(id, new ColoredSelection(id, CursorCanvas));
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

        public void Select(Guid id, int selectionStartOffset, int selectionEndOffset)
        {
            if (Selections.Count == 0) return;

            if (selectionStartOffset == 0 && selectionEndOffset == 0)
            {
                Selections[id].Lines = null;
                return;
            }

            TextLocation start, end;

            if (selectionStartOffset < selectionEndOffset)
            {
                start = Editor.Document.GetLocation(selectionStartOffset);
                end = Editor.Document.GetLocation(selectionEndOffset);
            }
            else
            {
                start = Editor.Document.GetLocation(selectionEndOffset);
                end = Editor.Document.GetLocation(selectionStartOffset);
            }

            var lines = new List<LineSelection>();

            if (start.Line == end.Line)
            {
                //just get that one rectangle
                var startPos = GetVisualPosition(start);
                var endPos = GetVisualPosition(end);
                int width = (int)(endPos.X - startPos.X);

                lines.Add(new LineSelection(width, startPos, Selections[id].Color));
            }
            else
            {
                //first line
                var topStartPos = GetVisualPosition(start);
                var topEndPos = GetVisualPosition(Editor.Document.Lines[start.Line - 1].EndOffset);
                int topWidth = (int)(topEndPos.X - topStartPos.X);

                lines.Add(new LineSelection(topWidth, topStartPos, Selections[id].Color));

                //last line
                var bottomStartPos = GetVisualPosition(Editor.Document.Lines[end.Line - 1].Offset);
                var bottomEndPos = GetVisualPosition(end);
                int bottomWidth = (int)(bottomEndPos.X - bottomStartPos.X);

                lines.Add(new LineSelection(bottomWidth, bottomStartPos, Selections[id].Color));
                
                for (int i = start.Line; i < end.Line - 1; i++)
                {
                    int offset = Editor.TextArea.Document.Lines[i].Offset;
                    int endOffset = Editor.TextArea.Document.Lines[i].EndOffset;
                    var lineStart = GetVisualPosition(offset);
                    var lineEnd = GetVisualPosition(endOffset);
                    int width = (int)(lineEnd.X - lineStart.X);

                    var lineSelection = new LineSelection(width, lineStart, Selections[id].Color);
                    lines.Add(lineSelection);
                }
            }

            Selections[id].Lines = lines;
        }

        public void Paste(string text)
        {
            InsertText(text, Editor.CaretOffset);
        }

        public void Clear()
        {
            Editor.Clear();
        }
    }
}
