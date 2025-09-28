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
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "eBooks.API"))
            .AddJsonFile("appsettings.json", optional: true)
            .AddUserSecrets<Program>()
            .Build();
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddDbContext<EBooksContext>(options => options.UseSqlServer(configuration.GetConnectionString("Database")));
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
        try
        {
            var bus = RabbitHutch.CreateBus("host=localhost;username=guest;password=guest");
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
        }
        catch
        {
            Console.WriteLine("RabbitMQ is not activated, press <return> key to exit...");
        }
        Console.ReadLine();
    }
}
