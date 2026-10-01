using Azure.Storage.Blobs;
using CoffeeNChill.Functions.Services;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage")
            ?? "UseDevelopmentStorage=true";

        services.AddSingleton(x => new BlobServiceClient(connectionString));
        services.AddSingleton<BlobStorageService>();
        services.AddSingleton(x => new TableStorageService(connectionString));
    })
    .Build();

host.Run();