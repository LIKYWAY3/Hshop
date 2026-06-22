public class ProductDto
{
    public int ProductId { get; set; }

    public string ProductName { get; set; }

    public string Slug { get; set; }

    public decimal Price { get; set; }

    public decimal? SalePrice { get; set; }

    public string ThumbnailUrl { get; set; }

    public string CategoryName { get; set; }
}