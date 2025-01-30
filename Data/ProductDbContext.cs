using Microsoft.EntityFrameworkCore;
using Mobile_App_01.backend.Model;

public class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
}