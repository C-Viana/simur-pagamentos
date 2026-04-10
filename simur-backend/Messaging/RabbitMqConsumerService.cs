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
        private AsyncRetryPolicy _retryPolicy;
        private readonly IConfiguration _configuration;
        private readonly IConfigurationSection _sectionPrefix;
        private readonly IServiceScopeFactory _scopeFactory;
        private IChannel _channel;

        public RabbitMqConsumerService(ILogger<RabbitMqPublisherService> logger, IConfiguration configuration, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _configuration = configuration;
            _sectionPrefix = _configuration.GetSection("MessageBroker:RabbitMQ:Consumer");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await RabbitMqSetupService.CloseConnectionAsync();
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
            _channel = await RabbitMqSetupService.GetChannelAsync(_configuration);
            _logger.LogInformation("Consumer connected to queue {Queue}", Environment.GetEnvironmentVariable("RABBITMQ_CONSUMER_QUEUE"));
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

                        _logger.LogInformation($"\n========== PAYMENT RECEIVED BY CONSUMER ==========\nPaymentId: {receivedStatus.PaymentId}\nType: {receivedStatus.Type}\nStatus: {receivedStatus.Status}\nReason: {receivedStatus.Reason}\n==================================================");

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
                queue: Environment.GetEnvironmentVariable("RABBITMQ_CONSUMER_QUEUE"),
                autoAck: false,
                consumer: consumer);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

    }
}