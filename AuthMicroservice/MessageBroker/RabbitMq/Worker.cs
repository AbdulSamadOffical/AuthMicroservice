using AuthMicroservice.Dtos;
using AuthMicroservice.MessageBroker.RabbitMq.Interfaces;

namespace AuthMicroservice.MessageBroker.RabbitMq
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IBus _busControl;
        public Worker(ILogger<Worker> logger, IBus bus)
        {
            _logger = logger;
            _busControl = bus;
        }
      
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
          await _busControl.ReceiveAsync<User>("stock", x =>
          {
              Task.Run(() => { DidJob(x); }, stoppingToken);
          });
      }

      private void DidJob(User user)
      {

         _logger.LogInformation($"Username: {user.UserName}, Stock Symbol: {user.Email}");

      }
      
        }
    }
