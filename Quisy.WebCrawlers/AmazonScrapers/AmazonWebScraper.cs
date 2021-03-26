using HtmlAgilityPack;
using Quisy.SqlDbRepository;
using Quisy.SqlDbRepository.Entities;
using Quisy.Tools.Extensions;
using Quisy.WebScrapers.Helpers;
using Quisy.WebScrapers.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Quisy.WebScrapers.AmazonScrapers
{
    public static class AmazonWebScraper
    {
        private static string _BaseUrl = "https://amazon.com";
        private static string _UrlWithParam = $"{_BaseUrl}/s?k=";
        private static string _Delimitator = "+";
        private const int _IndexesCount = 5;

        public static Task<IEnumerable<ProductDTO>> GetProductsByQueryAsync(string query)
        {
            try
            {
                var doc = GetHtmlFromAmazon(query);
                if (doc == null)
                    return Task.FromResult(Enumerable.Empty<ProductDTO>());
                var products = GetProductsDetails(doc);
                return Task.FromResult(FormatProducts(products));
            }
            catch (Exception ex)
            {
                QuisyDbRepository.AddLog(LogType.Exception, 
                    $"Exception at {nameof(AmazonWebScraper)}, method: {nameof(AmazonWebScraper.GetProductsByQueryAsync)}. " +
                    $"Query: {query}. Message {ex.Message}");
                return Task.FromResult(Enumerable.Empty<ProductDTO>());
            }            
        }

        private static List<ProductDTO> GetProductsDetails(HtmlDocument doc)
        {
            var products = new List<ProductDTO>();
            for (int i = 0; i <= _IndexesCount; i++)
            {
                var product = ExtractProductFromHtml(doc, i);
                if (product != null)
                    products.Add(product);
            }

            return products;
        }

        private static ProductDTO ExtractProductFromHtml(HtmlDocument doc, int index)
        {
            var product = new ProductDTO { Source = "Amazon" };

            var node = HtmlNodeHelper.GetFirstByNameAndAttribute(doc.DocumentNode.Descendants(), "div", "data-index", index.ToString());

            if (node == null || !node.InnerHtml.Contains("price") && !node.OuterHtml.Contains("price"))
                return null;

            var descendants = node.Descendants();

            var span = HtmlNodeHelper.GetFirstByNameAndClass(descendants, "span", "a-size-medium a-color-base a-text-normal");

            if (string.IsNullOrWhiteSpace(span?.InnerText) || span.InnerHtml.Length <= 1)
                span = HtmlNodeHelper.GetFirstByNameAndClass(descendants, "span", "a-size-base-plus a-color-base a-text-normal");
            if (string.IsNullOrWhiteSpace(span?.InnerText))
                return null;

            product.Title = span.InnerText;

            product.Price = HtmlNodeHelper.GetFirstByNameAndClass(descendants, "span", "a-price-whole")?.InnerText;
            if (product.Price == null)
                return null;

            product.Image = HtmlNodeHelper.GetFirstValueByNameAndAttribute(descendants, "img", "src");

            product.Link = HttpUtility.HtmlDecode(_BaseUrl + HtmlNodeHelper.GetFirstValueByNameAndAttribute(descendants, "a", "href"));

            return product;
        }

        private static IEnumerable<ProductDTO> FormatProducts(List<ProductDTO> products)
        {
            foreach (var product in products)
            {
                product.Price = product.Price.FormatCurrency();
            }
            return products;
        }

        private static HtmlDocument GetHtmlFromAmazon(string query)
        {
            var formatedQuery = query.Replace(" ", _Delimitator);
            var urlWithQuery = $"{_UrlWithParam}{formatedQuery}";
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
