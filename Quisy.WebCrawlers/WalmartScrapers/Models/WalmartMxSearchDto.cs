namespace Quisy.WebScrapers.WalmartScrapers.Models
{
    public class WalmartMxSearchDto
    {
        public Appendix Appendix { get; set; }
    }

    public class Appendix
    {
        public SearchResults SearchResults { get; set; }
    }

    public class SearchResults
    {
        public Content[] Content { get; set; }
    }

    public class Content
    {
        public string skuDisplayName { get; set; }
        public decimal skuPrice { get; set; }
        public ImageUrls imageUrls { get; set; }
        public string productSeoUrl { get; set; }
    }

    public class ImageUrls
    {
        public string small { get; set; }
        public string large { get; set; }
    }
}
