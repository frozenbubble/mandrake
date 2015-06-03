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
using MahApps.Metro.Controls.Dialogs;
using System.Windows.Input;
using Mandrake.Client.View;
using Microsoft.Win32;
using System.IO;
using Mandrake.Client;

namespace Mandrake.Samples.Client.ViewModel
{
    public class MainViewModel: DependencyObject
    {

        public static RoutedCommand DownloadCommand = new RoutedCommand();
        public static RoutedCommand SendCommand = new RoutedCommand();

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

        public MainWindow Window { get; set; }

        public static readonly DependencyProperty UsernameProperty;

        public string Username
        {
            set { SetValue(UsernameProperty, value); }
            get { return (string)GetValue(UsernameProperty); }
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

        static MainViewModel()
        {
            FrameworkPropertyMetadata metadata = new FrameworkPropertyMetadata(
                "", FrameworkPropertyMetadataOptions.None);
            
            UsernameProperty = DependencyProperty.Register("Username",
                typeof(string), typeof(MainViewModel));

            DownloadCommand.InputGestures.Add(new KeyGesture(Key.D, ModifierKeys.Control));
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

        public async Task Connect()
        {
            Username = await Window.ShowInputAsync("Alias", "What's your name?");

            if (Username != null)
            {
                var controller = await Window.ShowProgressAsync("Please wait...", "Connecting");
                await ClientManager.Connect(Username);
                await controller.CloseAsync();
            }
        }

        public async void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string documentName = await Window.ShowInputAsync("Document name", "What's the name of the document you would like to create?");
            if (documentName != null && documentName != "")
            {
                var newDocument = (await ClientManager.CreateDocumentAsync(documentName)) as MultiCaretTextEditor;
                if (newDocument != null) Window.ChangeEditor(newDocument);
            }
        }

        internal void Default_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;//(ClientManager == null);
        }

        public void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.ShowDialog();

            if (dialog.FileName != null && !dialog.FileName.Equals("")) File.WriteAllText(dialog.FileName, Window.Editor.Text);
        }

        public async void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();

            if (dialog.ShowDialog() == true)
            {
                var content = File.ReadAllText(dialog.FileName);
                var upLoadedDocument = await ClientManager.UploadDocumentAsync(dialog.SafeFileName, content) as MultiCaretTextEditor;

                if (upLoadedDocument != null) Window.ChangeEditor(upLoadedDocument);
            }
        }

        public void FindCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new FindAndReplaceWindow(Window.Editor.TextArea);
            dialog.Show();
        }

        private void CopyCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, Window.Editor.SelectedText);
        }

        private void PasteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var text = Clipboard.GetData(DataFormats.Text) as string;
            Window.Editor.Paste(text);
        }

        private async void DownloadCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var docs = await ClientManager.GetAvailableDocumentsAsync();
            var documentsWindow = new DocumentsWindow(docs);
            documentsWindow.Closed += documentsWindow_Closed;

            documentsWindow.Show();
        }

        private async void documentsWindow_Closed(object sender, EventArgs e)
        {
            var documentsWindow = sender as DocumentsWindow;
            var docName = documentsWindow.SelectedDocument;

            if (docName != null && docName != "")
            {
                var downloadedDocument = (await ClientManager.OpenDocumentAsync(docName)) as MultiCaretTextEditor;
                if (downloadedDocument != null) Window.ChangeEditor(downloadedDocument);
            }

        }

        private void SendCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Window.Message.Text.Length == 0) return;

            SendMessage(Window.Message.Text);
            Window.Message.Clear();
        }
    }
}
