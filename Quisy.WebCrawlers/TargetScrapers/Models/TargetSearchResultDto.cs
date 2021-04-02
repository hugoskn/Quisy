namespace Quisy.WebScrapers.TargetScrapers.Models
{
    public class TargetSearchResultDto
    {
        public TargetDataDto Data { get; set; }
    }

    public class TargetDataDto
    {
        public TargetSearchDto Search { get; set; }
    }

    public class TargetSearchDto
    {
        public TargetProductDto[] Products { get; set; }
    }

    public class TargetProductDto
    {
        public TargetProductItem Item { get; set; }
        public TargetProductPrice Price { get; set; }
    }

    public class TargetProductPrice
    {
        public decimal current_retail { get; set; }
    }

    public class TargetProductItem
    {
        public TargetProductItemEnrichment Enrichment { get; set; }
        public TargetProductItemDescription product_description { get; set; }
    }

    public class TargetProductItemDescription
    {
        public string Title { get; set; }
    }

    public class TargetProductItemEnrichment
    {
        public string buy_url { get; set; }
        public TargetImages Images { get; set; }
    }

    public class TargetImages
    {
        public string primary_image_url { get; set; }
    }
}
