using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TatarDelivery.CatalogService.Domain;
using TatarDelivery.CatalogService.Data;
using TatarDelivery.CatalogService.Services;

namespace TatarDelivery.CatalogService.Services;

public class CatalogService : ICatalogService
{
    private readonly ICatalogDbContext _context;
    
    public CatalogService(ICatalogDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Category>> GetCategoriesAsync()
    {
        return await _context.Categories
            .AsNoTracking()
            .ToListAsync();
    }
    
    public async Task<Dish?> FindDishByIdAsync(int id)
    {
        return await _context.Dishes
            .AsNoTracking()
            .Include(d => d.Category)
            .FirstOrDefaultAsync(d => d.Id == id);
    }
    
    public async Task<IEnumerable<Dish>> GetDishesAsync()
    {
        return await _context.Dishes
            .AsNoTracking()
            .Include(d => d.Category)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Dish>> GetDishesByCategoryAsync(int categoryId)
    {
        return await _context.Dishes
            .AsNoTracking()
            .Include(d => d.Category)
            .Where(d => d.CategoryId == categoryId)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Dish>> SearchDishesAsync(string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Enumerable.Empty<Dish>();
        }
        
        return await _context.Dishes
            .AsNoTracking()
            .Include(d => d.Category)
            .Where(d => 
                EF.Functions.ILike(d.Name, $"%{query}%") || 
                EF.Functions.ILike(d.Description, $"%{query}%"))
            .ToListAsync();
    }
}