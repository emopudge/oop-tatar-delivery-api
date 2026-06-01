using TatarDelivery.DeliveryService.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "DeliveryService", Version = "v1" }));
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IGeocodingService, YandexGeocodingService>();
builder.Services.AddSingleton<DeliveryValidationService>();

var app = builder.Build();
app.UseCors(policy => policy
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());
app.UseSwagger(); 
app.UseSwaggerUI(); 
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();