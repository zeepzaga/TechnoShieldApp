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
        private List<Organization> _listOrganization = new List<Organization>();
        public OrderListPage()
        {
            InitializeComponent();
            _listOrders = AppData.Context.Order.ToList().OrderByDescending(p => p.DateTimeOfCreate).ToList();
            _listOrganization = AppData.Context.Organization.ToList();
            _listOrganization.Insert(0, new Organization
            {
                Name = "Все органиации"
            });
            CbOrganization.ItemsSource = _listOrganization;
            CbOrganization.SelectedIndex = 0;
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
                _searchList = _searchList.Where(p => p.DateTimeOfCreate.Date <= DpEnd.SelectedDate.Value).ToList();
            if (DpStart.SelectedDate != null)
                _searchList = _searchList.Where(p => p.DateTimeOfCreate.Date >= DpStart.SelectedDate.Value).ToList();
            if (CbOrganization.SelectedIndex > 0)
                _searchList = _searchList.Where(p => p.Organization == CbOrganization.SelectedItem as Organization).ToList();
            IcOrders.ItemsSource = null;
            IcOrders.ItemsSource = _searchList;
        }

        private void BtnClearSearching_Click(object sender, RoutedEventArgs e)
        {
            DpStart.SelectedDate = DpEnd.SelectedDate = null;
            CbOrganization.SelectedIndex = 0;
            UpdateOrderList();
        }

        private void BtnCreateOrder_Click(object sender, RoutedEventArgs e)
        {
            AppData.MainFrame.Navigate(new CreateOrderPage(null));
        }

        private void BtnStatOrder_Click(object sender, RoutedEventArgs e)
        {
            if (DpEnd.SelectedDate != null || DpStart.SelectedDate != null)
            {
                if (DpEnd.SelectedDate != null && DpStart.SelectedDate != null)
                    AppData.MainFrame.Navigate(new StatOrderPage(DpStart.SelectedDate.Value, DpEnd.SelectedDate.Value));
                else if (DpStart.SelectedDate!=null)
                    AppData.MainFrame.Navigate(new StatOrderPage(DpStart.SelectedDate.Value, null));
                else if (DpEnd.SelectedDate != null)
                    AppData.MainFrame.Navigate(new StatOrderPage(null, DpEnd.SelectedDate.Value));
            }
            else
            {
                AppData.MainFrame.Navigate(new StatOrderPage(null, null));
            }


        }
    }
}
