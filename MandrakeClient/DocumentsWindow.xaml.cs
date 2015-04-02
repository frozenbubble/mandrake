using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Mandrake.Client
{
    /// <summary>
    /// Interaction logic for DocumentsWindow.xaml
    /// </summary>
    public partial class DocumentsWindow : MetroWindow
    {
        public string SelectedDocument { get; set; }
        public List<string> Documents { get; set; }

        public DocumentsWindow(IEnumerable<string> documents)
        {
            InitializeComponent();
            DocumentList.ItemsSource = Documents = documents.ToList();
        }

        public void HandleDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectedDocument = SelectedDocument = Documents[DocumentList.SelectedIndex];
            this.Close();
        }
    }
}
