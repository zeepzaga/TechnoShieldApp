using mshtml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TechnoShieldApp.Entities;

namespace TechnoShieldApp.Pages.Manager
{
    /// <summary>
    /// Interaction logic for PrintOrderStatPage.xaml
    /// </summary>
    public partial class PrintOrderStatPage : Page
    {
        public PrintOrderStatPage(DateTime? startDate, DateTime? endDate, Organization organization)
        {
            InitializeComponent();
            List<Order> _listOrder = new List<Order>();
            _listOrder = AppData.Context.Order.ToList();
            StringBuilder result = new StringBuilder();
            result.Append(@"<!DOCTYPE html ><html><meta http-equiv='Content-Type' content='text/html;charset=UTF-8'><head></head>");
            result.Append("<body>");
            string titile = "Статистика по заказам";
            if (startDate != null)
            {
                titile += $" с {startDate.Value.ToShortDateString()}";
                _listOrder = _listOrder.Where(p => p.DateTimeOfCreate.Date >= startDate.Value.Date).ToList();
            }
            if (endDate != null)
            {
                titile += $" по {endDate.Value.ToShortDateString()}";
                _listOrder = _listOrder.Where(p => p.DateTimeOfCreate.Date <= endDate.Value.Date).ToList();
            }
            if (organization != null)
            {
                titile += $"\n Организация: {organization.Name}";
                _listOrder = _listOrder.Where(p => p.Organization == organization).ToList();
            }
            result.Append($"<h1 align=\"center\">{titile}</h1>");
            result.Append($"<h3>Заказы</h3>");
            result.Append("<table width=100% border=1 bordercolor=#000 style='border-collapse:collapse;'>");
            result.Append("<tr>");
            result.Append("<th>Организация-заказчик</th>");
            result.Append("<th>Дата заказа</th>");
            result.Append("<th>Статус заказа</th>");
            result.Append("<th>Товары и услуги заказа</th>");
            result.Append("</tr>");
            foreach (var item in _listOrder)
            {
                result.Append("<tr>");
                int rowspan = item.Purchase.Count() + item.ServiceOfProductInOrder.GroupBy(p => p.Service).Count();
                if (item.Purchase.Count != 0) rowspan++;
                if (item.ServiceOfProductInOrder.Count != 0) rowspan++;
                result.Append($"<td rowspan=\"{rowspan}\">" +
                    $"{item.Organization.Name}</td>");
                result.Append($"<td rowspan=\"{rowspan}\">" +
                    $"{item.DateTimeOfCreate.Date.ToShortDateString()}</td>");
                result.Append($"<td rowspan=\"{rowspan}\">" +
                    $"{item.StatusOfOrder.Name}</td>");
                string serviceProduct = "";
                ServiceOfProductInOrder firstServiceOfProductInOrder = null;
                if (item.Purchase.Count != 0)
                {
                    result.Append("<th>Товары</th></tr>");
                }
                else if (item.ServiceOfProductInOrder.Count != 0)
                {
                    result.Append("<th>Услуги</th></tr>");
                }
                else
                {
                    result.Append("<td>Нет данны</td>");
                    result.Append("</tr>");
                }
                foreach (var purchase in item.Purchase)
                {
                    result.Append($"<tr><td>{purchase.Product.Name} ({purchase.Count})</td></tr>");
                }
                if (item.ServiceOfProductInOrder.Count != 0 && item.Purchase.Count != 0)
                    result.Append("<tr><th>Услуги</th></tr>");
                serviceProduct = "";
                foreach (var service in item.ServiceOfProductInOrder.ToList().OrderBy(p=>p.Service.Name).ToList())
                {
                    if (firstServiceOfProductInOrder == null)
                    {
                        firstServiceOfProductInOrder = service;
                        serviceProduct = $"{firstServiceOfProductInOrder.Service.Name}";
                        serviceTableOpen(result, serviceProduct);
                    }
                    else
                    {
                        if (service.Service != firstServiceOfProductInOrder.Service)
                        {
                            result.Append("</p></td></tr>");
                            serviceProduct = $"{firstServiceOfProductInOrder.Service.Name}";
                            firstServiceOfProductInOrder = service;
                            serviceTableOpen(result, serviceProduct);
                        }
                    }
                    if (service.Product != null)
                    {
                        result.Append($"<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                            $"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{service.Product.Name} " +
                            $"({service.Count})");
                    }
                }
            }
            result.Append("</table>");
            result.Append("</body>");
            result.Append("</html>");
            WebBrowserMain.NavigateToString(result.ToString());
        }

        private static void serviceTableOpen(StringBuilder result, string serviceProduct)
        {
            result.Append($"<tr><td><p>");
            result.Append($"• {serviceProduct}");
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            var document = WebBrowserMain.Document as IHTMLDocument2;
            document.execCommand("Print");
        }
    }
}