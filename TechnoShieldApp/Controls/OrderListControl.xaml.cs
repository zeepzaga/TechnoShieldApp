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

        private void MainGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            TblOrganization.Width = (sender as Grid).ActualWidth - 1010;
            if (MainGrid.ActualWidth < 1201) TblOrganization.Width += 130;
            else if (MainGrid.ActualWidth < 1310) TblOrganization.Width += 100;
            else if (MainGrid.ActualWidth < 1340) TblOrganization.Width += 80;
        }
    }
}
