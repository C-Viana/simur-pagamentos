let amqplib = require('amqplib')
let cardManager = require('./ManageCard.js')
let slipManager = require('./ManageBankSlip.js')
let pixManager = require('./ManagePix.js')

var connection
var channel

const broker = {
  RabbitMq: {
        Hostname: process.env.RABBITMQ_HOSTNAME,
        Port: process.env.RABBITMQ_PORT,
        Username: process.env.RABBITMQ_USERNAME,
        Password: process.env.RABBITMQ_PASSWORD,
        ConnectionName: process.env.RABBITMQ_CONNECTION_NAME,
    Consumer: {
        Exchange: process.env.RABBITMQ_CONSUMER_EXCHANGE,
        Queue: process.env.RABBITMQ_CONSUMER_QUEUE,
        RoutingKey: process.env.RABBITMQ_CONSUMER_ROUTINGKEY
    },
    Publisher: {
        Exchange: process.env.RABBITMQ_PUBLISHER_EXCHANGE,
        Queue: process.env.RABBITMQ_PUBLISHER_QUEUE,
        RoutingKey: process.env.RABBITMQ_PUBLISHER_ROUTINGKEY
    },
    DeadLetters: {
        Exchange: process.env.RABBITMQ_DEADLETTERS_EXCHANGE,
        Queue: process.env.RABBITMQ_DEADLETTERS_QUEUE,
        RoutingKey: process.env.RABBITMQ_DEADLETTERS_ROUTINGKEY,
        MgsTtl: process.env.RABBITMQ_DEADLETTERS_MGSTTL
    }
  }
}

async function connectRabbitMq() {
  console.log(`USERNAME: ${process.env.RABBITMQ_USERNAME} PASSWORD: ${process.env.RABBITMQ_PASSWORD}`)
  console.log('Connecting to broker')
  if(connection == null) {
    connection = await amqplib.connect(
      `amqp://${broker.RabbitMq.Username}:${broker.RabbitMq.Password}@${broker.RabbitMq.Hostname}:${broker.RabbitMq.Port}`, {
      reconnectTries: 5,
      reconnectDelay: 1000,
      clientProperties: {
        connection_name: broker.RabbitMq.ConnectionName
      }
    })
    channel = await connection.createChannel()
    channel.prefetch(1);
  console.log('Channel created successfully')
  }
  connection.on('error', (err) => {
    console.log('RabbitMQ connection error', err)
  })
  connection.on('connection', () => {
    console.log('Connection successfully (re)established')
  })
  
  return channel
}

function updatePaymentStatus(payment) {
  switch(payment.Type)
  {
    case 'CREDIT_CARD':
        return cardManager.setCreditCardStatus(payment)
//    case 'BOLETO':
//        return slipManager.setBankSlipStatus(payment)
    case 'PIX_DYNAMIC':
    case 'PIX_STATIC':
        return pixManager.setPixStatus(payment)
    case 'DEBIT_CARD':
        return cardManager.setDebitCardStatus(payment)
    default:
        console.log("Payment data is not recognized")
  }
}

async function publishStatusUpdate(msg, messageContent) {
  try {
    const queueArgs = {
        arguments: { 
        'x-dead-letter-exchange': broker.RabbitMq.DeadLetters.Exchange,
        'x-dead-letter-routing-key': broker.RabbitMq.DeadLetters.RoutingKey
      }
    }
    await channel.assertExchange(broker.RabbitMq.Publisher.Exchange, 'direct', { durable: true });
    await channel.assertQueue(broker.RabbitMq.Publisher.Queue, { durable: true, arguments: queueArgs.arguments});
    await channel.bindQueue(broker.RabbitMq.Publisher.Queue, broker.RabbitMq.Publisher.Exchange, broker.RabbitMq.Publisher.RoutingKey);

    channel.publish(
      broker.RabbitMq.Publisher.Exchange, 
      broker.RabbitMq.Publisher.RoutingKey, 
      Buffer.from(messageContent),
      { persistent: true }
    );
    console.log(`Message sent to exchange "${broker.RabbitMq.Publisher.Exchange}" with key "${broker.RabbitMq.Publisher.RoutingKey}": "${messageContent}"\n`);
  } catch (error) {
    console.error('Error when deserializing message:', error);
    channel.nack(msg, false, false);
  }
}

async function runConsumerChannel() {
  //DEAD LETTER QUEUE
  await channel.assertExchange(broker.RabbitMq.DeadLetters.Exchange, 'direct', { durable: true });
  await channel.assertQueue(broker.RabbitMq.DeadLetters.Queue, { durable: true});
  await channel.bindQueue(broker.RabbitMq.DeadLetters.Queue, broker.RabbitMq.DeadLetters.Exchange, broker.RabbitMq.DeadLetters.RoutingKey);
  // --------------------------------------------------------
  //CONSUMER QUEUE
  const queueArgs = {
      arguments: { 
      'x-dead-letter-exchange': broker.RabbitMq.DeadLetters.Exchange,
      'x-dead-letter-routing-key': broker.RabbitMq.DeadLetters.RoutingKey
    }
  }
  await channel.assertExchange(broker.RabbitMq.Consumer.Exchange, 'direct', { durable: true });
  await channel.assertQueue(broker.RabbitMq.Consumer.Queue, {durable: true, arguments: queueArgs.arguments });
  await channel.bindQueue(broker.RabbitMq.Consumer.Queue, broker.RabbitMq.Consumer.Exchange, broker.RabbitMq.Consumer.RoutingKey);
  // --------------------------------------------------------
  // STARTING CONSUMING MESSAGES HERE
  console.log(" [*] Waiting for messages in %s. To exit press CTRL+C", broker.RabbitMq.Consumer.Queue);
  // --------------------------------------------------------
  channel.consume(broker.RabbitMq.Consumer.Queue, (msg) => {
      if (msg !== null) {
          try {
              const messageContent = msg.content.toString();
              //console.log('RAW MESSAGE TO STRING: ' + messageContent)
              const paymentStatus = JSON.parse(messageContent);

              console.log('LOG >>> RECEIVED PAYMENT STATUS ENTRY:');
              console.log('LOG >>> Status.Id:', paymentStatus.Id);
              console.log('LOG >>> PaymentId:', paymentStatus.PaymentId);
              console.log('LOG >>> Type:', paymentStatus.Type);
              console.log('LOG >>> Status:', paymentStatus.Status);
              console.log('LOG >>> Reason:', paymentStatus.Reason);
              console.log(`LOG >>> ChangedAt: ${paymentStatus.ChangedAt}`);

              const updatedStatus = updatePaymentStatus(paymentStatus);
              publishStatusUpdate(msg, JSON.stringify(updatedStatus))
              channel.ack(msg);

          } catch (err) {
              console.error('Erro ao desserializar mensagem:', err);
              channel.nack(msg, false, true);
          }
      }
  }, { noAck: false })
}

async function RunGlobalAcquirer() {
  channel = await connectRabbitMq()
  runConsumerChannel()
  process.on('SIGINT', onShutdown)
  process.on('SIGTERM', onShutdown)
}

// Clean up when you receive a shutdown signal
async function onShutdown() {
  await connection.close()
}

module.exports = {RunGlobalAcquirer}