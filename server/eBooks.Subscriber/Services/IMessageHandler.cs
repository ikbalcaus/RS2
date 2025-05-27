namespace eBooks.Subscriber.Interfaces
{
    public interface IMessageHandler<T>
    {
        Task SendEmail(T message);
        Task NotifyUser(T message);
    }
}
