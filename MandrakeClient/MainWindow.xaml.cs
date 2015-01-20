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
using Mandrake.View.Controls;
using System.ServiceModel;
using Mandrake.Management.Client;
using Mandrake.Client.OTServiceReference;
using Mandrake.Model;
using ServiceModelEx;

namespace Mandrake.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ClientManager callback;

        OTAwareServiceClient proxy;
        public MainWindow()
        {
            InitializeComponent();

            callback = new ClientManager(editor);
            //callback = new ClientManager(editor);

            callback.ManagerChain.Add(new EditorInsertOperationManager());
            callback.ManagerChain.Add(new EditorDeleteOperationManager());
            editor.Document.Changed += callback.OnChange;

            Task t = new Task(() =>
            {
                var ic = new InstanceContext(callback);
                proxy = new OTAwareServiceClient(ic);
                proxy.AddGenericResolver();

                callback.Service = proxy;
                callback.Id = Guid.NewGuid();

                proxy.Register(callback.Id);
                proxy.Hello("Hello there!");
            });

            t.Start();
        }
    }
}
