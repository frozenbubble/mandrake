using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mandrake.Model;
using Mandrake.Model.Document;
using Mandrake.Service;
using System.ServiceModel;
using Mandrake.Client.Base;
using Mandrake.Management;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mandrake.Samples.Client.ViewModel
{
    internal class MainViewModel
    {
        private static MainViewModel instance;

        public ClientManager ClientManager { get; set; }
        public ObservableCollection<ClientMetaData> Clients { get; set; }
        public ObservableCollection<ChatMessage> Messages { get; set; }
        public MainViewModel Current { get; set; }

        public MainViewModel(ClientManager callback)
        {
            ClientManager = callback;
            Messages = new ObservableCollection<ChatMessage>();
            Clients = new ObservableCollection<ClientMetaData>();
            instance = this;
        }

        public void SendMessage(string message)
        {
            Messages.Add(ClientManager.SendChatMessage(message));
        }

        public void OnMessageArrived(object sender, ChatMessage msg)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => Messages.Add(msg)));
        }



        internal void Register(ClientMetaData meta)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => Clients.Add(meta)));
        }
    }
}
