namespace App.Models
{
  public class ProductModel
  {
    public string SKU { get; set; } = string.Empty;
    public int Id { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; } = decimal.Zero;
    public string Description { get; set; } = string.Empty;
    public int Stock { get; set; } = 0;
  }
}