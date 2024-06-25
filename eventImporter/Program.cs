using System.Net;

using eventImporter.Catalog;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;

using Polly;
using Polly.Extensions.Http;

namespace eventImporter;

public class Program
{
    static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
                       .ConfigureServices((hostContext, services) =>
                                          {
                                              var cateCatalogServiceSettings = hostContext.Configuration.GetSection(nameof(CatalogServiceSettings))
                                                                                          .Get<CatalogServiceSettings>() ?? throw new ArgumentException("failed to load CatalogServiceSettings from settings");
                                              services.AddHttpClient<CatalogClient>(client =>
                                                                                    {
                                                                                        client.BaseAddress = cateCatalogServiceSettings.BaseAddress;
                                                                                        client.DefaultRequestHeaders.Add("Accept", "application/json");
                                                                                        client.DefaultRequestHeaders.Add("User-Agent", "EventImporter");
                                                                                    })
                                                      .AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError()
                                                                                            .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
                                                                                            .WaitAndRetryAsync(cateCatalogServiceSettings.RetryAttempts,
                                                                                                               retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
                                                      .AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError()
                                                                                            .CircuitBreakerAsync(cateCatalogServiceSettings.CircuitBreakerAttempts,
                                                                                                                 TimeSpan.FromSeconds(30)));

                                              var semanticKernelSettings = hostContext.Configuration.GetSection(nameof(SemanticKernelSettings))
                                                                                      .Get<SemanticKernelSettings>() ?? throw new ArgumentException("failed to load SemanticKernelSettings from settings");
                                              var openAiKey = hostContext.Configuration["OPENAIKEY"] ?? throw new ArgumentException("OpenAI key not found");

                                              services.AddTransient(_ =>
                                                                    {
                                                                        var builder = Kernel.CreateBuilder()
                                                                                            .AddAzureOpenAIChatCompletion(deploymentName:semanticKernelSettings.Model,
                                                                                                                          endpoint:semanticKernelSettings.Endpoint,
                                                                                                                          apiKey:openAiKey,
                                                                                                                          httpClient:new HttpClient
                                                                                                                                     {
                                                                                                                                         Timeout = TimeSpan.FromMinutes(10),
                                                                                                                                     });
                                                                        return builder;
                                                                    });
                                              services.AddTransient<EventFileParser>();
                                              services.AddSingleton(provider => new PeriodicEventFileImporter(provider.GetRequiredService<EventFileParser>(),
                                                                                                              provider.GetRequiredService<CatalogClient>(),
                                                                                                              [
                                                                                                                  "https://gdexcdn.azureedge.net/data/email.txt",
                                                                                                                  "https://gdexcdn.azureedge.net/data/edifact.txt",
                                                                                                                  "https://gdexcdn.azureedge.net/data/unparsable.txt"
                                                                                                              ]
                                                                                                             ));
                                          })
                       .Build();

        var runner = host.Services.GetRequiredService<PeriodicEventFileImporter>();
        await runner.RunAsync();
    }
}