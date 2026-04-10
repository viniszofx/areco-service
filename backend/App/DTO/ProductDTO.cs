namespace App.DTO;

public class ProductDTO
{
  public string SKU { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string Category { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public decimal Price { get; set; } = decimal.Zero;
  public int Stock { get; set; } = 0;
}