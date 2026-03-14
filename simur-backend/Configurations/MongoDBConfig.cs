using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace simur_backend.Configurations
{
    public static class MongoDBConfig
    {
        public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var mongoConnectionString = configuration.GetSection("DatabaseConnectionString:MongoDB");
            if (string.IsNullOrEmpty(mongoConnectionString["URI"]) || string.IsNullOrEmpty(mongoConnectionString["DbName"]))
            {
                throw new InvalidOperationException("MongoDB connection string not found in app settings");
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
