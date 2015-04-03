using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Search;
using MahApps.Metro.Controls;
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
    /// Interaction logic for FindAndReplaceWindow.xaml
    /// </summary>
    public partial class FindAndReplaceWindow : MetroWindow
    {
        public SearchPanel Finder { get; set; }
        public TextArea TextArea { get; set; }
        public FindAndReplaceWindow(TextArea textarea)
        {
            InitializeComponent();

            Finder = SearchPanel.Install(textarea);
            TextArea = textarea;
            Finder.Open();
            Finder.Visibility = Visibility.Hidden;

            //Finder.MarkerBrush = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
        }

        private void Pattern_TextChanged(object sender, TextChangedEventArgs e)
        {
            Finder.SearchPattern = Pattern.Text;
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            Finder.FindPrevious();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            Finder.FindNext();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Finder.Close();
            Finder.Uninstall();
        }
    }
}
