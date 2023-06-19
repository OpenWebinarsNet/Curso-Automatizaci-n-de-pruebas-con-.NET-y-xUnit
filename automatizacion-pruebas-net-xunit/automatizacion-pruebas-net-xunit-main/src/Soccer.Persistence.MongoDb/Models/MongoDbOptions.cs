namespace Soccer.Persistence.MongoDb.Models;

public sealed class MongoDbOptions
{
    public string ConnectionString { get; }
    public string DatabaseName { get; }

    /// <summary>
    /// Instantiates options that serve as configuration for MongoDb
    /// </summary>
    /// <param name="hostname">The hostname</param>
    /// <param name="username">The admin username</param>
    /// <param name="password">The password</param>
    /// <param name="databaseName">The database name</param>
    public MongoDbOptions(string hostname, string username, string password, string databaseName)
    {
        DatabaseName = databaseName;
        ConnectionString = $"mongodb://{username}:{password}@{hostname}:27017";
    }
}
