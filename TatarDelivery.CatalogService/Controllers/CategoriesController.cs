using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TatarDelivery.CatalogService.Contracts.Responses;
using TatarDelivery.CatalogService.Contracts.Responses.Mappings;
using TatarDelivery.CatalogService.Services;

namespace TatarDelivery.CatalogService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICatalogService _catalogService;
    
    public CategoriesController(ICatalogService catalogService)
    {
        _catalogService = catalogService;
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _catalogService.GetCategoriesAsync();
        var response = categories.Select(CatalogMappings.ToCategoryResponse.Compile());
        
        return Ok(response);
    }
}