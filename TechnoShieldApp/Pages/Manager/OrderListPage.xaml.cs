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
    /// Interaction logic for OrderListPage.xaml
    /// </summary>
    public partial class OrderListPage : Page
    {
        List<Order> _listOrders = new List<Order>();
        public OrderListPage()
        {
            InitializeComponent();
            _listOrders = AppData.Context.Order.ToList();
            UpdateOrderList();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            UpdateOrderList();
        }

        private void UpdateOrderList()
        {
            List<Order> _searchList = _listOrders;
            if (DpEnd.SelectedDate != null)
            {
                _searchList = _searchList.Where(p => p.DateTimeOfCreate.Date <= DpEnd.SelectedDate.Value).ToList();
            }
            if (DpStart.SelectedDate != null)
            {
                _searchList = _searchList.Where(p => p.DateTimeOfCreate.Date >= DpStart.SelectedDate.Value).ToList();
            }
            IcOrders.ItemsSource = null;
            IcOrders.ItemsSource = _searchList.Where(p => p.Organization.Name.ToLower().Trim()
            .Contains(TbCompanyName.Text.ToLower().Trim())).ToList();
        }

        private void BtnClearSearching_Click(object sender, RoutedEventArgs e)
        {
            DpStart.SelectedDate = DpEnd.SelectedDate = null;
            TbCompanyName.Text = "";
            UpdateOrderList();
        }

        private void BtnCreateOrder_Click(object sender, RoutedEventArgs e)
        {
            AppData.MainFrame.Navigate(new CreateOrderPage(null));
        }
    }
}
