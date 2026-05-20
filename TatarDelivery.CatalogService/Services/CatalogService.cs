using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TatarDelivery.CatalogService.Domain;
using TatarDelivery.CatalogService.Data;
using TatarDelivery.CatalogService.Services;
using TatarDelivery.CatalogService.Contracts.Requests;

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
    public async Task<Dish> CreateDishAsync(CreateDishRequest request)
    {
        var categoryExists = await _context.Categories
            .AnyAsync(c => c.Id == request.CategoryId);

        if (!categoryExists)
        {
            throw new InvalidOperationException($"Категория с ID {request.CategoryId} не найдена.");
        }

        var dish = new Dish
        {
            Name = request.Name,
            Description = request.Description ?? "",
            Price = request.Price,
            IsAvailable = request.IsAvailable,
            CategoryId = request.CategoryId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Dishes.Add(dish);
        await _context.SaveChangesAsync();

        return dish;
    }
}