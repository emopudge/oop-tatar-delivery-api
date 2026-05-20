using Microsoft.EntityFrameworkCore;
using TatarDelivery.CatalogService.Data;
using TatarDelivery.CatalogService.Services;

var builder = WebApplication.CreateBuilder(args);

// добавление сервисов в контейнер
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Catalog Service API", Version = "v1" });
});

// подключение к бд
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// интерфейсы и реализации
builder.Services.AddScoped<ICatalogDbContext, AppDbContext>();
builder.Services.AddScoped<ICatalogService, CatalogService>();

var app = builder.Build();

// конвеер http-запросов
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog Service API v1");
    });
}

// app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// инициализация бд
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    await CatalogDataSeeder.SeedAsync(context);
}

app.Run();