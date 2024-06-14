using GloboTicket.Catalog.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddTransient<IEventRepository, AzureStorageEventRepository>();
//builder.Services.AddTransient<IEventRepository, InMemoryEventRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();
app.Run();
