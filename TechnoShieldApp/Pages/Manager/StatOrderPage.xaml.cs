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
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TechnoShieldApp.Entities;

namespace TechnoShieldApp.Pages.Manager
{
    /// <summary>
    /// Interaction logic for StatOrderPage.xaml
    /// </summary>
    public partial class StatOrderPage : Page
    {
        private List<Organization> _listOrganization = new List<Organization>();
        public StatOrderPage(DateTime? startDateTime, DateTime? endDteTime)
        {
            InitializeComponent();
            DpStart.SelectedDate = startDateTime;
            DpEnd.SelectedDate = endDteTime;
            _listOrganization = AppData.Context.Organization.ToList();
            _listOrganization.Insert(0, new Organization
            {
                Name = "Все организации"
            });
            CbOrganization.ItemsSource = _listOrganization;
            CbOrganization.SelectedIndex = 0;
            ChartProduct.ChartAreas.Add(new ChartArea("Main"));
            Series currentSeriesProduct = new Series("Product")
            {
                IsValueShownAsLabel = true,
                YValueType = ChartValueType.Int64
            };
            ChartProduct.Series.Add(currentSeriesProduct);
            ChartOrder.ChartAreas.Add(new ChartArea("Main"));
            Series seriesPoint = new Series("OrderPoint")
            {
                IsValueShownAsLabel = true,
                YValueType = ChartValueType.Int64
            };
            Series seriesLine = new Series("OrderLine")
            {
                YValueType = ChartValueType.Int64
            };
            ChartOrder.Series.Add(seriesPoint);
            ChartOrder.Series.Add(seriesLine);
            UpdateChartProduct();
            UpdateChartOrder();
        }
        private void UpdateChartProduct()
        {
            var purchaseList = AppData.Context.Purchase.ToList();
            if (ChbServicePlus.IsChecked == true)
            {
                foreach (var item in AppData.Context.ServiceOfProductInOrder.ToList())
                {
                    if (item.Product != null)
                    {
                        purchaseList.Add(new Purchase
                        {
                            Count = item.Count,
                            Order = item.Order,
                            OrderId = item.OrderId,
                            Product = item.Product,
                            ProductId = item.Product.Id
                        });
                    }
                }
            }
            if (DpEnd.SelectedDate != null)
                purchaseList = purchaseList
                    .Where(p => p.Order.DateTimeOfCreate.Date <= DpEnd.SelectedDate.Value.Date).ToList();
            if (DpStart.SelectedDate != null)
                purchaseList = purchaseList
                    .Where(p => p.Order.DateTimeOfCreate.Date >= DpStart.SelectedDate.Value.Date).ToList();
            if (CbOrganization.SelectedIndex > 0)
                purchaseList = purchaseList
                    .Where(p => p.Order.Organization == CbOrganization.SelectedItem as Organization).ToList();
            Series currentSeries = ChartProduct.Series.FirstOrDefault();
            currentSeries.ChartType = SeriesChartType.StackedColumn;
            currentSeries.Points.Clear();
            foreach (var item in purchaseList.OrderByDescending(p=>p.Count)
                .ToList().GroupBy(p => p.Product).ToList().Take(9))
            {
                currentSeries.Points.AddXY(item.Key.Name, purchaseList
                    .Where(p => p.Product == item.Key).Sum(p => p.Count));
            }
        }
        private void UpdateChartOrder()
        {
            var orderList = AppData.Context.Order.ToList().OrderBy(p => p.DateTimeOfCreate).ToList();
            if (DpEnd.SelectedDate != null)
                orderList = orderList.Where(p => p.DateTimeOfCreate.Date <= DpEnd.SelectedDate.Value.Date).ToList();
            if (DpStart.SelectedDate != null)
                orderList = orderList.Where(p => p.DateTimeOfCreate.Date >= DpStart.SelectedDate.Value.Date).ToList();
            if (CbOrganization.SelectedIndex > 0)
                orderList = orderList
                    .Where(p => p.Organization == CbOrganization.SelectedItem as Organization).ToList();
            Series pointSeries = ChartOrder.Series.FirstOrDefault();
            Series lineSeries = ChartOrder.Series.LastOrDefault();
            lineSeries.ChartType = SeriesChartType.Spline;
            pointSeries.ChartType = SeriesChartType.Point;
            pointSeries.Points.Clear();
            lineSeries.Points.Clear();
            lineSeries.Color = System.Drawing.Color.Blue;
            if (RBtnOrderDay.IsChecked == true)
            {
                foreach (var item in orderList.GroupBy(p => p.DateTimeOfCreate.Date))
                {
                    pointSeries.Points.AddXY(item.Key.ToString("dd MMMM"),
                        orderList.Where(p => p.DateTimeOfCreate.Day == item.Key.Day).Count());
                    lineSeries.Points.AddXY(item.Key.ToString("dd MMMM"),
                        orderList.Where(p => p.DateTimeOfCreate.Day == item.Key.Day).Count());
                }
            }
            if (RBtnOrderMonth.IsChecked == true)
            {
                foreach (var item in orderList.GroupBy(p => p.DateTimeOfCreate.Date.Month))
                {
                    pointSeries.Points.AddXY(new DateTime(01, item.Key, 01).ToString("MMMM"),
                        orderList.Where(p => p.DateTimeOfCreate.Month == item.Key).Count());
                    lineSeries.Points.AddXY(new DateTime(01, item.Key, 01).ToString("MMMM"),
                        orderList.Where(p => p.DateTimeOfCreate.Month == item.Key).Count());
                }
            }
            if (RbtnOrderYear.IsChecked == true)
            {
                foreach (var item in orderList.GroupBy(p => p.DateTimeOfCreate.Date.Year))
                {
                    pointSeries.Points.AddXY(new DateTime(item.Key, 01, 01).ToString("yyyy г."),
                        orderList.Where(p => p.DateTimeOfCreate.Year == item.Key).Count());
                    lineSeries.Points.AddXY(new DateTime(item.Key, 01, 01).ToString("yyyy г."),
                        orderList.Where(p => p.DateTimeOfCreate.Year == item.Key).Count());
                }
            }
        }

        private void BtnCreateChart_Click(object sender, RoutedEventArgs e)
        {
            UpdateChartProduct();
            UpdateChartOrder();
        }

        private void BtnClearParam_Click(object sender, RoutedEventArgs e)
        {
            DpStart.SelectedDate = DpEnd.SelectedDate = null;
            CbOrganization.SelectedIndex = 0;
            UpdateChartProduct();
            UpdateChartOrder();
        }
        private void RadioButtonClick(object sender, RoutedEventArgs e)
        {
            UpdateChartOrder();
        }

        private void ChbServicePlus_Click(object sender, RoutedEventArgs e)
        {
            UpdateChartProduct();
        }

        private void BtnPrintStatd_Click(object sender, RoutedEventArgs e)
        {
            //RenderTargetBitmap rtb = new RenderTargetBitmap((int)GridChartProduct.ActualWidth, (int)GridChartProduct.ActualHeight,
            //    96d, 96d, PixelFormats.Pbgra32);
            //rtb.Render(GridChartProduct);
            //var encoder = new JpegBitmapEncoder();
            //encoder.Frames.Add(BitmapFrame.Create(rtb));
            //using (var stream = File.Create("pngProductStat.jpeg"))
            //encoder.Save(stream);
            AppData.MainFrame.Navigate(new PrintOrderStatPage(DpStart.SelectedDate, DpEnd.SelectedDate, null));
        }
    }
}
