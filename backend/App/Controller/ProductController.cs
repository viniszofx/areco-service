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
}