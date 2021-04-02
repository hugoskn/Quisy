using Quisy.SqlDbRepository;
using Quisy.SqlDbRepository.Entities;
using Quisy.Tools.HttpHelpers;
using Quisy.WebScrapers.Models;
using Quisy.WebScrapers.TargetScrapers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quisy.WebScrapers.TargetScrapers
{
    public class TargetWebApiScraper
    {
        private static string _BaseUrl = "https://redsky.target.com";
        private static string _UrlSearch = $"{_BaseUrl}/redsky_aggregations/v1/web/plp_search_v1?" +
            "key=ff457966e64d5e877fdbad070f276d18ecec4a01" +
            "&channel=WEB" +
            "&count={0}" +
            "&default_purchasability_filter=true" +
            "&keyword={1}" +
            "&offset=0" +
            "&page=%2Fs%2F{1}" +
            "&pricing_store_id=1783" +
            "&visitor_id=0178945F999802018A6CD6F8D9CAC355";
        private const int _IndexesCount = 5;

        public static async Task<IEnumerable<ProductDTO>> GetProductsByQueryAsync(string query)
        {
            var url = string.Format(_UrlSearch, _IndexesCount, query.Replace(" ", "+"));
            try
            {
                var results = await HttpClientHelper.Get<TargetSearchResultDto>(url);
                var targetProducts = results?.Data.Search?.Products;
                if (targetProducts?.Any() != true)
                    return Enumerable.Empty<ProductDTO>();

                var products = MapProducts(targetProducts);
                return products;
            }
            catch (Exception ex)
            {
                await QuisyDbRepository.AddLogAsync(LogType.Exception,
                    $"Exception at {nameof(TargetWebApiScraper)}, method: {nameof(TargetWebApiScraper.GetProductsByQueryAsync)}. " +
                    $"Query: {query}. Message {ex.Message}");
                return Enumerable.Empty<ProductDTO>();
            }
        }

        private static List<ProductDTO> MapProducts(TargetProductDto[] targetProducts)
        {
            var products = new List<ProductDTO>();
            for (int i = 0; i < targetProducts.Length && i < _IndexesCount; i++)
            {
                products.Add(new ProductDTO
                {
                    Image = targetProducts[i].Item?.Enrichment?.Images?.primary_image_url,
                    Title = targetProducts[i].Item?.product_description?.Title,
                    Price = ((int?)targetProducts[i].Price?.current_retail)?.ToString(),
                    Link = _BaseUrl + targetProducts[i].Item?.Enrichment?.buy_url,
                    Source = "Target"
                });
            }

            return products;
        }
    }
}
