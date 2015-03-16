﻿using System;
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
using Mandrake.Client;
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

        public MainWindow()
        {
            InitializeComponent();
            
            callback = new ClientManager(editor);
            this.viewModel = MainViewModel.Current;
            this.viewModel.ClientManager = callback;
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
            string username = viewModel.Username = await this.ShowInputAsync("Alias", "What's your name?");

            new Task(() => callback.Connect(username)).Start();
        }

        private void ChatButton_Click(object sender, RoutedEventArgs e)
        {
            if (SideBar.Visibility == Visibility.Visible) SideBar.Visibility = System.Windows.Visibility.Collapsed;

            else SideBar.Visibility = System.Windows.Visibility.Visible;
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, editor.SelectedText);
        }

        private void PasteButton_Click(object sender, RoutedEventArgs e)
        {
            var text = Clipboard.GetData(DataFormats.Text) as string;
            editor.Paste(text);
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            var historyWindow = new HistoryWindow(callback);
            historyWindow.Show();
        }

        private async void NewButton_Click(object sender, RoutedEventArgs e)
        {
            string documentName = viewModel.Username = await this.ShowInputAsync("Document name", "What's the name of the document you would like to create?");

            editor = (MultiCaretTextEditor)await callback.CreateDocument(documentName);
        }
    }
}
