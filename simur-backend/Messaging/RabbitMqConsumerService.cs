using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
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
        private AsyncRetryPolicy _retryPolicy;

        private string _consumerExchange;
        private string _consumerRoutingKey;
        private string _consumerQueue;

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
            _logger.LogInformation("STOPPING Consumer service");
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
            _logger.LogInformation("EXECUTING Consumer service - Starting loop");

            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3,
                    retryAttempt => TimeSpan.FromSeconds((30 * retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogInformation("Attempt {retryCount} after error: {exception.Message}", retryCount, exception.Message);
                    });

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await InitializeRabbitMQ();
                    await ConsumePayments(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error on RabbitMQ consumer. Trying to reconnect in 10s...");
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }
            }
        }

        private async Task InitializeRabbitMQ()
        {
            var rabbitSection = _configuration.GetSection("MessageBroker:RabbitMQ");

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
            await _channel.QueueBindAsync(_dlqQueue, _dlqExchange, _dlqRoutingKey);

            var queueArgs = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", _dlqExchange },
                { "x-dead-letter-routing-key", _dlqRoutingKey }
            };
            //Set a consumer queue for payments
            _consumerExchange = rabbitSection["Consumer:Exchange"];
            _consumerRoutingKey = rabbitSection["Consumer:RoutingKey"];
            _consumerQueue = rabbitSection["Consumer:Queue"];

            await _channel.ExchangeDeclareAsync(_consumerExchange, ExchangeType.Direct, durable: true);
            await _channel.QueueDeclareAsync(_consumerQueue, durable: true, exclusive: false, autoDelete: false, arguments: queueArgs);
            await _channel.QueueBindAsync(_consumerQueue, _consumerExchange, _consumerRoutingKey);

            await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

            _logger.LogInformation("Consumer connected to queue {Queue}", _consumerQueue);
        }

        public async Task ConsumePayments(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, eventArgs) =>
            {
                using var scope = _scopeFactory.CreateScope();
                var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentServices>();

                try
                {
                    // Atualiza o status no banco
                    await _retryPolicy.ExecuteAsync(async () =>
                    {
                        byte[] body = eventArgs.Body.ToArray();
                        string message = Encoding.UTF8.GetString(body);
                        PaymentStatusHistory receivedStatus = JsonSerializer.Deserialize<PaymentStatusHistory>(message);

                        PaymentStatusHistory newStatusEntry = new(
                            receivedStatus.PaymentId,
                            receivedStatus.Type,
                            receivedStatus.Status,
                            receivedStatus.Reason,
                            receivedStatus.ChangedAt
                        );

                        _logger.LogInformation("Updating payment {PaymentId} to status {Status}", newStatusEntry.PaymentId, newStatusEntry.Status);
                        await paymentService.UpdatePaymentStatusAsync(newStatusEntry);
                    });

                    await _channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process message. Message sent to DLQ");
                    await _channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false);
                }
            };

            await _channel.BasicConsumeAsync(
                queue: _configuration["MessageBroker:RabbitMQ:Consumer:Queue"],
                autoAck: false,
                consumer: consumer);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

    }
}