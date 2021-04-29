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

namespace TechnoShieldApp.Pages.Manager
{
    /// <summary>
    /// Interaction logic for OrderDetailViewPage.xaml
    /// </summary>
    public partial class OrderDetailViewPage : Page
    {
        Order _order;
        public OrderDetailViewPage(Order order)
        {
            InitializeComponent();
            _order = order;
            UpdateData();
        }

        private void UpdateData()
        {
            DataContext = null;
            DataContext = _order;
            CbStatusOfOrder.ItemsSource = AppData.Context.StatusOfOrder.ToList();
            CbStatusOfOrder.SelectedItem = _order.StatusOfOrder;
        }

        private void BtnEditOrder_Click(object sender, RoutedEventArgs e)
        {
            AppData.MainFrame.Navigate(new CreateOrderPage(_order));
        }

        private void BtnEditWorkers_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateData();
        }
    }
}
