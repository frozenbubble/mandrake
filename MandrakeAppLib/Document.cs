using ICSharpCode.AvalonEdit;
using Mandrake.Model.Document;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Mandrake.Sample.Client.Document
{
    //[Export(typeof(IOTAwareContext))]
    public class OTAwareEditor : TextEditor, IOTAwareContext
    {
        public bool IsUpdatedByUser { get; set; }

        public OTAwareEditor()
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
}
