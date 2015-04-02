using ICSharpCode.AvalonEdit;
using Mandrake.Client.View;
using Mandrake.Management;
using Mandrake.Model.Document;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Mandrake.Sample.Client.Document
{
    [Export(typeof(IDocumentFactory))]
    public class MultiCaretEditorFactory: IDocumentFactory
    {
        public IOTAwareContext CreateDocument(string name)
        {
            MultiCaretTextEditor doc = null;

            Application.Current.Dispatcher.Invoke(() => doc = new MultiCaretTextEditor() { DocumentName = name });

            return doc;
        }
    }

    [Export(typeof(ISynchronize))]
    public class MultiCaretTextEditorSynchronizer: ISynchronize
    {
        public object GetContent(IOTAwareContext ctx)
        {
            var context = ctx as MultiCaretTextEditor;

            return context.Editor.Text;
        }

        public void SetContent(IOTAwareContext ctx, object value)
        {
            var context = ctx as MultiCaretTextEditor;
            var content = value as string;

            context.Clear();
            context.InsertText(content, 0);
        }
    }
}
