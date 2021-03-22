using System;

namespace Quisy.WebScrapers
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Amazon...");
            var prods = EbayWebScraper.GetProductsByQuery("tv 50 inch");

            foreach (var prod in prods)
            {
                Console.WriteLine(prod.Title);
                if(prod.Image != null)
                    Console.WriteLine(prod.Image);
                if (prod.Price != null)
                    Console.WriteLine(prod.Price);
                Console.WriteLine();
            }
            
        }
    }
}
