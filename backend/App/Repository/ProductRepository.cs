using App.Config;
using App.Models;
using Microsoft.EntityFrameworkCore;

namespace App.Repository;

public class ProductRepository
{
  private readonly DB _db;

  public ProductRepository(DB db)
  {
    _db = db;
  }

  public async Task<IReadOnlyList<ProductModel>> GetAllAsync()
  {
    return await _db.Products.AsNoTracking().OrderBy(p => p.Id).ToListAsync();
  }

  public Task<ProductModel?> GetByIdAsync(int id)
  {
    return _db.Products.FirstOrDefaultAsync(p => p.Id == id);
  }

  public Task<ProductModel?> GetBySkuAsync(string sku)
  {
    return _db.Products.FirstOrDefaultAsync(p => p.SKU == sku);
  }

  public async Task<ProductModel> AddAsync(ProductModel product)
  {
    await _db.Products.AddAsync(product);
    await _db.SaveChangesAsync();
    return product;
  }

  public async Task<ProductModel> UpdateAsync(ProductModel product)
  {
    _db.Products.Update(product);
    await _db.SaveChangesAsync();
    return product;
  }

  public async Task DeleteAsync(ProductModel product)
  {
    _db.Products.Remove(product);
    await _db.SaveChangesAsync();
  }
}