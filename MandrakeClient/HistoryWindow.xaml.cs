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
        List<IOperationManager> ManagerChain;
        List<Operation> opsList;

        public HistoryWindow(ClientManager callback, string documentName)
        {
            InitializeComponent();

            ClientManager = callback;
            ManagerChain = new List<IOperationManager>(ClientManager.ManagerChain);
            ManagerChain.Add(new AggregateOperationManager());
            GetHistory(documentName);
        }

        private async void GetHistory(string documentName)
        {
            var log = await ClientManager.GetHistory(documentName);
            opsList = new List<Operation>(OperationCompressor.Compress(log));
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
