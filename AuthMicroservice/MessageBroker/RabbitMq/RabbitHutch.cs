using AuthMicroservice.MessageBroker.RabbitMq.Interfaces;
using Ocelot.Infrastructure;
using RabbitMQ.Client;
using Stock.Infrastructure.MessageBroker.rabbitmq;

namespace AuthMicroservice.MessageBroker.RabbitMq
{
    public class RabbitHutch
    {
        private static ConnectionFactory _factory;
        private static IConnection _connection;
        private static IModel _channel;
        public static IBus CreateBus(string hostName, ILogger _logger)
        {
            _factory = new ConnectionFactory
            {
                HostName = hostName,
                DispatchConsumersAsync = true
            };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();

            return new RabbitBus(_channel, _logger);
        }
    }
}
