using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Mandrake.Management;
using Mandrake.Model.Document;
using Mandrake.Client.Base;
using Mandrake.Samples.Client.ViewModel;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Mandrake.Client;
using Microsoft.Win32;
using System.IO;
using Mandrake.Client.View;

namespace Mandrake.Samples.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private ClientManager callback;
        private MainViewModel viewModel;

        public static RoutedCommand DownloadCommand = new RoutedCommand();
        public static RoutedCommand SendCommand = new RoutedCommand();

        public MultiCaretTextEditor Editor 
        { 
            get { return editor; } 
            private set { editor = value; } 
        }

        public MainWindow()
        {
            InitializeComponent();
            editor.DocumentName = "New Session";
            DownloadCommand.InputGestures.Add(new KeyGesture(Key.D, ModifierKeys.Control));
            
            callback = new ClientManager(editor);
            this.viewModel = MainViewModel.Current;
            this.viewModel.ClientManager = callback;
            this.DataContext = viewModel;

            callback.ClientRegistered += callback_ClientRegistered;
            callback.MessageArrived += viewModel.OnMessageArrived;
        }

        void callback_ClientRegistered(object sender, ClientMetaData meta)
        {
            editor.Dispatcher.BeginInvoke(new Action(() => editor.RegisterClient(meta.Id, meta.Name)));
            ClientsList.Dispatcher.BeginInvoke(new Action(() => InvalidateProperty(ListView.ItemsSourceProperty)));
            viewModel.Register(meta);
        }

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string username = viewModel.Username = await this.ShowInputAsync("Alias", "What's your name?");

            if (username != null) Connect(username); 
        }

        private async void Connect(string username)
        {
            await viewModel.Connect();
            Register(editor);
        }

        private void ChatButton_Click(object sender, RoutedEventArgs e)
        {
            if (SideBar.Visibility == Visibility.Visible) SideBar.Visibility = System.Windows.Visibility.Collapsed;

            else SideBar.Visibility = System.Windows.Visibility.Visible;
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            var historyWindow = new HistoryWindow(callback, editor.DocumentName);
            historyWindow.Show();
        }

        internal void ChangeEditor(MultiCaretTextEditor newDocument)
        {
            ContentGrid.Children.Remove(editor);
            UnRegister(editor);
            
            editor = newDocument;
            foreach (var client in callback.Clients) editor.RegisterClient(client.Id, client.Name);
            
            Register(editor);
            ContentGrid.Children.Add(editor);
            Grid.SetRow(editor, 0);
        }

        internal void Register(MultiCaretTextEditor document)
        {
            document.DocumentChanged += callback.OnChange;
            document.CaretPositionChanged += callback.OnChange;
            document.SelectionChanged += callback.OnChange; ;
        }

        private void UnRegister(MultiCaretTextEditor document)
        {
            document.DocumentChanged -= callback.OnChange;
            document.CaretPositionChanged -= callback.OnChange;
            document.SelectionChanged -= callback.OnChange;
        }

        private async void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string documentName = await this.ShowInputAsync("Document name", "What's the name of the document you would like to create?");
            if (documentName != null && documentName != "")
            {
                var newDocument = (await callback.CreateDocumentAsync(documentName)) as MultiCaretTextEditor;
                if (newDocument != null) ChangeEditor(newDocument);
            }
        }

        private void Default_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.ShowDialog();

            if (dialog.FileName != null && !dialog.FileName.Equals("")) File.WriteAllText(dialog.FileName, editor.Text);
        }

        private async void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();

            if (dialog.ShowDialog() == true)
            {
                var content = File.ReadAllText(dialog.FileName);
                var upLoadedDocument = await callback.UploadDocumentAsync(dialog.SafeFileName, content) as MultiCaretTextEditor;

                if (upLoadedDocument != null) ChangeEditor(upLoadedDocument);
            }
        }

        private void FindCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new FindAndReplaceWindow(editor.TextArea);
            dialog.Show();
        }

        private void CopyCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, editor.SelectedText);
        }

        private void PasteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var text = Clipboard.GetData(DataFormats.Text) as string;
            editor.Paste(text);
        }

        private async void DownloadCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var docs = await callback.GetAvailableDocumentsAsync();
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
                var downloadedDocument = (await callback.OpenDocumentAsync(docName)) as MultiCaretTextEditor;
                if (downloadedDocument != null) ChangeEditor(downloadedDocument);
            }

        }

        private void SendCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Message.Text.Length == 0) return;

            viewModel.SendMessage(Message.Text);
            MessageBox.Dispatcher.BeginInvoke(new Action(() => InvalidateProperty(ListView.ItemsSourceProperty)));

            Message.Clear();
        }
    }
}
