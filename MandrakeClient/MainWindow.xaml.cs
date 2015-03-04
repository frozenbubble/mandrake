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
using Mandrake.Model;
using Mandrake.Client.Base;
using Mandrake.Client.Base.OTServiceReference;
using System.Reflection;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using Mandrake.Samples.Client.ViewModel;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace Mandrake.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private ClientManager callback;
        private MainViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
            
            callback = new ClientManager(editor);
            this.viewModel = new MainViewModel(callback);
            this.DataContext = viewModel;

            editor.DocumentChanged += callback.OnChange;
            editor.CaretPositionChanged += callback.OnChange;
            editor.SelectionChanged += callback.OnChange;
            callback.ClientRegistered += callback_ClientRegistered;
            callback.MessageArrived += viewModel.OnMessageArrived;
        }

        void callback_ClientRegistered(object sender, ClientMetaData meta)
        {
            editor.Dispatcher.BeginInvoke(new Action(() => editor.RegisterClient(meta.Id, meta.Name)));
            ClientsList.Dispatcher.BeginInvoke(new Action(() => InvalidateProperty(ListView.ItemsSourceProperty)));
            viewModel.Register(meta);
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void SendMessage()
        {
            viewModel.SendMessage(Message.Text);
            MessageBox.Dispatcher.BeginInvoke(new Action(() => InvalidateProperty(ListView.ItemsSourceProperty)));

            Message.Clear();
        }

        private void Message_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) SendMessage();
        }

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string name = await this.ShowInputAsync("Alias", "What's your name?");

            new Task(() => callback.Connect(name)).Start();
        }
    }
}
