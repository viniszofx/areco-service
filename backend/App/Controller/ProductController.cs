using Microsoft.AspNetCore.Mvc;

namespace App.Controller;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
  public string Get()
  {
    return "Hello World!";
  }

  [HttpGet("{id}")]
  public string Get(string id)
  {
    return $"Hello World! {id}";
  }

  [HttpPost]
  public string Post()
  {
    return "Hello World!";
  }

  [HttpPut("{id}")]
  public string Put(string id)
  {
    return $"Hello World! {id}";
  }

  [HttpDelete("{id}")]
  public string Delete(string id)
  {
    return $"Hello World! {id}";
  }

}