using HtmlAgilityPack;
using Quisy.SqlDbRepository;
using Quisy.SqlDbRepository.Entities;
using Quisy.Tools.Extensions;
using Quisy.WebScrapers.Helpers;
using Quisy.WebScrapers.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Quisy.WebScrapers.BestBuyScrapers
{
    public static class BestBuyWebScraper
    {
        private static string _BaseUrl = "https://www.bestbuy.com";
        private static string _UrlWithQueryParam = $"{_BaseUrl}/site/searchpage.jsp?intl=nosplash&st=";
        private static string _Delimitator = "+";
        private const int _IndexesCount = 5;

        public static async Task<IEnumerable<ProductDTO>> GetProductsByQueryAsync(string query)
        {
            try
            {
                var doc = GetHtmlFromBestBuy(query);
                if (doc == null)
                    return Enumerable.Empty<ProductDTO>();
                var products = ExtractProductFromHtml(doc);
                return FormatProducts(products.ToList());
            }
            catch (Exception ex)
            {
                await QuisyDbRepository.AddLogAsync(LogType.Exception,
                    $"Exception at {nameof(BestBuyWebScraper)}, method: {nameof(BestBuyWebScraper.GetProductsByQueryAsync)}. " +
                    $"Query: {query}. Message {ex.Message}");
                return Enumerable.Empty<ProductDTO>();
            }
        }

        private static IEnumerable<ProductDTO> ExtractProductFromHtml(HtmlDocument doc)
        {
            var node = HtmlNodeHelper.GetFirstByNameAndClass(doc.DocumentNode.Descendants(), "ol", "sku-item-list");

            var descendants = node.Descendants();

            var lis = HtmlNodeHelper.GetAllByNameAndAttribute(descendants, "li", "class", "sku-item");

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
            var product = new ProductDTO { Source = "BestBuy" };

            var title = HtmlNodeHelper.GetFirstByNameAndClass(nodes, "h4", "sku-header"); ;
            if (string.IsNullOrWhiteSpace(title?.InnerText))
                return null;

            product.Title = title.InnerText;

            var price = HtmlNodeHelper.GetFirstByNameAndClass(nodes, "div", "priceView-hero-price priceView-customer-price");
            if (string.IsNullOrWhiteSpace(price?.InnerText))
                return null;

            product.Price = price.InnerText;

            product.Image = HtmlNodeHelper.GetFirstValueByNameAndAttribute(nodes, "img", "src");

            product.Link = _BaseUrl + HtmlNodeHelper.GetFirstValueByNameAndAttribute(nodes, "a", "href");

            return product;

        }

        private static IEnumerable<ProductDTO> FormatProducts(List<ProductDTO> products)
        {
            foreach (var product in products)
            {
                product.Price = product.Price.FormatCurrency();
                product.Title = product.Title.FormatHtml();
            }
            return products;
        }

        private static HtmlDocument GetHtmlFromBestBuy(string query)
        {
            var formatedQuery = query.Replace(" ", _Delimitator);
            var urlWithQuery = $"{_UrlWithQueryParam}{formatedQuery}";
            CultureInfo.CurrentCulture = new CultureInfo("en-US", true);

            var web = new HtmlWeb();
            var doc = web.Load(urlWithQuery);
            return doc;
        }
    }
}
