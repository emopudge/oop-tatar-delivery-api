namespace TatarDelivery.CatalogService.Domain;

public class Category
{
    public Category()
    {
        Dishes = new List<Dish>();
    }
    
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ICollection<Dish> Dishes { get; set; }
}