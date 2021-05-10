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
            UpdateOrderReadyList();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            UpdateOrderList();
        }

        private void UpdateOrderList()
        {
            List<Order> _searchList = _listOrders.OrderByDescending(p=>p.DateTimeOfCreate).ToList();
            if (DpEnd.SelectedDate != null)
                _searchList = _searchList.Where(p => p.DateTimeOfCreate.Date <= DpEnd.SelectedDate.Value).ToList();
            if (DpStart.SelectedDate != null)
                _searchList = _searchList.Where(p => p.DateTimeOfCreate.Date >= DpStart.SelectedDate.Value).ToList();
            if (CbOrganization.SelectedIndex > 0)
                _searchList = _searchList.Where(p => p.Organization == CbOrganization.SelectedItem as Organization).ToList();
            IcOrders.ItemsSource = null;
            IcOrders.ItemsSource = _searchList;
        }

        private void UpdateOrderReadyList()
        {
            List<Order> _searchList = _listOrders.Where(p => p.StatusOfOrderId == 4).ToList().OrderBy(p => p.DateTimeOfEnd).ToList();
            if (DpEnd.SelectedDate != null)
                _searchList = _searchList.Where(p => p.DateTimeOfEnd.Value <= DpEnd.SelectedDate.Value).ToList();
            if (DpStart.SelectedDate != null)
                _searchList = _searchList.Where(p => p.DateTimeOfEnd.Value >= DpStart.SelectedDate.Value).ToList();
            if (CbOrganization.SelectedIndex > 0)
                _searchList = _searchList.Where(p => p.Organization == CbOrganization.SelectedItem as Organization).ToList();
            IcOrdersReady.ItemsSource = null;
            IcOrdersReady.ItemsSource = _searchList;
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
            AppData.MainFrame.Navigate(new StatOrderPage(DpStart.SelectedDate, DpEnd.SelectedDate));
        }

        private void BtnSearchReady_Click(object sender, RoutedEventArgs e)
        {
            UpdateOrderReadyList();
        }

        private void BtnClearSearchingReady_Click(object sender, RoutedEventArgs e)
        {
            DpStartReadyOrder.SelectedDate = DpEndReadyOrder.SelectedDate = null;
            CbOrganizationEndReadyOrder.SelectedIndex = 0;
            UpdateOrderReadyList();
        }
    }
}
