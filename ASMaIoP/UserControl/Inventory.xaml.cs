using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ASMaIoP.UserControl
{
    /// <summary>
    /// Логика взаимодействия для Inventory.xaml
    /// </summary>
    public partial class Inventory : System.Windows.Controls.UserControl
    {
        public Inventory()
        {
            InitializeComponent();
            asd();

        }
        public void asd()
        {
            TreeViewItem ads = new TreeViewItem();
            ads.Header = "asdas";
            ads.Items.Add("asdas");
            YourItems.Items.Add(ads);

        }
    }
}
