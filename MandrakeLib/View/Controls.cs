using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mandrake.Model.Document
{
    public interface IOTAwareContext
    {
        //string ContextId { get; set; }
        string Name { get; set; }
        bool IsUpdatedByUser { get; set; }
        void InsertText(string text, int position);
        void RemoveText(int position, int length);
    }
}
