using Microsoft.AspNetCore.Mvc;
using Quisy.WebScrapers.AmazonScrapers;
using Quisy.WebScrapers.CostcoScrapers;
using Quisy.WebScrapers.EbayScrapers;
using Quisy.WebScrapers.Helpers;
using Quisy.WebScrapers.Models;
using Quisy.WebScrapers.SamsScrapers;
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
            if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(query))
                return BadRequest();

            if (provider.ToLower() == "all")
                return Ok(await ScrapersFacade.GetAllProductsFromScrapersAsync(query));

            IEnumerable<ProductDTO> products;
            switch (provider.ToLower())
            {
                case "amazon":
                    products = await AmazonWebScraper.GetProductsByQueryAsync(query);
                    return Ok(products);
                case "costco":
                    products = await CostcoWebScraper.GetProductsByQueryAsync(query);
                    return Ok(products);
                case "ebay":
                    products = await EbayWebScraper.GetProductsByQueryAsync(query);
                    return Ok(products);
                case "sams":
                    products = await SamsScraper.GetProductsByQueryAsync(query);
                    return Ok(products);
                case "walmart":
                    products = await WalmartWebScraper.GetProductsByQueryAsync(query);
                    return Ok(products);
                default:
                    return NotFound("Products Provider not found");
            }
        }        
    }
}
