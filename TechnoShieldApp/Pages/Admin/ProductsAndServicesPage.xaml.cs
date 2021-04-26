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

namespace TechnoShieldApp.Pages.Admin
{
    /// <summary>
    /// Interaction logic for ProductsAndServicesPage.xaml
    /// </summary>
    public partial class ProductsAndServicesPage : Page
    {
        List<Product> _listProduct = new List<Product>();
        List<Product> _searchList = new List<Product>();
        List<TypeOfProduct> _listTypeOfProduct = new List<TypeOfProduct>();
        List<IGrouping<TypeOfService, TypeOfService>> _serviceList;
        decimal _pageCountProduct = 0;
        decimal _pageNowProduct = 0;
        int _productOnPage = 50;
        public ProductsAndServicesPage()
        {
            InitializeComponent();
            try
            {
                _listTypeOfProduct = AppData.Context.TypeOfProduct.ToList();
                _serviceList = AppData.Context.TypeOfService.ToList().GroupBy(p => p).ToList();
                _listTypeOfProduct.Insert(0, new TypeOfProduct
                {
                    Name = "Все типы"
                });
                CbTypeOfProduct.ItemsSource = _listTypeOfProduct;
                CbTypeOfProduct.SelectedIndex = 0;
                _listProduct = AppData.Context.Product.ToList();
                UpdateListServce();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к БД\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            AppData.MainFrame.Navigate(new AddProductPage(null));
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateListsProduct();
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Product product = ((sender as TextBlock).DataContext as Product);
            if (MessageBox.Show($"Удалить товар:\n{product.Name}", "Вопрос", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                AppData.Context.Product.Remove(product);
                AppData.Context.SaveChanges();
                IcProducts.ItemsSource = null;
                _listProduct = AppData.Context.Product.ToList();
                UpdateListsProduct();
            }
        }
        private void PageFlippingProduct(decimal newPage, List<Product> products)
        {
            ScrollProduct.ScrollToHome();
            IcProducts.ItemsSource = null;
            BtnFirstPageProduct.IsEnabled = BtnLastPageProduct.IsEnabled = BtnPreviousPageProduct.IsEnabled
                = BtnNextPageProduct.IsEnabled = true;
            IcProducts.ItemsSource = products.Skip((Convert.ToInt32(newPage) - 1) * _productOnPage).Take(_productOnPage).ToList().GroupBy(p => p.TypeOfProduct.Name).ToList();
            if (products.Count == 0)
            {
                _pageNowProduct = 0;
                BtnFirstPageProduct.IsEnabled = BtnLastPageProduct.IsEnabled =
                    BtnPreviousPageProduct.IsEnabled = BtnNextPageProduct.IsEnabled = false;
            }
            TblPage.Text = $"{_pageNowProduct} / {_pageCountProduct}";
            CheckPageProduct();
        }
        private void CheckPageProduct()
        {
            if (_pageNowProduct >= _pageCountProduct) BtnNextPageProduct.IsEnabled = false;
            else BtnNextPageProduct.IsEnabled = true;
            if (_pageNowProduct <= 1) BtnPreviousPageProduct.IsEnabled = false;
            else BtnPreviousPageProduct.IsEnabled = true;
        }
        private void UpdateListsProduct()
        {
            _searchList = _listProduct;
            if (CbTypeOfProduct.SelectedIndex > 0)
            {
                _searchList = _listProduct.Where(p => p.TypeOfProduct == CbTypeOfProduct.SelectedItem as TypeOfProduct).ToList();
            }
            _searchList =
                _searchList.Where(p => p.Name.ToUpper().Contains(TbSearchProduct.Text.ToUpper())).ToList();
            _pageNowProduct = 1;
            _pageCountProduct = Math.Ceiling((decimal)_searchList.Count / _productOnPage);
            TblTotalPatients.Text = $"Всего товаров: {_searchList.Count}";
            PageFlippingProduct(_pageNowProduct, _searchList);
        }
        private void BtnPreviousPageProduct_Click(object sender, RoutedEventArgs e)
        {
            _pageNowProduct--;
            PageFlippingProduct(_pageNowProduct, _searchList);
        }

        private void BtnNextPageProduct_Click(object sender, RoutedEventArgs e)
        {
            _pageNowProduct++;
            PageFlippingProduct(_pageNowProduct, _searchList);
        }

        private void BtnLastPageProduct_Click(object sender, RoutedEventArgs e)
        {
            _pageNowProduct = _pageCountProduct;
            PageFlippingProduct(_pageNowProduct, _searchList);
        }


        private void BtnFirstPageProduct_Click(object sender, RoutedEventArgs e)
        {
            _pageNowProduct = 1;
            PageFlippingProduct(_pageNowProduct, _searchList);
        }

        private void TbSearchProduct_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateListsProduct();
        }

        private void CbTypeOfProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateListsProduct();
        }

        private void BtnEditService_Click(object sender, RoutedEventArgs e)
        {
            Service service = (sender as Button).DataContext as Service;
            AppData.MainFrame.Navigate(new AddServicePage(service));
        }

        private void TblRow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock textBlock = (sender as TextBlock);
            var grid = textBlock.Parent as Grid;
            var control = (grid.Children[2] as ItemsControl);
            if (control.Visibility == Visibility.Visible)
            {
                textBlock.Text = "\xE70D"; //Стрелка вниз
                control.Visibility = Visibility.Collapsed;
            }
            else
            {
                textBlock.Text = "\xE70E"; // Верх
                control.Visibility = Visibility.Visible;
            }
        }

        private void BtnDeleteproductOnService_Click(object sender, RoutedEventArgs e)
        {
            ((sender as Button).DataContext as Product).TypeOfService = null;
            AppData.Context.SaveChanges();
            (((sender as Button).Parent as StackPanel).Parent as Border).Visibility = Visibility.Collapsed;
        }

        private void TbSearhTypeOfService_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateListServce();           
        }
        private void UpdateListServce()
        {
            ICService.ItemsSource = null;
            ICService.ItemsSource = _serviceList.Where(p=>p.Key.Name.ToLower().Contains(TbSearhTypeOfService.Text.ToLower()));
        }
    }
}
