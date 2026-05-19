using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TatarDelivery.OrderService.Data;

namespace TatarDelivery.OrderService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Tatar Delivery Order Service",
                Version = "v1",
                Description = "Order Service для сервиса заказа татарской еды"
            });
        });

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.EnsureCreated();
        }

        app.UseSwagger();
        app.UseSwaggerUI();

        app.MapControllers();
        app.MapGet("/", () => Results.Redirect("/swagger"));

        app.Run();
    }
}