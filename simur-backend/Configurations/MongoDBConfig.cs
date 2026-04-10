using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using simur_backend.Exceptions.CustomExceptions;

namespace simur_backend.Configurations
{
    public static class MongoDBConfig
    {
        public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var mongoConnectionString = configuration.GetSection("DatabaseConnectionString:MongoDB");
            if (string.IsNullOrEmpty(mongoConnectionString["URI"]) || string.IsNullOrEmpty(mongoConnectionString["DbName"]))
            {
                throw new InvalidEnvironmentSetupException("MongoDB connection string not found in app settings");
            }
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));

            MongoClientSettings settings = MongoClientSettings.FromConnectionString(mongoConnectionString["URI"]);
            settings.RetryReads = bool.Parse(mongoConnectionString["RetryReads"]);
            settings.RetryWrites = bool.Parse(mongoConnectionString["RetryWrites"]);
            services.AddSingleton<IMongoClient>(setup => new MongoClient(settings));
            services.AddSingleton<IMongoDatabase>( 
                setup =>
                {
                    return setup.GetRequiredService<IMongoClient>().GetDatabase(mongoConnectionString["DbName"]);
                }
            );
            return services;
        }
        public static IServiceCollection AddDatabaseConfigurationFromEnvironmet(this IServiceCollection services, IConfiguration configuration)
        {
            //var mongoConnectionString = configuration.GetSection("DatabaseConnectionString:MongoDB");
            var mongoConnectionString = new Dictionary<string, string>()
            {
                { "URI", $"mongodb://{Environment.GetEnvironmentVariable("MONGODB_SERVICE")}:{Environment.GetEnvironmentVariable("MONGODB_PORT")}/?directConnection=true&replicaSet=rs0" },
                { "DbName", Environment.GetEnvironmentVariable("MONGODB_NAME") },
                { "RetryReads", Environment.GetEnvironmentVariable("MONGODB_RETRY_WRITES") },
                { "RetryWrites", Environment.GetEnvironmentVariable("MONGODB_RETRY_READS") }
            };
            
            if (string.IsNullOrEmpty(mongoConnectionString["URI"]) || string.IsNullOrEmpty(mongoConnectionString["DbName"]))
            {
                throw new InvalidEnvironmentSetupException("MongoDB connection string not found in app settings");
            }
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));

            MongoClientSettings settings = MongoClientSettings.FromConnectionString(mongoConnectionString["URI"]);
            settings.RetryReads = bool.Parse(mongoConnectionString["RetryReads"]);
            settings.RetryWrites = bool.Parse(mongoConnectionString["RetryWrites"]);
            services.AddSingleton<IMongoClient>(setup => new MongoClient(settings));
            services.AddSingleton<IMongoDatabase>(
                setup =>
                {
                    return setup.GetRequiredService<IMongoClient>().GetDatabase(mongoConnectionString["DbName"]);
                }
            );
            return services;
        }
    }
}
