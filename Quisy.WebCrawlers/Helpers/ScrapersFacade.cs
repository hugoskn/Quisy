using Quisy.WebScrapers.AmazonScrapers;
using Quisy.WebScrapers.BestBuyScrapers;
using Quisy.WebScrapers.CostcoScrapers;
using Quisy.WebScrapers.EbayScrapers;
using Quisy.WebScrapers.Models;
using Quisy.WebScrapers.SamsScrapers;
using Quisy.WebScrapers.TargetScrapers;
using Quisy.WebScrapers.WalmartScrapers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quisy.WebScrapers.Helpers
{
    public static class ScrapersFacade
    {
        public static async Task<IEnumerable<ProductDTO>> GetAllProductsFromScrapersAsync(string query)
        {
            var getAmazonProductsTask = AmazonWebScraper.GetProductsByQueryAsync(query);
            var getBestBuyProductsTask = BestBuyWebScraper.GetProductsByQueryAsync(query);
            var getCostcoProductsTask = CostcoWebScraper.GetProductsByQueryAsync(query);
            var getEbayProductsTask = EbayWebScraper.GetProductsByQueryAsync(query);
            var getSamsProductsTask = SamsWebApiScraper.GetProductsByQueryAsync(query);
            var getTargetProductsTask = TargetWebApiScraper.GetProductsByQueryAsync(query);
            var getWalmartProductsTask = WalmartWebScraper.GetProductsByQueryAsync(query);
            var products = new List<ProductDTO>();
            products.AddRange(await getAmazonProductsTask);
            products.AddRange(await getBestBuyProductsTask);
            products.AddRange(await getCostcoProductsTask);
            products.AddRange(await getEbayProductsTask);
            products.AddRange(await getSamsProductsTask);
            products.AddRange(await getTargetProductsTask);
            products.AddRange(await getWalmartProductsTask);

            return products;
        }
    }
}
