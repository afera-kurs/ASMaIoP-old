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
using ASMaIoP.UserControl;

namespace ASMaIoP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public void toolBarUse()
        {
            MessageBox.Show("фывфыв");
        }

        public MainWindow()
        {
            InitializeComponent();
            GUI.Content = new WaitLogins(this);
        }

        public void WindowEnabled()
        {
            allMenu.IsEnabled = true;
            GUI.Content = null;
        }

        private void MyProfile_Click(object sender, RoutedEventArgs e)
        {

        }
        private void CreateProfile_Click(object sender, RoutedEventArgs e)
        {

        }
        private void List_Click(object sender, RoutedEventArgs e)
        {

        }
        private void CreateList_Click(object sender, RoutedEventArgs e)
        {

        }
        private void ThisBar_CLick1(object sender, RoutedEventArgs e)
        {

        }
        private void ThisBar_CLick2(object sender, RoutedEventArgs e)
        {

        }
    }
}

