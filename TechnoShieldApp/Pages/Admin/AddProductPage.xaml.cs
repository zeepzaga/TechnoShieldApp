using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for AddProductPage.xaml
    /// </summary>
    public partial class AddProductPage : Page
    {
        private byte[] _photo = null;
        Product _product;
        public AddProductPage(Product product)
        {
            InitializeComponent();
            try
            {
                CbTypeOfService.ItemsSource = AppData.Context.TypeOfService.ToList();
                CbTypeOfProduct.ItemsSource = AppData.Context.TypeOfProduct.ToList();
            }
            catch
            {
                MessageBox.Show("Ошибка подключения к БД", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            _product = product;
            if (product != null)
            {
                _photo = _product.Photo;

                BtnSave.Content = "Редактировать";
                CbTypeOfService.SelectedItem = _product.TypeOfService;
                this.Title = "Редактирование товара";
                this.DataContext = product;
                CbTypeOfProduct.SelectedItem = product.TypeOfProduct;
                if (product.Photo != null)
                {
                    TbPath.Text = "selected photo";
                    TblDeletePhoto.Visibility = Visibility.Visible;
                }
            }

        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(TbName.Text) || CbTypeOfProduct.SelectedIndex < 0)
            {
                MessageBox.Show("Название и тип товара обязательны для заполнения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                if (_product == null)
                {
                    var product = AppData.Context.Product.Add(new Product
                    {
                        Description = String.IsNullOrWhiteSpace(TbDescription.Text) ? null : TbDescription.Text,
                        Name = TbName.Text,
                        TypeOfProduct = CbTypeOfProduct.SelectedItem as TypeOfProduct,
                        Price = decimal.Parse(TbPrice.Text),
                        Photo = _photo,
                        TypeOfService = CbTypeOfService.SelectedItem as TypeOfService != null ? CbTypeOfService.SelectedItem as TypeOfService : null
                    });
                    AppData.Context.SaveChanges();

                    MessageBox.Show("Товар успешно добавлен", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    _product.Description = String.IsNullOrWhiteSpace(TbDescription.Text) ? null : TbDescription.Text;
                    _product.Name = TbName.Text;
                    _product.TypeOfProduct = CbTypeOfProduct.SelectedItem as TypeOfProduct;
                    _product.Price = decimal.Parse(TbPrice.Text);
                    _product.Photo = _photo;
                    _product.TypeOfService = CbTypeOfService.SelectedItem as TypeOfService != null ? CbTypeOfService.SelectedItem as TypeOfService : null;

                    AppData.Context.SaveChanges();
                    MessageBox.Show("Товар успешно отредактирован", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                AppData.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Отменить действия?", "Вопрос", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                AppData.MainFrame.GoBack();
            }
        }

        private void BtnPhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Multiselect = false,
                Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.png) | *.jpg; *.jpeg; *.jpe; *.png"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                _photo = File.ReadAllBytes(openFileDialog.FileName);
                TbPath.Text = openFileDialog.SafeFileName;
                ImgProduct.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                TblDeletePhoto.Visibility = Visibility.Visible;
            }
        }

        private void TextBlock_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Удалить фото продукта", "Вопрос", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _photo = null;
                ImgProduct.Source = null;
                TblDeletePhoto.Visibility = Visibility.Collapsed;
                TbPath.Text = "";
            }
        }

        private void ImgProduct_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            if (ImgProduct.Source != null)
                TblDeletePhoto.Visibility = Visibility.Visible;
            else
                TblDeletePhoto.Visibility = Visibility.Collapsed;
        }
    }
}