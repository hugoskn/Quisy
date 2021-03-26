using Quisy.SqlDbRepository;
using Quisy.SqlDbRepository.Entities;
using Quisy.Tools.HttpHelpers;
using Quisy.WebScrapers.Models;
using Quisy.WebScrapers.WalmartScrapers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quisy.WebScrapers.WalmartScrapers
{
    public static class WalmartMexicoWebScraper
    {
        private static string _BaseUrl = "https://www.walmart.com.mx";
        private static string _UrlSearch = $"{_BaseUrl}/api/v2/page/search?Ntt=";
        private const int _IndexesCount = 5;

        public static async Task<IEnumerable<ProductDTO>> GetProductsByQueryAsync(string query)
        {
            try
            {
                var results = await HttpClientHelper.Get<WalmartMxSearchDto>(_UrlSearch + query);

                if (results?.Appendix?.SearchResults?.Content?.Any() != true)
                    return Enumerable.Empty<ProductDTO>();

                var contents = results.Appendix.SearchResults.Content;
                var products = MapProducts(contents);

                return products;
            }
            catch (Exception ex)
            {
                QuisyDbRepository.AddLog(LogType.Exception,
                    $"Exception at {nameof(WalmartMexicoWebScraper)}, method: {nameof(WalmartMexicoWebScraper.GetProductsByQueryAsync)}. " +
                    $"Query: {query}. Message {ex.Message}");
                return Enumerable.Empty<ProductDTO>();
            }
        }

        private static List<ProductDTO> MapProducts(Content[] contents)
        {
            var products = new List<ProductDTO>();
            for (int i = 0; i < contents.Length && i < _IndexesCount; i++)
            {
                products.Add(new ProductDTO
                {
                    Image = _BaseUrl + contents[i].imageUrls?.small,
                    Title = contents[i].skuDisplayName,
                    Price = ((int?)contents[i].skuPrice)?.ToString(),
                    Link = _BaseUrl + contents[i].productSeoUrl,
                    Source = "Walmart"
                });
            }

            return products;
        }
    }
}
