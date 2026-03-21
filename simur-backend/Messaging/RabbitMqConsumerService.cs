using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using simur_backend.Models.Constants;
using simur_backend.Models.Entities;
using simur_backend.Services.Payments;
using System.Text;
using System.Text.Json;

namespace simur_backend.Messaging
{
    public class RabbitMqConsumerService : BackgroundService
    {
        private readonly ILogger<RabbitMqPublisherService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;

        private IConnection _connection;
        private IChannel _channel;

        private string _consumerExchange;
        private string _consumerRoutingKey;
        private string _consumerQueue;

        private string _publisherExchange;
        private string _publisherRoutingKey;
        private string _publisherQueue;

        private string _dlqExchange;
        private string _dlqRoutingKey;
        private string _dlqQueue;

        public RabbitMqConsumerService(ILogger<RabbitMqPublisherService> logger, IConfiguration configuration, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _scopeFactory = scopeFactory;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("RabbitMqConsumerService STOPPING Consumer service");
            if (_channel != null)
            {
                await _channel.CloseAsync();
                _channel.Dispose();
            }
            if (_connection != null)
            {
                await _connection.CloseAsync();
                _connection.Dispose();
            }
            await base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("RabbitMqConsumerService EXECUTING Consumer service - Starting loop");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await InitializeRabbitMQ();
                    await ConsumePayments(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "RabbitMqConsumerService Error on RabbitMQ consumer. Trying to reconnect in 10s...");
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }
            }
        }

        private async Task InitializeRabbitMQ()
        {
            var rabbitSection = _configuration.GetSection("MessageBroker:RabbitMQ");
            _publisherExchange = rabbitSection["Publisher:Exchange"];
            _publisherRoutingKey = rabbitSection["Publisher:RoutingKey"];
            _publisherQueue = rabbitSection["Publisher:Queue"];

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
            _dlqExchange = rabbitSection["DeadLetters:Exchange"];
            _dlqRoutingKey = rabbitSection["DeadLetters:RoutingKey"];
            _dlqQueue = rabbitSection["DeadLetters:Queue"];
            await _channel.QueueDeclareAsync(_dlqQueue, durable: true, exclusive: false, autoDelete: false);
            var queueArgs = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", _dlqExchange },          // ou um exchange específico
                { "x-dead-letter-routing-key", _dlqRoutingKey },    // rota para a DLQ
                { "x-message-ttl", 300000 }                        // opcional: expira mensagem após 5 min
            };

            //Set a consumer queue for payments
            _consumerExchange = rabbitSection["Consumer:Exchange"];
            _consumerRoutingKey = rabbitSection["Consumer:RoutingKey"];
            _consumerQueue = rabbitSection["Consumer:Queue"];

            await _channel.ExchangeDeclareAsync(_consumerExchange, ExchangeType.Direct, durable: true);
            await _channel.QueueDeclareAsync(_consumerQueue, durable: true, exclusive: false, autoDelete: false, arguments: queueArgs);
            await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

            _logger.LogInformation("RabbitMqConsumerService: Consumer connected to queue {Queue}", _consumerQueue);
        }

        public async Task ConsumePayments(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            
            consumer.ReceivedAsync += async (model, eventArgs) =>
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentServices>();

                    byte[] body = eventArgs.Body.ToArray();
                    string message = Encoding.UTF8.GetString(body);
                    PaymentStatusHistory receivedStatus = JsonSerializer.Deserialize<PaymentStatusHistory>(message);

                    PaymentStatusHistory newStatusEntry = new (
                        receivedStatus.PaymentId,
                        receivedStatus.Type,
                        receivedStatus.Status,
                        receivedStatus.Reason,
                        receivedStatus.ChangedAt
                    );

                    _logger.LogInformation("RabbitMqConsumerService Updating payment {PaymentId} to status {Status}", newStatusEntry.PaymentId, newStatusEntry.Status);

                    // Atualiza o status no banco
                    await paymentService.UpdatePaymentStatusAsync(newStatusEntry);
                    await _channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "RabbitMqConsumerService Failed to process message. Rejecting...");
                    await _channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false);
                }
            };

            await _channel.BasicConsumeAsync(
                queue: _configuration["MessageBroker:RabbitMQ:Consumer:Queue"],
                autoAck: false,
                consumer: consumer);

            // Mantém o consumer vivo até o cancelamento
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(5000, stoppingToken);
            }
        }

    }
}
