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

        private static string _publisherQueue;
        private static string _publisherExchange;
        private static string _publisherRoutingKey;
        private static string _consumerExchange;
        private static string _consumerRoutingKey;
        private static string _consumerQueue;
        private static string _dlqExchange;
        private static string _dlqRoutingKey;
        private static string _dlqQueue;

        private static List<string> _returnedMsgs = new List<string>();

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

            //Set a Dead Letter Queue
            _dlqExchange = rabbitSection["DeadLetters:Exchange"];
            _dlqRoutingKey = rabbitSection["DeadLetters:RoutingKey"];
            _dlqQueue = rabbitSection["DeadLetters:Queue"];
            //Set a consumer queue
            _consumerExchange = rabbitSection["Consumer:Exchange"];
            _consumerRoutingKey = rabbitSection["Consumer:RoutingKey"];
            _consumerQueue = rabbitSection["Consumer:Queue"];
            //Set a publisher queue
            _publisherQueue = rabbitSection["Publisher:Queue"];
            _publisherExchange = rabbitSection["Publisher:Exchange"];
            _publisherRoutingKey = rabbitSection["Publisher:RoutingKey"];

            //Set up Dead Letter queue configurations
            var queueArgs = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", _dlqExchange },
                { "x-dead-letter-routing-key", _dlqRoutingKey }
            };

            //Set up connection attributes
            var factory = new ConnectionFactory
            {
                HostName = rabbitSection["Hostname"],
                UserName = rabbitSection["Username"],
                Password = rabbitSection["Password"],
                Port = int.Parse(rabbitSection["Port"]),
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
            _connection = await factory.CreateConnectionAsync(rabbitSection["ConnectionName"]);
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
    }
}
