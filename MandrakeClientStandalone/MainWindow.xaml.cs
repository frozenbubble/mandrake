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
using Mandrake.Management.Client;
using Mandrake.Model;
using Mandrake.Client.Base;
using Mandrake.Client.Base.OTServiceReference;
using System.Reflection;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;

namespace Mandrake.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private ClientManager callback;

        public MainWindow()
        {
            InitializeComponent();

            //callback = new ClientManager(editor);
            //editor.Document.Changed += callback.OnChange;

            //Task t = new Task(() => { });
            //t.Start();
            //callback.Connect();
            //new Task( () => callback.Connect() ).Start();
        }
    }
}
