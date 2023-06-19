using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Soccer.Persistence.MongoDb.Models;
using Soccer.WebApi;

namespace Soccer.FunctionalTests;
public abstract class FunctionalTest
    : GivenWhenThenTest
{
    private readonly string _uniqueTestId;
    protected HttpClient HttpClient { get; }
    protected IConfiguration Configuration { get; }
    protected FunctionalTest()
    {
        var server =
            new TestServer(
                new WebHostBuilder()
                    .UseStartup<Startup>()
                    .UseCommonConfiguration()
                    .UseEnvironment("Test")
                    .ConfigureTestServices(ConfigureTestServices));

        HttpClient = server.CreateClient();
        Configuration = server.Services.GetRequiredService<IConfiguration>();

        // Cada ejecución de cada test va a tener asociado un ID único que podemos usar para nombrar la base de datos durante esa ejecución.
        _uniqueTestId = Guid.NewGuid().ToString().Replace("-", string.Empty);
    }

    protected virtual void ConfigureTestServices(IServiceCollection services)
    {
        services.Replace<MongoDbOptions>(
            sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var hostname = configuration.GetValue<string>("DocumentStore:Hostname");
                var username = configuration.GetValue<string>("DocumentStore:Username");
                var password = configuration.GetValue<string>("DocumentStore:Password");
                var databaseName = configuration.GetValue<string>("DocumentStore:DatabaseName");
                var databaseUniqueName = $"{databaseName}-{_uniqueTestId}"; 
                var mongoDbOptions = new MongoDbOptions(hostname, username, password, databaseUniqueName);
                return mongoDbOptions;
            },
            ServiceLifetime.Singleton);
    }
}

public static class TestExtensions
{
    public static IWebHostBuilder UseCommonConfiguration(this IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((hostingContext, config) =>
        {
            var env = hostingContext.HostingEnvironment;

            config
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                config.AddUserSecrets(appAssembly, true);
            }
        });

        return builder;
    }

    public static IServiceCollection Replace<TService>(
        this IServiceCollection services,
        Func<IServiceProvider, TService> implementationFactory,
        ServiceLifetime lifetime)
        where TService : class
    {
        var descriptorToRemove = services.FirstOrDefault(d => d.ServiceType == typeof(TService));

        if (descriptorToRemove is null)
        {
            return services;
        }

        services.Remove(descriptorToRemove);
        var descriptorToAdd = new ServiceDescriptor(typeof(TService), implementationFactory, lifetime);
        services.Add(descriptorToAdd);

        return services;
    }
}