namespace Quisy.WebScrapers.SamsScrapers.Models
{
    public class SamsSearchDto
    {
        public Response Response { get; set; }
    }

    public class Response
    {
        public Products[] Products { get; set; }
    }

    public class Products
    {
        public decimal sale_price { get; set; }
        public string url { get; set; }
        public string thumb_image { get; set; }
        public string title { get; set; }
    }
}
