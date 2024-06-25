using GloboTicket.Catalog;
using GloboTicket.Catalog.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddTransient<IEventRepository, AzureStorageEventRepository>();
builder.Services.AddTransient(provider =>
                              {
                                  var setting = builder.Configuration.GetSection(nameof(EventRecommendationsSettings)).Get<EventRecommendationsSettings>();
                                  setting.ApiKey = builder.Configuration["OPENAIKEY"] ?? throw new ArgumentException("OPENAIKEY not found");
                                  return new EventRecommendations(setting, provider.GetRequiredService<IEventRepository>());
                              });
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();
app.Run();
