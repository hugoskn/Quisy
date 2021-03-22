using Microsoft.AspNetCore.Mvc;
using Quisy.WebScrapers.Amazon;

namespace Quisy.Api.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        [HttpGet("{provider}")]
        public IActionResult Get(string provider, string query)
        {
            if (string.IsNullOrWhiteSpace(provider))
                return BadRequest();
            switch (provider.ToLower())
            {
                case "amazon":
                    var products = AmazonWebScraper.GetProductsByQuery(query);
                    return Ok(products);
                default:
                    return NotFound("Provider not found");
            }            
        }        
    }
}
