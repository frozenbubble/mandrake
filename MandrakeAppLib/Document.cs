using ICSharpCode.AvalonEdit;
using Mandrake.Model.Document;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mandrake.Sample.Client.Document
{
    public class OTAwareEditor : TextEditor, IOTAwareContext
    {
        public bool IsUpdatedByUser { get; set; }

        public OTAwareEditor()
            : base()
        {
            IsUpdatedByUser = true;
        }


        public void InsertText(string text, int position)
        {
            using (this.DeclareChangeBlock())
            {
                Document.Insert(position, text);
            }
        }

        public void RemoveText(int position, int length)
        {
            using (this.DeclareChangeBlock())
            {
                Document.Remove(position, length);
            }
        }
    }

    [Export(typeof(IOTAwareContext))]
    public class OTAwareDocument : IOTAwareContext
    {
        public bool IsUpdatedByUser { get; set; }
        public StringBuilder document { get; set; }

        public OTAwareDocument()
        {
            IsUpdatedByUser = true;
            this.document = new StringBuilder();
        }

        public void InsertText(string text, int position)
        {
            document.Insert(position, text);
        }

        public void RemoveText(int position, int length)
        {
            document.Remove(position, length);
        }
    }
}
