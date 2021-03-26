using Quisy.WebScrapers.Helpers;
using System;
using System.Diagnostics;
using System.Linq;

namespace Quisy.WebScrapers
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Getting products from scrappers...");
            var watch = new Stopwatch();
            watch.Start();
            var prods = ScrapersFacade.GetAllProductsFromScrapersAsync("tv 50 inch").Result;
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
    }
}
