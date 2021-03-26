using Quisy.Tools.HttpHelpers;
using Quisy.WebScrapers.Models;
using Quisy.WebScrapers.SamsScrapers.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quisy.WebScrapers.SamsScrapers
{
    public class SamsScraper
    {
        private static string _BaseUrl = "https://www.samsclub.com";
        private static string _UrlSearch = $"{_BaseUrl}/api/node/vivaldi/v1/search-suggestions?num=8&pnum={_IndexesCount}&v=1&cnum=0&term=";
        private const int _IndexesCount = 5;

        public static async Task<IEnumerable<ProductDTO>> GetProductsByQueryAsync(string query)
        {
            var results = await HttpClientHelper.Get<SamsSearchDto>(_UrlSearch + query);

            if (results?.Response?.Products?.Any() != true)
                return null;

            var samsProducts = results?.Response?.Products;
            var products = new List<ProductDTO>();
            for (int i = 0; i < samsProducts.Length && i < _IndexesCount; i++)
            {
                products.Add(new ProductDTO
                {
                    Image = samsProducts[i].thumb_image,
                    Title = samsProducts[i].title,
                    Price = ((int?)samsProducts[i].sale_price)?.ToString(),
                    Link = _BaseUrl + samsProducts[i].url,
                    Source = "Sams"
                });
            }

            return products;
        }
    }
}
