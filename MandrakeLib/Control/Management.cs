using ICSharpCode.AvalonEdit.Document;
using Mandrake.Model;
using Mandrake.Model.Document;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;


namespace Mandrake.Management
{
    public delegate void OperationActionEventHandler(object sender, Operation o);

    public abstract class OTManager
    {
        protected int serverMessages;

        [Import(typeof(ISynchronize))]
        protected ISynchronize syncManager;
        
        [Import(typeof(ITransform))]
        protected ITransform transformer;

        [Import(typeof(IOTAwareContext))]
        public IOTAwareContext Context { get; set; }
        
        [ImportMany(typeof(IOperationManager))]
        public IEnumerable<IOperationManager> ManagerChain { get; set; }
        
        
        public List<Operation> Log { get; protected set; }

        protected abstract void Transform(Operation o);
        protected abstract void Execute(Operation o);
    }


    public interface IOperationManager
    {
        Operation TryRecognize(object sender, EventArgs e);
        bool TryExecute(object context, Operation o);
    }

    

    public interface ISynchronize
    {
        void Synchronize(object content);
    }

    //public class TextEditorSynchronizer: ISynchronize
    //{
    //    public OTAwareEditor editor { get; set; }

    //    public TextEditorSynchronizer(OTAwareEditor editor)
    //    {
    //        this.editor = editor;
    //    }

    //    public void Synchronize(object content)
    //    {
    //        var stringContent = content as string;

    //        if (stringContent == null) throw new ArgumentException("Synchronization context is not of type string");

    //        using (editor.DeclareChangeBlock())
    //        {
    //            editor.Document.Text = stringContent;
    //        }

    //    }

    //}
}