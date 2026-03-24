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
        private readonly string _queue;
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

            //Set a Dead Letter Queue
            _channel.QueueDeclareAsync(rabbitSection["DeadLetters:Queue"], durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBindAsync(rabbitSection["DeadLetters:Queue"], rabbitSection["DeadLetters:Exchange"], rabbitSection["DeadLetters:RoutingKey"]);

            var queueArgs = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", rabbitSection["DeadLetters:Exchange"] },
                { "x-dead-letter-routing-key", rabbitSection["DeadLetters:RoutingKey"] }
            };

            _queue = rabbitSection["Publisher:Queue"];
            _exchange = rabbitSection["Publisher:Exchange"];
            _routingKey = rabbitSection["Publisher:RoutingKey"];

            _channel.ExchangeDeclareAsync(_exchange, ExchangeType.Direct, durable: true);
            _channel.QueueDeclareAsync(_queue, durable: true, exclusive: false, autoDelete: false, arguments: queueArgs);
            _channel.QueueBindAsync(_queue, _exchange, _routingKey);
            _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);
        }


        public async Task PublishPaymentStatus(PaymentStatusHistory statusEntry)
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
                    string payload = JsonSerializer.Serialize(statusEntry);
                    byte[] byteArray = Encoding.UTF8.GetBytes(payload);
                    await _channel.BasicPublishAsync(_exchange, _routingKey, byteArray);
                    _logger.LogInformation("New status entry {status} with ID {StatusId} pushed to exchange {exchange} / routing {routingKey}", statusEntry.Status, statusEntry.Id, _exchange, _routingKey);
                }
                else
                {
                    _logger.LogInformation("Payment {PaymentId} has been completed", statusEntry.PaymentId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish payment {PaymentId}", statusEntry.Id);
            }
        }

        public void Dispose()
        {
            _channel.DisposeAsync();
            _connection.DisposeAsync();
        }

    }
}