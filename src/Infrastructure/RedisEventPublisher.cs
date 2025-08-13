using Application.Abstractions;
using StackExchange.Redis;
using System.Text.Json;

namespace Infrastructure
{
    public class RedisEventPublisher : IEventPublisher, IAsyncDisposable
    {
        private readonly IConnectionMultiplexer _mux;
        private readonly ISubscriber _sub;
        private const string Channel = "events.nutrition";

        public RedisEventPublisher(IConnectionMultiplexer mux) { _mux = mux; _sub = mux.GetSubscriber(); }

        public Task PublishAsync<T>(T @event, CancellationToken ct = default)
        {
            var payload = JsonSerializer.Serialize(new { type = @event!.GetType().Name, occurredAtUtc = DateTime.UtcNow, data = @event });
            return _sub.PublishAsync(Channel, payload);
        }

        public ValueTask DisposeAsync() => _mux.DisposeAsync();
    }
}
