using HtmlAgilityPack;
using Quisy.WebScrapers.Helpers;
using Quisy.WebScrapers.Models;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Quisy.WebScrapers
{
    public static class EbayWebScraper
    {
        private static string _BaseUrl = "https://www.ebay.com/sch/i.html?_nkw=";
        private static string _Delimitator = "+";
        private const int _IndexesCount = 5;

        public static Task<IEnumerable<ProductDTO>> GetProductsByQueryAsync(string query)
        {
            var doc = GetHtmlFromEbay(query);

            if (doc == null)            
                return null;

            var products = ExtractProductFromHtml(doc);            
                        
            return Task.FromResult(FormatProducts(products.ToList()));
        }

        private static IEnumerable<ProductDTO> ExtractProductFromHtml(HtmlDocument doc)
        {
            var node = HtmlNodeHelper.GetFirstByNameAndClass(doc.DocumentNode.Descendants(), "ul", "srp-results srp-list clearfix");

            var descendants = node.Descendants();

            var lis = HtmlNodeHelper.GetAllByNameAndAttribute(descendants, "li", "class", "s-item");

            var products = new List<ProductDTO>();

            for (int i = 0; i < _IndexesCount && i < lis.Count(); i++)
            {
                var product = ExtractProductDetails(lis.ElementAt(i).Descendants());
                if (product != null)
                    products.Add(product);
            }

            return products;
        }

        private static ProductDTO ExtractProductDetails(IEnumerable<HtmlNode> nodes)
        {
            var product = new ProductDTO { Source = "EBay" };

            var title = nodes.FirstOrDefault(n => n.Name == "h3");
            if (string.IsNullOrWhiteSpace(title?.InnerText))
                return null;

            product.Title = title.InnerText;
            
            var price = HtmlNodeHelper.GetFirstByNameAndClass(nodes, "span", "s-item__price");
            if (string.IsNullOrWhiteSpace(price?.InnerText))
                return null;

            product.Price = price.InnerText;

            product.Image = HtmlNodeHelper.GetFirstValueByNameAndAttribute(nodes, "img", "src");

            return product;

        }

        private static IEnumerable<ProductDTO> FormatProducts(List<ProductDTO> products)
        {
            foreach (var product in products)
            {
                product.Price = FormatPrice(product.Price);
            }
            return products;
        }

        private static string FormatPrice(string price)
        {
            if (string.IsNullOrWhiteSpace(price) || !price.Contains("$"))
                return price;
            var indexCurrencySymbol = price.IndexOf("$");
            var result = price.Substring(indexCurrencySymbol);

            if (string.IsNullOrWhiteSpace(result) || !result.Contains("."))
                return result;
            var indexDot = result.IndexOf(".");
            result = result.Substring(0, indexDot);

            return result.Replace(",", string.Empty).Replace("$", string.Empty).Replace(" ", string.Empty);
        }

        private static HtmlDocument GetHtmlFromEbay(string query)
        {
            var formatedQuery = query.Replace(" ", _Delimitator);
            var urlWithQuery = $"{_BaseUrl}{formatedQuery}";
            CultureInfo.CurrentCulture = new CultureInfo("en-US", true);

            var web = new HtmlWeb();
            var doc = web.Load(urlWithQuery);            
            return doc;
        }

        private static HtmlDocument LoadHtmlFromLocalPath()
        {
            var html = File.ReadAllText(@"C:\Shared\inner.html");
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            return doc;
        }
    }
}
