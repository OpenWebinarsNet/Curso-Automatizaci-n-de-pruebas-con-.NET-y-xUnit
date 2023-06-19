using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Soccer.Persistence.Abstractions;
using Soccer.Persistence.MongoDb.Mappers;
using Soccer.Persistence.MongoDb.Models;

namespace Soccer.Persistence.MongoDb.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMongoDb(
            this IServiceCollection services,
            Func<IServiceProvider, MongoDbOptions> mongoDbOptionsRetriever)
    {
        // Register MongoDbOptions
        services
            .AddSingleton(
                sp =>
                {
                    var mongoDbOptions = mongoDbOptionsRetriever.Invoke(sp);
                    return mongoDbOptions;
                });

        // Register IMongoClient
        services
            .AddSingleton<IMongoClient>(
                sp =>
                {
                    var mongoDbOptions = sp.GetRequiredService<MongoDbOptions>();
                    var mongoClient = new MongoClient(mongoDbOptions.ConnectionString);
                    return mongoClient;
                });

        // Register IMongoCollection<GameEntity>
        services
            .AddTransient(sp =>
            {
                var mongoDbOptions = sp.GetRequiredService<MongoDbOptions>();
                var databaseName = mongoDbOptions.DatabaseName;
                var mongoClient = sp.GetRequiredService<IMongoClient>();
                var database = mongoClient.GetDatabase(databaseName);
                var gameCollection = database.GetCollection<GameEntity>("games");
                return gameCollection;
            });

        // Register Goal <--> GoalEntity Mapper
        services.AddSingleton<GameEntityMapper>();

        // Register the repository
        services.AddTransient<IGameRepository, GameRepositoryMongoDb>();

        return services;
    }
}
