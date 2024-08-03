namespace BlackFriday.Infrastructure;

public sealed class Product
{
    public string Asin { get; set; }
    public long BoughtInLastMonth { get; set; }
    public string CategoryName { get; set; }
    public string ImgUrl { get; set; }
    public bool IsBestSeller { get; set; }
    public decimal Price { get; set; }
    public string ProductUrl { get; set; }
    public long Reviews { get; set; }
    public decimal Stars { get; set; }
    public string Title { get; set; }
}
