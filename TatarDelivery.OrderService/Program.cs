using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TatarDelivery.OrderService.Clients;
using TatarDelivery.OrderService.Data;
using TatarDelivery.OrderService.Services;

namespace TatarDelivery.OrderService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddHttpClient();

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddScoped<IOrderService, Services.OrderService>();

        builder.Services.AddSingleton<IPaymentClient, MockTinkoffPaymentClient>();

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Tatar Delivery Order Service",
                Version = "v1",
                Description = "Order Service для сервиса заказа татарской еды"
            });
        });
        builder.Services.AddHttpClient("TinkoffMock");

        var app = builder.Build();
        
        app.UseCors(policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.EnsureCreated();
            EnsureOrderSchema(dbContext);
        }

        app.UseSwagger();
        app.UseSwaggerUI();

        app.MapControllers();
        app.MapGet("/", () => Results.Redirect("/swagger"));

        app.Run();
    }

    private static void EnsureOrderSchema(AppDbContext dbContext)
    {
        var connection = dbContext.Database.GetDbConnection();
        connection.Open();

        try
        {
            using var columnsCommand = connection.CreateCommand();
            columnsCommand.CommandText = "PRAGMA table_info('Orders');";

            var hasRestaurantId = false;
            using (var reader = columnsCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (string.Equals(reader.GetString(1), "RestaurantId", StringComparison.OrdinalIgnoreCase))
                    {
                        hasRestaurantId = true;
                        break;
                    }
                }
            }

            if (hasRestaurantId)
            {
                return;
            }

            using var alterCommand = connection.CreateCommand();
            alterCommand.CommandText = "ALTER TABLE Orders ADD COLUMN RestaurantId INTEGER NOT NULL DEFAULT 1;";
            alterCommand.ExecuteNonQuery();
        }
        finally
        {
            connection.Close();
        }
    }
}
