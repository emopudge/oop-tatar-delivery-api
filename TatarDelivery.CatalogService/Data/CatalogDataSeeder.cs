using System.Collections.Generic;
using System.Threading.Tasks;
using TatarDelivery.CatalogService.Domain;
using TatarDelivery.CatalogService.Data;

namespace TatarDelivery.CatalogService.Data;

public static class CatalogDataSeeder
{
    public static async Task SeedAsync(ICatalogDbContext context)
    {
        if (!context.Categories.Any())
        {
            var categories = new List<Category>
            {
                new Category { Name = "Супы", Description = "Традиционные татарские супы" },
                new Category { Name = "Основные блюда", Description = "Главные блюда татарской кухни" },
                new Category { Name = "Выпечка", Description = "Татарская выпечка и десерты" },
                new Category { Name = "Напитки", Description = "Традиционные татарские напитки" }
            };
            
            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }
        
        if (!context.Dishes.Any())
        {
            var dishes = new List<Dish>
            {
                new Dish { 
                    Name = "Эчпочмак", 
                    Description = "Традиционный татарский пирожок с мясом и картофелем", 
                    Price = 350,
                    IsAvailable = true,
                    CategoryId = 2
                },
                new Dish { 
                    Name = "Кыстыбый", 
                    Description = "Татарский блин с различными начинками", 
                    Price = 250,
                    IsAvailable = true,
                    CategoryId = 2
                },
                new Dish { 
                    Name = "Чак-чак", 
                    Description = "Традиционный татарский десерт из теста и меда", 
                    Price = 200,
                    IsAvailable = true,
                    CategoryId = 3
                },
                new Dish { 
                    Name = "Бэлиш", 
                    Description = "Многослойное блюдо из теста с мясом", 
                    Price = 450,
                    IsAvailable = true,
                    CategoryId = 2
                },
                new Dish { 
                    Name = "Губерди", 
                    Description = "Татарские пельмени с мясом", 
                    Price = 300,
                    IsAvailable = true,
                    CategoryId = 2
                },
                new Dish { 
                    Name = "Кулуба", 
                    Description = "Традиционный татарский суп с мясом и тестом", 
                    Price = 280,
                    IsAvailable = true,
                    CategoryId = 1
                },
                new Dish { 
                    Name = "Тандырный хлеб", 
                    Description = "Ароматный хлеб, приготовленный в тандыре", 
                    Price = 100,
                    IsAvailable = true,
                    CategoryId = 3
                },
                new Dish { 
                    Name = "Айран", 
                    Description = "Традиционный татарский кисломолочный напиток", 
                    Price = 120,
                    IsAvailable = true,
                    CategoryId = 4
                },
                new Dish { 
                    Name = "Элеш", 
                    Description = "Татарский пирог с мясом и картофелем", 
                    Price = 380,
                    IsAvailable = true,
                    CategoryId = 2
                },
                new Dish { 
                    Name = "Бозбаш", 
                    Description = "Наваристый мясной суп", 
                    Price = 320,
                    IsAvailable = true,
                    CategoryId = 1
                },
                new Dish { 
                    Name = "Чебуреки", 
                    Description = "Татарские пирожки с мясом", 
                    Price = 220,
                    IsAvailable = true,
                    CategoryId = 2
                },
                new Dish { 
                    Name = "Майсыз кыстыбый", 
                    Description = "Блин без начинки", 
                    Price = 150,
                    IsAvailable = true,
                    CategoryId = 3
                }
            };
            
            await context.Dishes.AddRangeAsync(dishes);
            await context.SaveChangesAsync();
        }
    }
}