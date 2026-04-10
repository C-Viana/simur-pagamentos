using DnsClient.Internal;
using RabbitMQ.Client;
using System.Text;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

namespace simur_backend.Messaging
{
    public class RabbitMqSetupService
    {
        private static ILogger<RabbitMqSetupService> _logger;

        private static IConnection _connection;
        private static IChannel _channel;
        //Set connection params
        private static readonly string HOSTNAME = Environment.GetEnvironmentVariable("RABBITMQ_HOSTNAME");
        private static readonly string USERNAME = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME");
        private static readonly string PASSWORD = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD");
        private static readonly int PORT = int.Parse(Environment.GetEnvironmentVariable("RABBITMQ_PORT"));
        //Set a publisher queue
        private static readonly string _publisherQueue = Environment.GetEnvironmentVariable("RABBITMQ_PUBLISHER_QUEUE");
        private static readonly string _publisherExchange = Environment.GetEnvironmentVariable("RABBITMQ_CONSUMER_EXCHANGE");
        private static readonly string _publisherRoutingKey = Environment.GetEnvironmentVariable("RABBITMQ_PUBLISHER_ROUTINGKEY");
        //Set a consumer queue
        private static readonly string _consumerExchange = Environment.GetEnvironmentVariable("RABBITMQ_CONSUMER_EXCHANGE");
        private static readonly string _consumerRoutingKey = Environment.GetEnvironmentVariable("RABBITMQ_CONSUMER_ROUTINGKEY");
        private static readonly string _consumerQueue = Environment.GetEnvironmentVariable("RABBITMQ_CONSUMER_QUEUE");
        //Set a Dead Letter Queue
        private static readonly string _dlqExchange = Environment.GetEnvironmentVariable("RABBITMQ_DEADLETTERS_EXCHANGE");
        private static readonly string _dlqRoutingKey = Environment.GetEnvironmentVariable("RABBITMQ_DEADLETTERS_ROUTINGKEY");
        private static readonly string _dlqQueue = Environment.GetEnvironmentVariable("RABBITMQ_DEADLETTERS_QUEUE");

        private static List<string> _returnedMsgs = [];

        private static void StartLogger()
        {
            using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            _logger = loggerFactory.CreateLogger<RabbitMqSetupService>();
        }

        public static async Task<IChannel> GetChannelAsync(IConfiguration configuration)
        {
            if (_channel != null) 
                return _channel;

            StartLogger();
            var rabbitSection = configuration.GetSection("MessageBroker:RabbitMQ");

            //Set up Dead Letter queue configurations
            var queueArgs = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", _dlqExchange },
                { "x-dead-letter-routing-key", _dlqRoutingKey }
            };

        //Set up connection attributes
        var factory = new ConnectionFactory
            {
                HostName = HOSTNAME,
                UserName = USERNAME,
                Password = PASSWORD,
                Port = PORT,
                AutomaticRecoveryEnabled = true
            };

            //Set up Channel Options for publisher confirmations
            CreateChannelOptions channelOpts = new(
                    publisherConfirmationsEnabled: true,
                    publisherConfirmationTrackingEnabled: true,
                    outstandingPublisherConfirmationsRateLimiter: new ThrottlingRateLimiter(256)
                );

            // ---------------------------------------------------------------------------
            //********************* Create connection and channel ************************
            _connection = await factory.CreateConnectionAsync(Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION_NAME"));
            _channel = await _connection.CreateChannelAsync(channelOpts);
            //****************************************************************************
            // ---------------------------------------------------------------------------

            //Set Basic Return for publishing errors
            _channel.BasicReturnAsync += (sender, args) =>
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                _logger.LogError(
                    "[WARNING] Message returned from broker. Reason: {reason}, Exchange: {exchange}, RoutingKey: {routingKey}, Message: {message}",
                    args.ReplyText,
                    args.Exchange,
                    args.RoutingKey,
                    message
                );
                _returnedMsgs.Add(message);
                return Task.CompletedTask;
            };

            //Create and bind DLQ queue
            await _channel.QueueDeclareAsync(_dlqQueue, durable: true, exclusive: false, autoDelete: false);
            await _channel.QueueBindAsync(_dlqQueue, _dlqExchange, _dlqRoutingKey);

            //Create and bind CONSUMER queue
            await _channel.ExchangeDeclareAsync(_consumerExchange, ExchangeType.Direct, durable: true);
            await _channel.QueueDeclareAsync(_consumerQueue, durable: true, exclusive: false, autoDelete: false, arguments: queueArgs);
            await _channel.QueueBindAsync(_consumerQueue, _consumerExchange, _consumerRoutingKey);

            //Create and bind PRODUCER queue
            await _channel.ExchangeDeclareAsync(_publisherQueue, ExchangeType.Direct, durable: true);
            await _channel.QueueDeclareAsync(_publisherQueue, durable: true, exclusive: false, autoDelete: false, arguments: queueArgs);
            await _channel.QueueBindAsync(_publisherQueue, _publisherExchange, _publisherRoutingKey);

            //Set up QOS for the channel
            await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

            return _channel;
        }

        public static async Task CloseConnectionAsync()
        {
            _logger.LogInformation("STOPPING Consumer service");
            if (_channel.CloseReason == null)
            {
                await _channel.CloseAsync();
                await _channel.DisposeAsync();
            }
            if (_connection.CloseReason == null)
            {
                await _connection.CloseAsync();
                await _connection.DisposeAsync();
            }
        }

        public static IConnection GetConnection()
        {
            return _connection;
        }
    }
}
