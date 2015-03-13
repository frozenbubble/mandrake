using MahApps.Metro.Controls;
using Mandrake.Client.Base;
using Mandrake.Management;
using Mandrake.Model;
using Mandrake.Sample.Client.Util;
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
    /// Interaction logic for HistoryWindow.xaml
    /// </summary>
    public partial class HistoryWindow : MetroWindow
    {
        ClientManager ClientManager;
        IEnumerable<IOperationManager> ManagerChain;
        List<Operation> opsList;

        public HistoryWindow(ClientManager callback)
        {
            InitializeComponent();

            ClientManager = callback;
            ManagerChain = ClientManager.ManagerChain;
            GetHistory();
        }

        private async void GetHistory()
        {
            var log = await ClientManager.GetHistory();
            opsList = new List<Operation>(OperationCompressor.Compress(log));
            //opsList = new List<Operation>(log);
            Log.ItemsSource = opsList;
        }

        private void Log_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Context.Clear();

            int last = Log.SelectedIndex;
            for (int i = 0; i <= last; i++)
            {
                foreach (var manager in ManagerChain)
                {
                    manager.TryExecute(Context, opsList[i]);
                }
            }
        }
    }
}
