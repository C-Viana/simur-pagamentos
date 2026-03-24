using RabbitMQ.Client;
using simur_backend.Messaging;

namespace simur_backend.Configurations
{
    public static class RabbitMqConfig
    {
        static IConnection _connection;
        static IChannel _channel;

        public static async Task<IServiceCollection> AddRabbitMqBroker(this IServiceCollection services, IConfiguration configuration)
        {
            var RabbitMQStrings = configuration.GetSection("MessageBroker:RabbitMQ");

            ConnectionFactory connFactory = new()
            {
                HostName = RabbitMQStrings["Hostname"],
                UserName = RabbitMQStrings["Username"],
                Password = RabbitMQStrings["Password"],
                Port = int.Parse(RabbitMQStrings["Port"])
            };

            _connection = await connFactory.CreateConnectionAsync("payments-service-publisher");
            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(RabbitMQStrings["Publisher:Exchange"], ExchangeType.Direct, true, false);
            await _channel.QueueDeclareAsync(RabbitMQStrings["Publisher:Queue"], true, false, false);
            await _channel.QueueBindAsync(RabbitMQStrings["Publisher:Queue"], RabbitMQStrings["Publisher:Exchange"], RabbitMQStrings["Publisher:RoutingKey"]);

            services.AddScoped<IMessageBusService, RabbitMqPublisherService>();
            return services;
        }
    }
}
