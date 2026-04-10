using App.DTO;
using App.Service;
using Microsoft.AspNetCore.Mvc;

namespace App.Controller;

[ApiController]
[Route("product")]
public class ProductController : ControllerBase
{
  private readonly ProductService _service;
  private readonly ILogger<ProductController> _logger;

  public ProductController(ProductService service, ILogger<ProductController> logger)
  {
    _service = service;
    _logger = logger;
  }

  [HttpGet]
  public async Task<IActionResult> Get()
  {
    var products = await _service.GetAllAsync();
    return Ok(products);
  }

  [HttpGet("{id:int}")]
  public async Task<IActionResult> GetById(int id)
  {
    var product = await _service.GetByIdAsync(id);
    if (product is null)
      return NotFound(new { message = "Produto não encontrado." });

    return Ok(product);
  }

  [HttpPost]
  public async Task<IActionResult> Post([FromBody] ProductDTO dto)
  {
    try
    {
      var result = await _service.CreateAsync(dto);
      if (!result.success)
        return BadRequest(new { message = result.error });

      return CreatedAtAction(nameof(GetById), new { id = result.product!.Id }, result.product);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Unexpected error while creating product.");
      return StatusCode(500, new { message = "Erro interno do servidor." });
    }
  }

  [HttpPut("{id:int}")]
  public async Task<IActionResult> Put(int id, [FromBody] ProductDTO dto)
  {
    try
    {
      var result = await _service.UpdateAsync(id, dto);
      if (!result.success)
      {
        if (result.error == "Produto não encontrado.")
          return NotFound(new { message = result.error });

        return BadRequest(new { message = result.error });
      }

      return Ok(result.product);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Unexpected error while updating product {Id}.", id);
      return StatusCode(500, new { message = "Erro interno do servidor." });
    }
  }

  [HttpDelete("{id:int}")]
  public async Task<IActionResult> Delete(int id)
  {
    try
    {
      var result = await _service.DeleteAsync(id);
      if (!result.success)
        return NotFound(new { message = result.error });

      return NoContent();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Unexpected error while deleting product {Id}.", id);
      return StatusCode(500, new { message = "Erro interno do servidor." });
    }
  }
}