using Quisy.Tools.HttpHelpers;
using Quisy.WebScrapers.Models;
using Quisy.WebScrapers.WalmartScrapers.Models;
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
            var results = await HttpClientHelper.Get<WalmartMxSearchDto>(_UrlSearch + query);

            if (results?.Appendix?.SearchResults?.Content?.Any() != true)
                return null;

            var contents = results.Appendix.SearchResults.Content;
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
