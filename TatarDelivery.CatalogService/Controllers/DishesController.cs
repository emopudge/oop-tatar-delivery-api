using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TatarDelivery.CatalogService.Domain;
using TatarDelivery.CatalogService.Contracts.Requests;
using TatarDelivery.CatalogService.Contracts.Responses;
using TatarDelivery.CatalogService.Contracts.Responses.Mappings;
using TatarDelivery.CatalogService.Services;

namespace TatarDelivery.CatalogService.Controllers;

[ApiController]
[Route("dishes")]
public class DishesController : ControllerBase
{
    private readonly ICatalogService _catalogService;
    
    public DishesController(ICatalogService catalogService)
    {
        _catalogService = catalogService;
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DishResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDishes([FromQuery] GetDishesRequest request)
    {
        IEnumerable<Dish> dishes;
        
        if (request.CategoryId.HasValue)
        {
            dishes = await _catalogService.GetDishesByCategoryAsync(request.CategoryId.Value);
        }
        else if (!string.IsNullOrEmpty(request.Search))
        {
            dishes = await _catalogService.SearchDishesAsync(request.Search);
        }
        else
        {
            dishes = await _catalogService.GetDishesAsync();
        }
        
        var response = dishes.Select(CatalogMappings.ToDishResponse.Compile());
        return Ok(response);
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(DishResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDishById(int id)
    {
        var dish = await _catalogService.FindDishByIdAsync(id);
        if (dish == null)
        {
            return NotFound(new ErrorResponse("Dish not found", "NOT_FOUND"));
        }
        
        var response = CatalogMappings.ToDishResponse.Compile()(dish);
        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(DishResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateDish([FromBody] CreateDishRequest request)
    {
        var dish = await _catalogService.CreateDishAsync(request);

        var response = CatalogMappings.ToDishResponse.Compile()(dish);
        return CreatedAtAction(nameof(GetDishById), new { id = dish.Id }, response);
    }
}
