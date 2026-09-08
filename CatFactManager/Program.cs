using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CatFactManager;
using CatFactManager.Clients;
using CatFactManager.Services;
using CatFactManager.Repositories;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
    })
    .ConfigureServices((_, services) =>
    {
        services.AddHttpClient<ICatFactApiClient, CatFactApiClient>();
        services.AddTransient<ICatFactRepository, CatFactRepository>();
        services.AddTransient<ICatFactService, CatFactService>();

        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();