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

        private ClientManager clientManager;

        public ClientManager ClientManager
        {
            get { return clientManager; }
            set 
            { 
                clientManager = value;
                Id = ClientManager.Id;
            }
        }

        public ObservableCollection<ClientMetaData> Clients { get; set; }
        public ObservableCollection<ChatMessage> Messages { get; set; }
        public Guid Id { get; private set; }
        
        private static MainViewModel current;

        public static MainViewModel Current
        {
            get 
            {
                if (current == null) current = new MainViewModel();
                return current; 
            }
        }


        private MainViewModel()
        {
            Messages = new ObservableCollection<ChatMessage>();
            Clients = new ObservableCollection<ClientMetaData>();
        }

        public void SendMessage(string message)
        {
            Messages.Add(clientManager.SendChatMessage(message));
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
