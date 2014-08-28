using ICSharpCode.AvalonEdit.Document;
using Mandrake.Model;
using Mandrake.View.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
namespace Mandrake.Management
{
    public delegate void OperationActionEventHandler(object sender, Operation o);

    public abstract class OTManager
    {
        protected int myMessages;
        protected int otherMessages;

        public IOTAwareContext Context { get; set; }
        public List<IOperationManager> ManagerChain { get; set; }
        public List<Operation> Log { get; protected set; }

        protected abstract void Transform(Operation o);
        protected abstract void Execute(Operation o);
    }

    public interface IOperationManager
    {
        Operation TryRecognize(object sender, EventArgs e);
        bool TryExecute(object context, Operation o);
    }

    public class BasicInsertOperationManager : IOperationManager
    {
        public Operation TryRecognize(object sender, EventArgs e)
        {
            var change = e as DocumentChangeEventArgs;

            if (change != null && change.InsertionLength > 0) return new InsertOperation(change.Offset, change.InsertedText);
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

    public class EditorInsertOperationManager : IOperationManager
    {
        public Operation TryRecognize(object sender, EventArgs e)
        {
            var change = e as DocumentChangeEventArgs;

            if (change.InsertionLength > 0) return new InsertOperation(change.Offset, change.InsertedText);
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

    public interface ISynchronize
    {
        void Synchronize(object content);
    }

    public class TextEditorSynchronizer: ISynchronize
    {
        public OTAwareEditor editor { get; set; }

        public TextEditorSynchronizer(OTAwareEditor editor)
        {
            this.editor = editor;
        }

        public void Synchronize(object content)
        {
            var stringContent = content as string;

            if (stringContent == null) throw new ArgumentException("Synchronization context is not of type string");

            using (editor.DeclareChangeBlock())
            {
                editor.Document.Text = stringContent;
            }

        }

    }
}