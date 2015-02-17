using ICSharpCode.AvalonEdit.Document;
using Mandrake.Management;
using Mandrake.Model;
using Mandrake.Sample.Client.Document;
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
    //[Export(typeof(IOperationManager))]
    //public class BasicInsertOperationManager : IOperationManager
    //{
    //    public Operation TryRecognize(object sender, EventArgs e)
    //    {
    //        var change = e as DocumentChangeEventArgs;

    //        if (change != null && change.InsertionLength > 0) return new InsertOperation(change.Offset, change.InsertedText.Text);
    //        else return null;
    //    }

    //    public bool TryExecute(object context, Operation o)
    //    {
    //        var insert = o as InsertOperation;
    //        var document = context as OTAwareDocument;

    //        if (insert == null) return false;

    //        document.InsertText(insert.Literal, insert.Position);

    //        return true;
    //    }
    //}

    //[Export(typeof(IOperationManager))]
    //public class BasicDeleteOperationManager : IOperationManager
    //{
    //    public Operation TryRecognize(object sender, EventArgs e)
    //    {
    //        var change = e as DocumentChangeEventArgs;

    //        if (change != null && change.RemovalLength > 0) return new DeleteOperation(change.Offset, change.Offset + change.RemovalLength);

    //        else return null;
    //    }

    //    public bool TryExecute(object context, Operation o)
    //    {
    //        var remove = o as DeleteOperation;
    //        var document = context as OTAwareDocument;

    //        if (remove == null) return false;

    //        document.RemoveText(remove.StartPosition, remove.EndPosition - remove.StartPosition);

    //        return true;
    //    }
    //}

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

    [Export(typeof(IOperationManager))]
    public class RichTextInsertManager: IOperationManager
    {
        public Operation TryRecognize(object sender, EventArgs e)
        {
            var args = e as TextChangedEventArgs;
            var change = (args != null) ? args.Changes.FirstOrDefault() : null;
            var editor = sender as RichTextBox;

            if (change != null && change.AddedLength != 0 && editor != null) 
            {
                int offset = change.Offset / 2;
                var text = new char[change.AddedLength];
                var cp = editor.CaretPosition.GetPositionAtOffset(offset);
                
                cp.GetTextInRun(System.Windows.Documents.LogicalDirection.Forward, text, offset, change.AddedLength);
                
                return new InsertOperation(offset, new String(text));
            }

            else return null;

        }

        public bool TryExecute(object context, Operation o)
        {
            var insert = o as InsertOperation;
            var editor = (OTAwareRichTextBox)context;

            if (insert == null) return false;

            editor.Dispatcher.BeginInvoke(new Action(() =>
            {
                using (editor.DeclareChangeBlock())
                {
                    editor.IsUpdatedByUser = false;
                    editor.CaretPosition = editor.CaretPosition.GetPositionAtOffset(insert.Position);
                    editor.CaretPosition.InsertTextInRun(insert.Literal);
                    editor.IsUpdatedByUser = true;
                }

            }));

            return true;
        }
    }

    [Export(typeof(IOperationManager))]
    public class RichTextDeleteManager: IOperationManager
    {
        public Operation TryRecognize(object sender, EventArgs e)
        {
            var args = e as TextChangedEventArgs;
            var change = (args != null) ? args.Changes.FirstOrDefault() : null;

            if (change != null && change.RemovedLength != 0) return new DeleteOperation(change.Offset, change.RemovedLength);

            else return null;

        }

        public bool TryExecute(object context, Operation o)
        {
            var remove = o as DeleteOperation;
            var editor = (OTAwareRichTextBox)context;

            if (remove == null) return false;

            editor.Dispatcher.BeginInvoke(new Action(() =>
            {
                using (editor.DeclareChangeBlock())
                {
                    editor.IsUpdatedByUser = false;
                    editor.CaretPosition = editor.CaretPosition.GetPositionAtOffset(remove.StartPosition);
                    editor.CaretPosition.DeleteTextInRun(remove.Length);
                    editor.IsUpdatedByUser = true;
                }

            }));

            return true;
        }
    }
}
