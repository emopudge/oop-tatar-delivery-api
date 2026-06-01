using Microsoft.EntityFrameworkCore;
using TatarDelivery.CatalogService.Data;
using TatarDelivery.CatalogService.Services;
using TatarDelivery.CatalogService.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Catalog Service API", Version = "v1" });
});


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($" C# видит строку подключения: {builder.Configuration.GetConnectionString("DefaultConnection")}");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<ICatalogDbContext, AppDbContext>();
builder.Services.AddScoped<ICatalogService, CatalogService>();

var app = builder.Build();
app.UseCors(policy => policy
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog Service API v1");
    });
}


app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();
    await CatalogDataSeeder.SeedAsync(context);
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    if (!db.Dishes.Any())
    {
        Console.WriteLine("📦 Добавляем тестовые блюда...");
        
        db.Dishes.AddRange(
            new Dish
            {
                Name = "Эчпочмак",
                Description = "Традиционный треугольный пирожок с мясом и картофелем",
                Price = 350m,
                CategoryId = 1,
                IsAvailable = true,
                ImageUrl = "https://images.news.ru/2025/05/11/HMLkc51fTp73sxOpOeq5D2dqNNAr9lgMc6lmf7ch_780.png"
            },
            new Dish
            {
                Name = "Кыстыбый",
                Description = "Лепёшка с картофельным пюре и обжаренным луком",
                Price = 420m,
                CategoryId = 1,
                IsAvailable = true,
                ImageUrl = "https://img.povar.ru/uploads/9c/2f/fc/4f/kistibii_s_piure-846378.jpg"
            },
            new Dish
            {
                Name = "Азу по-татарски",
                Description = "Тушёная говядина с солёными огурцами и картофелем",
                Price = 280m,
                CategoryId = 1,
                IsAvailable = true,
                ImageUrl = "https://images.news.ru/photo/528214ca-098d-11f0-a80a-ac1f6bad3ff4_780.jpg"
            },
            new Dish
            {
                Name = "Бишбармак",
                Description = "Нарезанное мясо с лапшой и бульоном",
                Price = 500m,
                CategoryId = 1,
                IsAvailable = true,
                ImageUrl = "https://img.povar.ru/640w/24/c7/7c/da/myaso_po-kazahski-55714.jpg"
            },
            new Dish
            {
                Name = "Чак-чак",
                Description = "Сладкое блюдо из теста с мёдом",
                Price = 250m,
                CategoryId = 1,
                IsAvailable = true,
                ImageUrl = "https://rskrf.ru/upload/medialibrary/b63/oza9kdxkul6lbvk3hjnowctpqetyukq4.jpg"
            }
        );

        await db.SaveChangesAsync();
        Console.WriteLine(" Добавлено 5 блюд с картинками!");
    }
}
app.Run();
