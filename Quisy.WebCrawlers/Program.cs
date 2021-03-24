using Quisy.WebScrapers.AmazonScrapers;
using Quisy.WebScrapers.EbayScrapers;
using Quisy.WebScrapers.Models;
using Quisy.WebScrapers.WalmartScrapers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Quisy.WebScrapers
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Getting products from scrappers...");
            var watch = new Stopwatch();
            watch.Start();
            var prods = GetAllProductsFromScrapersAsync("tv 50 inch").Result;
            watch.Stop();
            Console.WriteLine($"Retrieved {prods.Count()} from scrappers in {watch.ElapsedMilliseconds / 1000} seconds \n");

            foreach (var prod in prods)
            {
                Console.WriteLine(prod.Title);
                if(prod.Image != null)
                    Console.WriteLine(prod.Image);
                if (prod.Link != null)
                    Console.WriteLine(prod.Link);
                if (prod.Price != null)
                    Console.WriteLine(prod.Price);
                if (prod.Source != null)
                    Console.WriteLine(prod.Source);
                Console.WriteLine();
            }
            
        }

        private static async Task<IEnumerable<ProductDTO>> GetAllProductsFromScrapersAsync(string query)
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
