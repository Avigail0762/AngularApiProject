using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace OrderService.Messaging
{
    public class RabbitMqEventPublisher : IEventPublisher, IDisposable
    {
        private readonly RabbitMqOptions _options;
        private readonly ILogger<RabbitMqEventPublisher> _logger;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMqEventPublisher(IOptions<RabbitMqOptions> options, ILogger<RabbitMqEventPublisher> logger)
        {
            _options = options.Value;
            _logger = logger;

            var factory = new ConnectionFactory
            {
                HostName = _options.Host,
                Port = _options.Port,
                UserName = _options.Username,
                Password = _options.Password,
                VirtualHost = _options.VirtualHost,
                DispatchConsumersAsync = true
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(_options.Exchange, ExchangeType.Topic, durable: true, autoDelete: false);
        }

        public Task PublishGiftPurchasedAsync(Contracts.GiftPurchasedEvent payload, CancellationToken cancellationToken = default)
        {
            Publish("order.events.gift-purchased", payload);
            return Task.CompletedTask;
        }

        public Task PublishOrderPlacedAsync(Contracts.OrderPlacedEvent payload, CancellationToken cancellationToken = default)
        {
            Publish("order.events.order-placed", payload);
            return Task.CompletedTask;
        }

        public Task PublishInventoryReservedAsync(Contracts.InventoryReservedEvent payload, CancellationToken cancellationToken = default)
        {
            Publish("order.events.inventory-reserved", payload);
            return Task.CompletedTask;
        }

        public Task PublishInventoryRejectedAsync(Contracts.InventoryRejectedEvent payload, CancellationToken cancellationToken = default)
        {
            Publish("order.events.inventory-rejected", payload);
            return Task.CompletedTask;
        }

        public Task PublishOrderFinalizedAsync(Contracts.OrderFinalizedEvent payload, CancellationToken cancellationToken = default)
        {
            Publish("order.events.order-finalized", payload);
            return Task.CompletedTask;
        }

        public Task PublishPurchaseFailedAsync(Contracts.PurchaseFailedEvent payload, CancellationToken cancellationToken = default)
        {
            Publish("order.events.purchase-failed", payload);
            return Task.CompletedTask;
        }

        private void Publish(string routingKey, object payload)
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload));
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;

            _channel.BasicPublish(
                exchange: _options.Exchange,
                routingKey: routingKey,
                basicProperties: properties,
                body: body);

            _logger.LogInformation("Published event to {RoutingKey}", routingKey);
        }

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
        }
    }
}