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
using TechnoShieldApp.Pages.Admin;

namespace TechnoShieldApp.Pages.Manager
{
    /// <summary>
    /// Interaction logic for ManagerMenuPage.xaml
    /// </summary>
    public partial class ManagerMenuPage : Page
    {
        public ManagerMenuPage()
        {
            InitializeComponent();
        }

        private void BtnProductsAndServices_Click(object sender, RoutedEventArgs e)
        {
            AppData.MainFrame.Navigate(new ProductsAndServicesPage());
        }

        private void BtnOrders_Click(object sender, RoutedEventArgs e)
        {
            AppData.MainFrame.Navigate(new OrderListPage());
        }
    }
}
