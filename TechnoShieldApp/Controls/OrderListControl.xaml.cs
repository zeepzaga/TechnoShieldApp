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
using TechnoShieldApp.Entities;
using TechnoShieldApp.Pages.Manager;

namespace TechnoShieldApp.Controls
{
    /// <summary>
    /// Interaction logic for OrderListControl.xaml
    /// </summary>
    public partial class OrderListControl : UserControl
    {
        public OrderListControl()
        {
            InitializeComponent();
        }

        private void BtnAbouOrder_Click(object sender, RoutedEventArgs e)
        {
            AppData.MainFrame.Navigate(new OrderDetailViewPage((sender as Button).DataContext as Order));
        }
    }
}
