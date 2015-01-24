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

namespace Mandrake.Sample.Client.Management
{
    //[Export(typeof(IOperationManager))]
    public class BasicInsertOperationManager : IOperationManager
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
            var document = context as OTAwareDocument;

            if (insert == null) return false;

            document.InsertText(insert.Literal, insert.Position);

            return true;
        }
    }

    //[Export(typeof(IOperationManager))]
    public class BasicDeleteOperationManager : IOperationManager
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
            var document = context as OTAwareDocument;

            if (remove == null) return false;

            document.RemoveText(remove.StartPosition, remove.EndPosition - remove.StartPosition);

            return true;
        }
    }

    [Export(typeof(IOperationManager))]
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

    [Export(typeof(IOperationManager))]
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
}
