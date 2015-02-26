using ICSharpCode.AvalonEdit.Document;
using Mandrake.Client.View;
using Mandrake.Management;
using Mandrake.Model;
using Mandrake.Sample.Client.Document;
using Mandrake.Sample.Client.Event;
using Mandrake.Sample.Client.Operations;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Mandrake.Sample.Client.Management
{
    #region OTAware editor

    //[Export(typeof(IOperationManager))]
    public class EditorInsertOperationManager : IOperationManager
    {
        public Operation TryRecognize(object sender, EventArgs e)
        {
            var change = e as DocumentChangeEventArgs;

            if (change.InsertionLength > 0) return new InsertOperation(change.Offset, change.InsertedText.Text);
            else return null;
        }

        public bool TryExecute(object context, Operation o)
        {
            var insert = o as InsertOperation;
            var editor = (OTAwareEditor)context;

            if (insert == null) return false;

            editor.Dispatcher.BeginInvoke(new Action(() =>
            {
                using (editor.DeclareChangeBlock())
                {
                    editor.IsUpdatedByUser = false;
                    editor.InsertText(insert.Literal, insert.Position);
                    editor.IsUpdatedByUser = true;
                }

            }));

            return true;
        }
    }

    //[Export(typeof(IOperationManager))]
    public class EditorDeleteOperationManager : IOperationManager
    {
        public Operation TryRecognize(object sender, EventArgs e)
        {
            var change = e as DocumentChangeEventArgs;

            if (change.RemovalLength > 0) return new DeleteOperation(change.Offset, change.Offset + change.RemovalLength);

            else return null;
        }

        public bool TryExecute(object context, Operation o)
        {
            var remove = o as DeleteOperation;
            var editor = (OTAwareEditor)context;

            if (remove == null) return false;

            editor.Dispatcher.BeginInvoke(new Action(() =>
            {
                using (editor.DeclareChangeBlock())
                {
                    editor.IsUpdatedByUser = false;
                    editor.RemoveText(remove.StartPosition, remove.EndPosition - remove.StartPosition);
                    editor.IsUpdatedByUser = true;
                }

            }));

            return true;
        }
    }
    #endregion

    #region MultiCaret Text Editor
    
    [Export(typeof(IOperationManager))]
    public class CaretPositionOperationManager: IOperationManager
    {
        public Operation TryRecognize(object sender, EventArgs e)
        {
            var args = e as CaretPositionChangedEventArgs;

            if (args == null) return null;
            else return new CaretPositionOperation(args.Offset);
        }

        public bool TryExecute(object context, Operation o)
        {
            var move = o as CaretPositionOperation;
            var editor = context as MultiCaretTextEditor;

            if (move == null || editor == null) return false;

            editor.IsUpdatedByUser = false;
            editor.MoveCaret(move.OwnerId, move.Position);
            editor.IsUpdatedByUser = true;

            return true;
        }
    }

    [Export(typeof(IOperationManager))]
    public class MultiCaretInsertOperationManager: IOperationManager
    {

        public Operation TryRecognize(object sender, EventArgs e)
        {
            var change = e as DocumentChangeEventArgs;

            if (change != null && change.InsertionLength > 0) return new InsertOperation(change.Offset, change.InsertedText.Text);
            else return null;
        }

        public bool TryExecute(object context, Operation o)
        {
            var insert = o as InsertOperation;
            var editor = context as MultiCaretTextEditor;

            if (insert == null || editor == null) return false;

            editor.Dispatcher.BeginInvoke(new Action(() =>
            {
                    editor.IsUpdatedByUser = false;
                    editor.InsertText(insert.Literal, insert.Position);
                    editor.IsUpdatedByUser = true;
            }));

            return true;
        }
    }

    [Export(typeof(IOperationManager))]
    public class MultiCaretDeleteOperationManager : IOperationManager
    {

        public Operation TryRecognize(object sender, EventArgs e)
        {
            var change = e as DocumentChangeEventArgs;

            if (change != null && change.RemovalLength > 0) return new DeleteOperation(change.Offset, change.Offset + change.RemovalLength);
            else return null;
        }

        public bool TryExecute(object context, Operation o)
        {
            var remove = o as DeleteOperation;
            var editor = context as MultiCaretTextEditor;

            if (remove == null || editor == null) return false;

            editor.Dispatcher.BeginInvoke(new Action(() =>
            {
                    editor.IsUpdatedByUser = false;
                    editor.RemoveText(remove.StartPosition, remove.EndPosition - remove.StartPosition);
                    editor.IsUpdatedByUser = true;

            }));

            return true;
        }
    }
    
    #endregion
}
