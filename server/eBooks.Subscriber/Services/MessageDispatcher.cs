using eBooks.Subscriber.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace eBooks.MessageHandlers
{
    public class MessageDispatcher
    {
        protected IServiceProvider _serviceProvider;

        public MessageDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Dispatch<T>(T message)
        {
            using var scope = _serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler<T>>();
            await handler.SendEmail(message);
            await handler.NotifyUser(message);
        }
    }
}
