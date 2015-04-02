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
        string DocumentName { get; set; }
        bool IsUpdatedByUser { get; set; }
        List<Operation> Log { get; set; }
        int ServerMessages { get; set; }
        int ClientMessages { get; set; }
    }

    public interface IDocumentFactory
    {
        IOTAwareContext CreateDocument(string name);
    }
}
