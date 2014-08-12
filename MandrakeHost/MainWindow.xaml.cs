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

namespace Mandrake.Host
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal static ServiceHost host = null;
        OTAwareService service;

        public ObservableCollection<SynchronizingConnection> Connections { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Connections = new ObservableCollection<SynchronizingConnection>();

            if (host != null)
            {
                host.Close();
                host = null;
            }

            service = new OTAwareService();
            service.Context = editor;

            List<IOperationManager> chain = new List<IOperationManager>();
            chain.Add(new EditorInsertOperationManager());
            chain.Add(new EditorDeleteOperationManager());
            service.ManagerChain = chain;

            service.OperationArrived += service_OperationArrived;
            service.OperationPerformed += service_OperationPerformed;
            service.OperationSent += service_OperationSent;
            service.RegistrationCompleted += service_RegistrationCompleted;
            

            host = new ServiceHost(service);
            host.Open();
        }

        void service_RegistrationCompleted(object sender, Mandrake.Model.Operation o)
        {
            Connections.Clear();
            foreach (var item in service.Clients.Values.ToList())
            {
                Connections.Add(item);
            }
        }

        void service_OperationSent(object sender, Mandrake.Model.Operation o)
        {
            throw new NotImplementedException();
        }

        void service_OperationPerformed(object sender, Mandrake.Model.Operation o)
        {
            opLog.AppendText(String.Format("[{0}]: {1} from client with Id: {2}", DateTime.Now, o.ToString(), o.OwnerId) + Environment.NewLine);
        }

        void service_OperationArrived(object sender, Mandrake.Model.Operation o)
        {
            throw new NotImplementedException();
        }
    }
}
