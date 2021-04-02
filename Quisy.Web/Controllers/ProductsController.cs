using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Quisy.SqlDbRepository;
using Quisy.WebScrapers.AmazonScrapers;
using Quisy.WebScrapers.BestBuyScrapers;
using Quisy.WebScrapers.CostcoScrapers;
using Quisy.WebScrapers.EbayScrapers;
using Quisy.WebScrapers.Helpers;
using Quisy.WebScrapers.Models;
using Quisy.WebScrapers.SamsScrapers;
using Quisy.WebScrapers.TargetScrapers;
using Quisy.WebScrapers.WalmartScrapers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quisy.Api.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private IMemoryCache _cache;
        public ProductsController(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        [HttpGet("{provider}")]
        public async Task<IActionResult> Get(string provider, string query)
        {
            if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(query))
                return BadRequest();

            QuisyDbRepository.AddLogAsync(SqlDbRepository.Entities.LogType.Information, "Query: " + query);

            var cacheKey = BuildCacheKey(provider, query);
            if (_cache.TryGetValue<IEnumerable<ProductDTO>>(cacheKey, out var products))
                return Ok(products);

            if (provider.ToLower() == "all")
                products = await ScrapersFacade.GetAllProductsFromScrapersAsync(query);
            else
                products = await GetByProviderAsync(provider, query);

            if (products?.Any() == true)
                _cache.Set(cacheKey, products);
            return Ok(products);
        }

        private async Task<IEnumerable<ProductDTO>> GetByProviderAsync(string provider, string query)
        {
            var products = Enumerable.Empty<ProductDTO>();
            switch (provider.ToLower())
            {
                case "amazon":
                    products = await AmazonWebScraper.GetProductsByQueryAsync(query);
                    return products;
                case "bestbuy":
                    products = await BestBuyWebScraper.GetProductsByQueryAsync(query);
                    return products;
                case "costco":
                    products = await CostcoWebScraper.GetProductsByQueryAsync(query);
                    return products;
                case "ebay":
                    products = await EbayWebScraper.GetProductsByQueryAsync(query);
                    return products;
                case "sams":
                    products = await SamsWebApiScraper.GetProductsByQueryAsync(query);
                    return products;
                case "target":
                    products = await TargetWebApiScraper.GetProductsByQueryAsync(query);
                    return products;
                case "walmart":
                    products = await WalmartWebScraper.GetProductsByQueryAsync(query);
                    return products;
                default:
                    return products;
            }
        }

        private string BuildCacheKey(string provider, string query)
        {
            var queryWordsSorted = query.Split(" ").OrderBy(q => q);
            query = string.Join("+", queryWordsSorted);
            return $"Products_{provider.ToLower()}_{query.ToLower()}";
        }
    }
}
