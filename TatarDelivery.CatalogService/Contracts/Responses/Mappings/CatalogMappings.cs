using System.Linq.Expressions;
using TatarDelivery.CatalogService.Domain;
using TatarDelivery.CatalogService.Contracts.Responses;

namespace TatarDelivery.CatalogService.Contracts.Responses.Mappings;

public static class CatalogMappings
{
    public static Expression<Func<Category, CategoryResponse>> ToCategoryResponse =>
        c => new CategoryResponse(c.Id, c.Name, c.Description);
    
    public static Expression<Func<Dish, DishResponse>> ToDishResponse =>
        d => new DishResponse(
            d.Id,
            d.Name,
            d.Description,
            d.Price,
            d.IsAvailable,
            d.CategoryId);
}