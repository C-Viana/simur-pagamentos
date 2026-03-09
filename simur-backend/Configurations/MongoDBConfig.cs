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
            var mongoSettings = configuration.GetSection("DatabaseConnectionString:MongoDB");
            if (string.IsNullOrEmpty(mongoSettings["URI"]) || string.IsNullOrEmpty(mongoSettings["DbName"]))
            {
                throw new InvalidOperationException("MongoDB connection string not found in app settings");
            }
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));

            services.AddSingleton<IMongoClient>(setup => new MongoClient(mongoSettings["URI"]));
            services.AddSingleton<IMongoDatabase>( 
                setup =>
                {
                    return setup.GetRequiredService<IMongoClient>().GetDatabase(mongoSettings["DbName"]);
                }
            );
            return services;
        }
    }
}
