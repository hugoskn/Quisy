using Microsoft.AspNetCore.Mvc;
using Quisy.WebScrapers.AmazonScrapers;
using Quisy.WebScrapers.EbayScrapers;
using Quisy.WebScrapers.Models;
using Quisy.WebScrapers.WalmartScrapers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quisy.Api.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        [HttpGet("{provider}")]
        public async Task<IActionResult> Get(string provider, string query)
        {
            if (string.IsNullOrWhiteSpace(provider))
                return BadRequest();

            if (provider.ToLower() == "all")
                return Ok(await GetAllProductsFromScrapersAsync(query));

            IEnumerable<ProductDTO> products;
            switch (provider.ToLower())
            {
                case "amazon":
                    products = await AmazonWebScraper.GetProductsByQueryAsync(query);
                    return Ok(products);
                case "ebay":
                    products = await EbayWebScraper.GetProductsByQueryAsync(query);
                    return Ok(products);
                default:
                    return NotFound("Products Provider not found");
            }            
        }

        private async Task<IEnumerable<ProductDTO>> GetAllProductsFromScrapersAsync(string query)
        {
            var getAmazonProductsTask = AmazonWebScraper.GetProductsByQueryAsync(query);
            var getEbayProductsTask = EbayWebScraper.GetProductsByQueryAsync(query);
            var getWalmartProductsTask = WalmartWebScraper.GetProductsByQueryAsync(query);
            var products = new List<ProductDTO>();
            products.AddRange(await getAmazonProductsTask);
            products.AddRange(await getEbayProductsTask);
            products.AddRange(await getWalmartProductsTask);

            return products;
        }
    }
}
