using mshtml;
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
    /// Interaction logic for PrintOrderPage.xaml
    /// </summary>
    public partial class PrintOrderPage : Page
    {
        public PrintOrderPage(Order order)
        {
            InitializeComponent();
            StringBuilder result = new StringBuilder();
            result.Append(@"<!DOCTYPE html ><html><meta http-equiv='Content-Type' content='text/html;charset=UTF-8'><head></head>");
            result.Append("<body>");
            result.Append("<h1 align=\"center\">ООО \"ТЕХНО-ЩИТ\"</h1>");
            result.Append($"<h2 align=\"center\">Заказ номер {order.Id}</h2>");
            result.Append("<table width=100% border=1 bordercolor=#000 style='border-collapse:collapse;'>");
            result.Append("<tr>");
            if (order.Purchase.Count != 0)
            {
                result.Append("<th colspan=\"7\">Товары</th></tr>");
                result.Append("<tr>");
                result.Append("<th colspan=\"3\">Название товара</th>");
                result.Append("<th>Цена</th>");
                result.Append("<th>Кол-во товара</th>");
                result.Append("<th colspan=\"2\">Итоговая цена по товару</th>");
                result.Append("</tr>");
            }
            else if (order.ServiceOfProductInOrder.Count != 0)
            {
                ServiceHead(result);
            }
            else
            {
                result.Append("<td>Нет данны</td>");
                result.Append("</tr>");
            }
            foreach (var purchase in order.Purchase)
            {
                result.Append($"<tr><td colspan=\"3\">{purchase.Product.Name}</td>");
                result.Append($"<td align=\"center\">{purchase.Product.Price}</td>");
                result.Append($"<td align=\"center\">{purchase.Count}</td>");
                result.Append($"<td align=\"center\" colspan=\"2\">{purchase.TotalPrice}</td></tr>");
            }
            if (order.Purchase.Count != 0)
            {
                result.Append("<tr><td align=\"right\" colspan=\"5\">ИТОГО (по товарам):</td>");
                result.Append($"<td colspan=\"2\">{order.Purchase.Sum(p => p.TotalPrice)}</td></tr>");
            }
            if (order.ServiceOfProductInOrder.Count != 0 && order.Purchase.Count != 0)
                ServiceHead(result);
            decimal _servicesTotalPrice = 0;
            foreach (var item in order.ServiceOfProductInOrder.ToList().OrderBy(p => p.Service.Name).ToList().GroupBy(p => p.Service))
            {
                result.Append("<tr>");
                int rowspan = order.ServiceOfProductInOrder.Count(p => p.Service == item.Key);
                result.Append($"<td rowspan=\"{rowspan}\">{item.Key.Name}</td>");
                result.Append($"<td align=\"center\" rowspan =\"{rowspan}\">{item.Key.Price}</td>");
                Product product = order.ServiceOfProductInOrder.FirstOrDefault(p => p.Service == item.Key).Product;
                AddProductsService(order, result,
                    order.ServiceOfProductInOrder.FirstOrDefault(p => p.Service == item.Key), product);
                decimal totalPriceForOneService = order.ServiceOfProductInOrder.FirstOrDefault(p => p.Service == item.Key).TotalPriceForOneService;
                _servicesTotalPrice += totalPriceForOneService;
                result.Append($"<td align=\"center\" rowspan=\"{rowspan}\">{totalPriceForOneService}</td>");
                result.Append("</tr>");
                foreach (var service in order.ServiceOfProductInOrder.Where(p => p.Service == item.Key).ToList().Skip(1).ToList())
                {
                    result.Append("<tr>");
                    AddProductsService(order, result, service, service.Product);
                    result.Append("</tr>");
                }
            }
            if (order.ServiceOfProductInOrder.Count != 0)
            {
                result.Append("<tr><td align=\"right\" colspan=\"5\">ИТОГО (по услугам):</td>");
                result.Append($"<td colspan=\"2\" align=\"left\">{_servicesTotalPrice}</td></tr>");
            }
            result.Append("<tr><td align=\"right\" colspan=\"5\">ИТОГО:</td>");
            result.Append($"<td colspan=\"2\" align=\"left\">{_servicesTotalPrice + order.Purchase.Sum(p => p.TotalPrice)}</td></tr>");
            result.Append("</table>");
            result.Append("<p>Заказчик: ________________________</p>");
            result.Append("</body>");
            result.Append("</html>");
            WebBrowserMain.NavigateToString(result.ToString());
        }

        private static void AddProductsService(Order order, StringBuilder result, ServiceOfProductInOrder serviceInOrder, Product product)
        {
            if (product != null)
            {
                result.Append($"<td >{product.Name}</td>");
                result.Append($"<td align=\"center\">{product.Price}</td>");
                result.Append($"<td align=\"center\">{serviceInOrder.Count}</td>");
                result.Append($"<td align=\"center\">{serviceInOrder.Count * product.Price}</td>");
            }
            else
            {
                result.Append($"<td align=\"center\">--</td>");
                result.Append($"<td align=\"center\">--</td>");
                result.Append($"<td align=\"center\">--</td>");
                result.Append($"<td align=\"center\">--</td>");
            }
        }

        private static void ServiceHead(StringBuilder result)
        {
            result.Append($"<th colspan=\"7\">Услуги</th></tr>");
            result.Append($"<tr>");
            result.Append("<th rowspan=\"2\">Оказываемая услуга</th>");
            result.Append("<th rowspan=\"2\">Цена за усугу</th>");
            result.Append("<th colspan=\"4\">Товары входящие в услугу</th>");
            result.Append("<th rowspan=\"2\">Итоговая цена услуги</th></tr>");
            result.Append("<tr>");
            result.Append("<th>Название товара</th>");
            result.Append("<th>Цена</th>");
            result.Append("<th>Кол-во товара</th>");
            result.Append("<th>Итоговая цена по товару</th>");
            result.Append("</tr>");
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            var document = WebBrowserMain.Document as IHTMLDocument2;
            document.execCommand("Print");
        }
    }
}
