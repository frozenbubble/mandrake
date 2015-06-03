using Mandrake.Management;
using Mandrake.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
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
using Mandrake.Host.View.Converters;
using System.Collections.ObjectModel;
using Mandrake.Sample.Client.Operations;
using MahApps.Metro.Controls;

namespace Mandrake.Host
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        internal static ServiceHost host = null;
        OTAwareService service;

        public ObservableCollection<SynchronizingConnection> Connections { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            editor.DocumentName = "New Session";

            Connections = new ObservableCollection<SynchronizingConnection>();

            if (host != null)
            {
                host.Close();
                host = null;
            }

            service = new OTAwareService();
            service.AddDocument(editor);
            
            service.OperationPerformed += service_OperationPerformed;
            service.MessageArrived += service_MessageArrived;
            service.MessageSent += service_MessageSent;
            service.RegistrationCompleted += service_RegistrationCompleted;

            ClientsList.ItemsSource = Connections;

            host = new OTServiceHost(service);
            host.Open();

            
        }

        void service_MessageArrived(object sender, OTMessage message)
        {
            Console.WriteLine("Message arrived");
        }

        void service_MessageSent(object sender, OTMessage message)
        {
            Console.WriteLine("Message sent. Content:");
            foreach (var item in message.Content)
            {
                Console.WriteLine("\t" + item);
            }
        }

        void service_RegistrationCompleted(object sender, Mandrake.Model.Operation o)
        {
            Connections.Clear();
            foreach (var item in service.Clients.Values.ToList())
            {
                Connections.Add(item);
            }
        }

        void service_OperationPerformed(object sender, Mandrake.Model.Operation o)
        {
            opLog.Dispatcher.BeginInvoke(new Action(
                () => opLog.AppendText(String.Format("{2} ({3}) [{0}]:\t {1}", DateTime.Now, o.ToString(), service.Clients[o.OwnerId].Name, o.OwnerId) + Environment.NewLine)));
        }
    }
}
