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

namespace TechnoShieldApp.Controls
{
    /// <summary>
    /// Interaction logic for ProductControl.xaml
    /// </summary>
    public partial class ProductControl : UserControl
    {
        public ProductControl()
        {
            InitializeComponent();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            AppData.MainFrame.Navigate(new AddProductPage((sender as Button).DataContext as Product));
        }

        //private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    Product product = ((sender as TextBlock).DataContext as Product);
        //    if (MessageBox.Show($"Удалить товар:\n{product.Name}", "Вопрос", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        //    {
        //        AppData.Context.ProductOfService.RemoveRange(AppData.Context.ProductOfService.ToList().Where(p => p.Product == product));
        //        AppData.Context.Product.Remove(product);
        //        AppData.Context.SaveChanges();
        //    }
        //}
    }
}
