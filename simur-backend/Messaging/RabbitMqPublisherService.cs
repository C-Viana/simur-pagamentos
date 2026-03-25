using RabbitMQ.Client;
using simur_backend.Models.Constants;
using simur_backend.Models.Entities;
using System.Text;
using System.Text.Json;

namespace simur_backend.Messaging
{
    public class RabbitMqPublisherService : IMessageBusService, IAsyncDisposable
    {
        private readonly ILogger<RabbitMqPublisherService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IConfigurationSection _sectionPrefix;
        private IChannel _channel;

        public RabbitMqPublisherService(ILogger<RabbitMqPublisherService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _sectionPrefix = _configuration.GetSection("MessageBroker:RabbitMQ:Publisher");
            InitializeRabbitMQ().GetAwaiter().GetResult();
        }

        private async Task InitializeRabbitMQ()
        {
            _channel = await RabbitMqSetupService.GetChannelAsync(_configuration);
            _logger.LogInformation("Producer initialized to queue {Queue}", _sectionPrefix["Queue"]);
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
                    BasicProperties props = new ()
                    {
                        Persistent = true,
                        MessageId = statusEntry.Id.ToString(),
                        Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                    };
                    string payload = JsonSerializer.Serialize(statusEntry);
                    byte[] byteArray = Encoding.UTF8.GetBytes(payload);
                    string exchange = _sectionPrefix["Exchange"];
                    string routingKey = _sectionPrefix["RoutingKey"];

                    _logger.LogInformation($"PUBLISH EXCHANGE IS {exchange} AND ROUTING KEY IS {routingKey}");

                    await _channel.BasicPublishAsync(
                        exchange,
                        routingKey, 
                        mandatory: true, 
                        basicProperties: props, 
                        body: byteArray);
                    _logger.LogInformation("New status entry {status} with ID {StatusId} pushed to exchange {exchange} / routing {routingKey}", statusEntry.Status, statusEntry.Id, exchange, routingKey);
                }
                else
                {
                    _logger.LogInformation("Payment {PaymentId} has been completed", statusEntry.PaymentId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish payment {PaymentId}", statusEntry.Id);
                throw;
            }
        }

        public async ValueTask DisposeAsync()
        {
            //await RabbitMqSetupService.CloseConnectionAsync();
        }
    }
}