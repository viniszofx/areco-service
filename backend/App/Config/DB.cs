using Microsoft.EntityFrameworkCore;

namespace App.Config
{
  public class DB : DbContext
  {
    public DB(DbContextOptions<DB> options) : base(options)
    {
    }

    public DbSet<Models.ProductModel> Products => Set<Models.ProductModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<Models.ProductModel>(entity =>
      {
        entity.HasKey(e => e.Id).HasName("PK_Products");
        entity.Property(e => e.SKU).IsRequired().HasMaxLength(50);
        entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        entity.Property(e => e.Price).HasPrecision(10, 2);
        entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
        entity.Property(e => e.Stock).IsRequired();
      });
    }
  }
}