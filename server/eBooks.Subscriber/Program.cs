using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using EasyNetQ;
using eBooks.Database;
using eBooks.Models.Messages;
using eBooks.MessageHandlers;
using eBooks.Subscriber.Services;
using eBooks.Subscriber.Interfaces;
using eBooks.Subscriber.MessageHandlers;

class Program
{
    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        var connectionString = Environment.GetEnvironmentVariable("_connectionString");
        services.AddDbContext<EBooksContext>(options => options.UseSqlServer(Environment.GetEnvironmentVariable("_connectionString")));
        services.AddTransient<EmailService>();
        services.AddSingleton<MessageDispatcher>();
        services.AddTransient<IMessageHandler<AccountDeactivated>, AccountDeactivatedHandler>();
        services.AddTransient<IMessageHandler<BookDeactivated>, BookDeactivatedHandler>();
        services.AddTransient<IMessageHandler<BookDiscounted>, BookDiscountedHandler>();
        services.AddTransient<IMessageHandler<BookReviewed>, BookReviewedHandler>();
        services.AddTransient<IMessageHandler<EmailVerification>, EmailVerificationHandler>();
        services.AddTransient<IMessageHandler<PaymentCompleted>, PaymentCompletedHandler>();
        services.AddTransient<IMessageHandler<PasswordForgotten>, PasswordForgottenHandler>();
        services.AddTransient<IMessageHandler<PublisherFollowing>, PublisherFollowingHandler>();
        services.AddTransient<IMessageHandler<PublisherVerified>, PublisherVerifiedHandler>();
        services.AddTransient<IMessageHandler<QuestionAnswered>, QuestionAnsweredHandler>();
        var serviceProvider = services.BuildServiceProvider();
        var dispatcher = serviceProvider.GetRequiredService<MessageDispatcher>();
        var bus = default(IBus);
        while (true)
        {
            try
            {
                var rabbitMqHost = Environment.GetEnvironmentVariable("_rabbitMqHost") ?? "localhost";
                var rabbitMqUser = Environment.GetEnvironmentVariable("_rabbitMqUser") ?? "guest";
                var rabbitMqPassword = Environment.GetEnvironmentVariable("_rabbitMqPassword") ?? "guest";

                bus = RabbitHutch.CreateBus($"host={rabbitMqHost};username={rabbitMqUser};password={rabbitMqPassword}");
                Console.WriteLine("Connected to RabbitMQ successfully.");
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RabbitMQ connection failed: {ex.Message}. Retrying in 5 seconds...");
                await Task.Delay(5000);
            }
        }
        await bus.PubSub.SubscribeAsync<AccountDeactivated>("account_deactivated", dispatcher.Dispatch);
        await bus.PubSub.SubscribeAsync<BookDeactivated>("book_deactivated", dispatcher.Dispatch);
        await bus.PubSub.SubscribeAsync<BookDiscounted>("book_discounted", dispatcher.Dispatch);
        await bus.PubSub.SubscribeAsync<BookReviewed>("book_reviewed", dispatcher.Dispatch);
        await bus.PubSub.SubscribeAsync<EmailVerification>("email_verification", dispatcher.Dispatch);
        await bus.PubSub.SubscribeAsync<PaymentCompleted>("payment_completed", dispatcher.Dispatch);
        await bus.PubSub.SubscribeAsync<PasswordForgotten>("password_forgotten", dispatcher.Dispatch);
        await bus.PubSub.SubscribeAsync<PublisherFollowing>("publisher_following", dispatcher.Dispatch);
        await bus.PubSub.SubscribeAsync<PublisherVerified>("publisher_verified", dispatcher.Dispatch);
        await bus.PubSub.SubscribeAsync<QuestionAnswered>("question_answered", dispatcher.Dispatch);
        Console.WriteLine("Listening for messages, press <return> key to exit...");
        await Task.Delay(Timeout.Infinite);
    }
}
