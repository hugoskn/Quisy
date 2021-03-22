using HtmlAgilityPack;
using Quisy.WebScrapers.Helpers;
using Quisy.WebScrapers.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Quisy.WebScrapers.Amazon
{
    public static class AmazonWebScraper
    {
        private static string _BaseUrl = "https://www.amazon.com/s?k=";
        private static string _Delimitator = "+";
        private const int _IndexesCount = 20;

        public static IEnumerable<ProductDTO> GetProductsByQuery(string query)
        {
            var doc = LoadHtmlFromLocalPath();

            if (doc == null)            
                return null;

            if (doc.DocumentNode.OuterHtml.Contains("mexico", StringComparison.InvariantCultureIgnoreCase))
                return null;

            var products = new List<ProductDTO>();
            for (int i = 0; i <= _IndexesCount; i++)
            {
                var product = new ProductDTO();

                var node = HtmlNodeHelper.GetFirstByNameAndAttribute(doc.DocumentNode.Descendants(), "div", "data-index", i.ToString());
                
                if (node == null || !node.InnerHtml.Contains("price") && !node.OuterHtml.Contains("price"))
                    continue;

                var descendants = node.Descendants();

                var span = HtmlNodeHelper.GetFirstByNameAndClass(descendants, "span", "a-size-medium a-color-base a-text-normal");

                if (string.IsNullOrWhiteSpace(span?.InnerText) || span.InnerHtml.Length <= 1)                
                    span = HtmlNodeHelper.GetFirstByNameAndClass(descendants, "span", "a-size-base-plus a-color-base a-text-normal");
                if (string.IsNullOrWhiteSpace(span?.InnerText))
                    continue;

                product.Title = span.InnerText;

                product.Price = HtmlNodeHelper.GetFirstByNameAndClass(descendants, "span", "a-price-whole")?.InnerText;
                if (product.Price == null)
                    continue;

                product.Image = HtmlNodeHelper.GetFirstValueByNameAndAttribute(descendants, "img", "src");

                products.Add(product);
            }
                        
            return FormatProducts(products);
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
            if (string.IsNullOrWhiteSpace(price) || !price.Contains("."))
                return price;
            var indexDot = price.IndexOf(".");
            var result = price.Substring(0, indexDot);
            return result;
        }

        private static HtmlDocument GetHtmlFromAmazon(string query)
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
