namespace ProiectIndividual.Products;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Brand { get; set; }
    public string SKU { get; set; }
    public ProductCategory Category { get; set; }
    public decimal Price { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string? ImageUrl { get; set; }
    public int StockQuantity { get; set; } = 1;
    
    
    public Product(Guid Id, string Name, string Brand, string SKU, ProductCategory Category,
        decimal Price, DateTime ReleaseDate, string? ImageUrl, int StockQuantity = 1)
    {
        this.Id = Id;
        this.Name = Name;
        this.Brand = Brand;
        this.SKU = SKU;
        this.Category = Category;
        this.Price = Price;
        this.ReleaseDate = ReleaseDate;
        this.ImageUrl = ImageUrl;
        this.StockQuantity = StockQuantity;
    }
}
