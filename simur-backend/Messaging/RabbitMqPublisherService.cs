using RabbitMQ.Client;
using simur_backend.Models.Constants;
using simur_backend.Models.Entities;
using System.Text;
using System.Text.Json;

namespace simur_backend.Messaging
{
    public class RabbitMqPublisherService : IMessageBusService, IDisposable
    {
        private readonly ILogger<RabbitMqPublisherService> _logger;
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly string _exchange;
        private readonly string _routingKey;

        public RabbitMqPublisherService(ILogger<RabbitMqPublisherService> logger, IConfiguration configuration)
        {
            _logger = logger;
            var rabbitSection = configuration.GetSection("MessageBroker:RabbitMQ");

            var factory = new ConnectionFactory
            {
                HostName = rabbitSection["Hostname"],
                UserName = rabbitSection["Username"],
                Password = rabbitSection["Password"],
                Port = int.Parse(rabbitSection["Port"]),
                AutomaticRecoveryEnabled = true
            };

            _connection = factory.CreateConnectionAsync(rabbitSection["ConnectionName"]).GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

            _exchange = rabbitSection["Publisher:Exchange"];
            _routingKey = rabbitSection["Publisher:RoutingKey"];

            _channel.ExchangeDeclareAsync(_exchange, ExchangeType.Direct, durable: true);
            _channel.QueueDeclareAsync(rabbitSection["Publisher:Queue"], durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBindAsync(rabbitSection["Publisher:Queue"], _exchange, _routingKey);
        }

        public void PublishPaymentStatus(PaymentStatusHistory statusEntry)
        {
            try
            {
                // Confirma o processamento
                bool isFinalStatus = statusEntry.Status
                    is PaymentStatus.SETTLED
                    or PaymentStatus.REJECTED
                    or PaymentStatus.CANCELLED
                    or PaymentStatus.FAILED
                    or PaymentStatus.BLOCKED
                    or PaymentStatus.EXPIRED
                    or PaymentStatus.PARTIALLY_REFUNDED
                    or PaymentStatus.REFUNDED;

                if (!isFinalStatus)
                {
                    var payload = JsonSerializer.Serialize(statusEntry);
                    var byteArray = Encoding.UTF8.GetBytes(payload);
                    _channel.BasicPublishAsync(_exchange, _routingKey, byteArray);
                    _logger.LogInformation("RabbitMqPublisherService New status entry {status} with ID {StatusId} pushed to exchange {exchange} / routing {routingKey}", statusEntry.Status, statusEntry.Id, _exchange, _routingKey);
                }
                else
                {
                    _logger.LogInformation("RabbitMqConsumerService Payment {PaymentId} has been completed", statusEntry.PaymentId);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish payment {PaymentId}", statusEntry.Id);
            }
        }

        public void Dispose()
        {
            _channel?.DisposeAsync();
            _connection?.DisposeAsync();
        }

    }
}
