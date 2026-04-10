using App.DTO;
using App.Models;
using App.Repository;

namespace App.Service;

public class ProductService
{
  private readonly ProductRepository _repository;
  private readonly ILogger<ProductService> _logger;

  public ProductService(ProductRepository repository, ILogger<ProductService> logger)
  {
    _repository = repository;
    _logger = logger;
  }

  public async Task<IReadOnlyList<ProductModel>> GetAllAsync()
  {
    return await _repository.GetAllAsync();
  }

  public async Task<ProductModel?> GetByIdAsync(int id)
  {
    return await _repository.GetByIdAsync(id);
  }

  public async Task<(bool success, string? error, ProductModel? product)> CreateAsync(ProductDTO dto)
  {
    var validationError = await ValidateAsync(dto);
    if (validationError is not null)
    {
      _logger.LogWarning("Validation error while creating product: {Error}", validationError);
      return (false, validationError, null);
    }

    var existingSku = await _repository.GetBySkuAsync(dto.SKU);
    if (existingSku is not null)
    {
      const string error = "SKU já existe.";
      _logger.LogWarning("Validation error while creating product: {Error}", error);
      return (false, error, null);
    }

    var product = new ProductModel
    {
      SKU = dto.SKU.Trim(),
      Name = dto.Name.Trim(),
      Category = dto.Category.Trim(),
      Description = dto.Description.Trim(),
      Price = dto.Price,
      Stock = dto.Stock
    };

    var created = await _repository.AddAsync(product);
    _logger.LogInformation("Product created. Id: {Id}, SKU: {SKU}", created.Id, created.SKU);

    return (true, null, created);
  }

  public async Task<(bool success, string? error, ProductModel? product)> UpdateAsync(int id, ProductDTO dto)
  {
    var current = await _repository.GetByIdAsync(id);
    if (current is null)
    {
      return (false, "Produto não encontrado.", null);
    }

    var validationError = await ValidateAsync(dto);
    if (validationError is not null)
    {
      _logger.LogWarning("Validation error while updating product {Id}: {Error}", id, validationError);
      return (false, validationError, null);
    }

    var existingSku = await _repository.GetBySkuAsync(dto.SKU);
    if (existingSku is not null && existingSku.Id != id)
    {
      const string error = "SKU já existe.";
      _logger.LogWarning("Validation error while updating product {Id}: {Error}", id, error);
      return (false, error, null);
    }

    current.SKU = dto.SKU.Trim();
    current.Name = dto.Name.Trim();
    current.Category = dto.Category.Trim();
    current.Description = dto.Description.Trim();
    current.Price = dto.Price;
    current.Stock = dto.Stock;

    var updated = await _repository.UpdateAsync(current);
    _logger.LogInformation("Product updated. Id: {Id}, SKU: {SKU}", updated.Id, updated.SKU);

    return (true, null, updated);
  }

  public async Task<(bool success, string? error)> DeleteAsync(int id)
  {
    var product = await _repository.GetByIdAsync(id);
    if (product is null)
    {
      return (false, "Produto não encontrado.");
    }

    await _repository.DeleteAsync(product);
    _logger.LogInformation("Product deleted. Id: {Id}, SKU: {SKU}", product.Id, product.SKU);

    return (true, null);
  }

  private Task<string?> ValidateAsync(ProductDTO dto)
  {
    if (string.IsNullOrWhiteSpace(dto.SKU))
      return Task.FromResult<string?>("SKU é obrigatório.");

    if (string.IsNullOrWhiteSpace(dto.Name))
      return Task.FromResult<string?>("Nome é obrigatório.");

    if (string.IsNullOrWhiteSpace(dto.Category))
      return Task.FromResult<string?>("Categoria é obrigatória.");

    if (dto.Stock < 0)
      return Task.FromResult<string?>("Estoque não pode ser menor que zero.");

    if (IsElectronics(dto.Category) && dto.Price < 50)
      return Task.FromResult<string?>("Para Eletrônicos, o preço deve ser no mínimo R$ 50,00.");

    return Task.FromResult<string?>(null);
  }

  private static bool IsElectronics(string category)
  {
    var normalized = category.Trim().ToLowerInvariant();
    return normalized is "eletrônicos" or "eletronicos";
  }
}