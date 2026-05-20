using Microsoft.EntityFrameworkCore;
using TatarDelivery.CatalogService.Domain;

namespace TatarDelivery.CatalogService.Data;

public interface ICatalogDbContext
{
    DbSet<Category> Categories { get; }
    DbSet<Dish> Dishes { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}