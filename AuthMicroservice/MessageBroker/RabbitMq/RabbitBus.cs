
using AuthMicroservice.MessageBroker.RabbitMq.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;


namespace Stock.Infrastructure.MessageBroker.rabbitmq
{
    public class RabbitBus : IBus
    {
        private readonly IModel _channel;
        private readonly ILogger _logger;

        internal RabbitBus(IModel channel, ILogger logger)
        {
            _channel = channel;
            _logger = logger;
        }
        public async Task SendAsync<T>(string queue, T message)
        {
            try
            {
                await Task.Run(() =>
                {
                    _channel.QueueDeclare(queue, true, false, false);
                    var properties = _channel.CreateBasicProperties();
                    properties.Persistent = false;
                    var output = JsonConvert.SerializeObject(message);
                    _channel.BasicPublish(string.Empty, queue, null,
                    Encoding.UTF8.GetBytes(output));
                });
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred: {Message}", ex.Message);

            }

        }
        public async Task ReceiveAsync<T>(string queue, Action<T> onMessage)
        {
            try
            {
                _channel.QueueDeclare(queue, true, false, false);
                var consumer = new AsyncEventingBasicConsumer(_channel);

                consumer.Received += async (s, e) =>
                {
                    var jsonSpecified = Encoding.UTF8.GetString(e.Body.Span);
                    var item = JsonConvert.DeserializeObject<T>(jsonSpecified);
                    onMessage(item);
                    await Task.Yield();
                };

                _channel.BasicConsume(queue, true, consumer);
                await Task.Yield();
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
            }

        }
    }
}