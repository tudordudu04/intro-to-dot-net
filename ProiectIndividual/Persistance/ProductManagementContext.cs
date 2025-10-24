using Microsoft.EntityFrameworkCore;
using ProiectIndividual.Products;
namespace ProiectIndividual.Persistance;

public class ProductManagementContext(DbContextOptions<ProductManagementContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
}