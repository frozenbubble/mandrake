using ICSharpCode.AvalonEdit.Document;
using Mandrake.Model;
using Mandrake.Model.Document;
using ServiceModelEx;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;


namespace Mandrake.Management
{
    public delegate void OperationActionEventHandler(object sender, Operation o);

    public abstract class OTManager
    {
        protected int serverMessages;

        //[Import(typeof(ISynchronize))]
        protected ISynchronize syncManager;

        [Import(typeof(ITransform))]
        public ITransform transformer { get; set; }

        //[Import(typeof(IOTAwareContext))]
        public IOTAwareContext Context { get; set; }
        
        [ImportMany]
        public IEnumerable<IOperationManager> ManagerChain { get; set; }
        
        
        public List<Operation> Log { get; protected set; }

        public OTManager()
        {
            Log = new List<Operation>();
            var catalog = new AggregateCatalog();

            Array.ForEach(AppDomain.CurrentDomain.GetAssemblies(), x => catalog.Catalogs.Add(new AssemblyCatalog(x)));
            if (Assembly.GetEntryAssembly() == null)
            {
                Array.ForEach(GenericResolverInstaller.GetWebAssemblies(), x => catalog.Catalogs.Add(new AssemblyCatalog(x)));
            }

            var container = new CompositionContainer(catalog);

            try
            {
                container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
            }
        }

        protected abstract void Transform(Operation o);
        protected abstract void Execute(Operation o);
    }


    public interface IOperationManager
    {
        Operation TryRecognize(object sender, EventArgs e);
        bool TryExecute(object context, Operation o);
    }

    public interface IOperationManagerMetaData
    {
        Type OperationType { get; }
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