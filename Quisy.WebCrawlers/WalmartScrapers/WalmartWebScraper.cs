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

namespace Quisy.WebScrapers.WalmartScrapers
{
    public static class WalmartWebScraper
    {
        private static string _BaseUrl = "https://www.walmart.com";
        private static string _UrlWithQuery = $"{_BaseUrl}/search/?query=";
        private const int _IndexesCount = 5;

        public static Task<IEnumerable<ProductDTO>> GetProductsByQueryAsync(string query)
        {
            if (RegionInfo.CurrentRegion.TwoLetterISORegionName == "MX")
                return WalmartMexicoWebScraper.GetProductsByQueryAsync(query);

            var products = ExtractProductsFromDefaultWalmart(query);

            return Task.FromResult(FormatProducts(products.ToList()));
        }

        private static IEnumerable<ProductDTO> ExtractProductsFromDefaultWalmart(string query)
        {
            try
            {
                var doc = GetHtmlFromWalmart(_UrlWithQuery, query);

                if (doc == null)
                    return Enumerable.Empty<ProductDTO>();
                return MapProducts(doc);
            }
            catch (Exception ex)
            {
                QuisyDbRepository.AddLog(LogType.Exception,
                    $"Exception at {nameof(WalmartWebScraper)}, method: {nameof(WalmartWebScraper.ExtractProductsFromDefaultWalmart)}. " +
                    $"Query: {query}. Message {ex.Message}");
                return Enumerable.Empty<ProductDTO>();
            }
        }

        private static IEnumerable<ProductDTO> MapProducts(HtmlDocument doc)
        {
            var products = new List<ProductDTO>();

            for (int i = 0; i < _IndexesCount; i++)
            {
                var li = HtmlNodeHelper.GetFirstByNameAndAttribute(
                    doc.DocumentNode.Descendants(), "li", "data-tl-id", $"ProductTileGridView-{i}");
                var product = ExtractProductDetails(li.Descendants());
                if (product != null)
                    products.Add(product);
            }

            return products;
        }

        private static ProductDTO ExtractProductDetails(IEnumerable<HtmlNode> nodes)
        {
            var product = new ProductDTO { Source = "Walmart" };

            var title = HtmlNodeHelper.GetFirstByNameAndAttribute(nodes, "a", "data-type", "itemTitles");
            if (string.IsNullOrWhiteSpace(title?.InnerText))
                return null;

            product.Title = title.InnerText;

            var price = HtmlNodeHelper.GetFirstByNameAndClass(nodes, "span", "price-characteristic");
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
                product.Title = product.Title.FormatHtml();
            }
            return products;
        }

        private static HtmlDocument GetHtmlFromWalmart(string url, string query)
        {
            return new HtmlWeb().Load($"{url}{query}");
        }

        private static HtmlDocument LoadHtmlFromLocalPath()
        {
            var html = File.ReadAllText(@"C:\Shared\walmart us.html");
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            return doc;
        }
    }
}
