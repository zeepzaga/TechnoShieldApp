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
            if (_order.Worker.ToList().Count == 0)
                TblWorkerInformation.Text = "Исполнителей заказа нет";
            else
                TblWorkerInformation.Text = "Информация об исполнителях закза";
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
            AppData.MainFrame.Navigate(new WorkerOfOrderPage(_order));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateData();
        }

        private void BtnSaveStatus_Click(object sender, RoutedEventArgs e)
        {
            StatusOfOrder statusOfOrder = (CbStatusOfOrder.SelectedItem as StatusOfOrder);
            if (statusOfOrder.Name == "Завершён" && DpDatOfEnd.SelectedDate == null)
            {
                MessageBox.Show("Чтобы завершить заказ, необходимо выбрать дату завершеия",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if(statusOfOrder.Name == "Завершён")
            {
                _order.StatusOfOrder = statusOfOrder;
                _order.DateTimeOfEnd = DpDatOfEnd.SelectedDate;
            }
            else
            {
                _order.StatusOfOrder = statusOfOrder;
                _order.DateTimeOfEnd = DpDatOfEnd.SelectedDate;
            }
            AppData.Context.SaveChanges();
        }

        private void CbStatusOfOrder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if((CbStatusOfOrder.SelectedItem as StatusOfOrder).Name == "Завершён")
            {
                DpDatOfEnd.SelectedDate = DateTime.Now;
                DpDatOfEnd.IsEnabled = true;
            }
            else
            {
                DpDatOfEnd.SelectedDate = null;
                DpDatOfEnd.IsEnabled = false;
            }
        }

        private void BtnPrintOrder_Click(object sender, RoutedEventArgs e)
        {
            AppData.MainFrame.Navigate(new PrintOrderPage(_order));
        }
    }
}
