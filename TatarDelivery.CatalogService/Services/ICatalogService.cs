using System.Collections.Generic;
using System.Threading.Tasks;
using TatarDelivery.CatalogService.Domain;

namespace TatarDelivery.CatalogService.Services;

public interface ICatalogService
{
    Task<IEnumerable<Category>> GetCategoriesAsync();
    Task<Dish?> FindDishByIdAsync(int id);
    Task<IEnumerable<Dish>> GetDishesAsync();
    Task<IEnumerable<Dish>> GetDishesByCategoryAsync(int categoryId);
    Task<IEnumerable<Dish>> SearchDishesAsync(string? query);
}