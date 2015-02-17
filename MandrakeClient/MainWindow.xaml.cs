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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Mandrake.Service;
using Mandrake.Management;
using Mandrake.Model.Document;
using System.ServiceModel;
using Mandrake.Management.Client;
using Mandrake.Model;
using Mandrake.Client.Base;
using Mandrake.Client.Base.OTServiceReference;
using System.Reflection;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;

namespace Mandrake.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ClientManager callback;

        public MainWindow()
        {
            InitializeComponent();

            //editor.Document.TextInput += Document_TextInput;
            //editor.TextInput += Document_TextInput;
            //editor.TextChanged += rtb_TextChanged;
            //editor.Document.DataContextChanged += Document_DataContextChanged;
            

            //callback = new ClientManager(editor);

            //editor.Document.Changed += callback.OnChange;
            
            //editor.TextChanged += callback.OnChange;

            //RichTextBox rtb = new RichTextBox();
            //rtb.TextChanged += rtb_TextChanged;
            
            //rtb.DeclareChangeBlock();
            //rtb.CaretPosition = rtb.CaretPosition.GetPositionAtOffset(1);
            //rtb.CaretPosition.InsertTextInRun("asd");
            //rtb.EndChange();

            //rtb.DeclareChangeBlock();
            //rtb.CaretPosition = rtb.CaretPosition.GetPositionAtOffset(1);
            //rtb.CaretPosition.DeleteTextInRun(1);
            //rtb.EndChange();

            //new Task( () => callback.Connect() ).Start();
        }

        void rtb_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Console.WriteLine(e.Changes.FirstOrDefault().ToString());
            var change = e.Changes.FirstOrDefault();
            var s = sender as RichTextBox;
            var textBuffer = new char[change.AddedLength];

            var start = s.CaretPosition.GetPositionAtOffset(change.Offset);
            var end = s.CaretPosition.GetPositionAtOffset(change.Offset + change.AddedLength);

            var text = new TextRange(s.Document.ContentStart, s.Document.ContentEnd);

            //var asd = s.CaretPosition.GetPositionAtOffset(change.Offset-3);
            //asd.GetTextInRun(LogicalDirection.Forward, textBuffer, change.Offset/2, change.AddedLength);
            //Console.WriteLine(textBuffer);
        }
    }
}
