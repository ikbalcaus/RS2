using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using EasyNetQ;
using eBooks.Database;
using eBooks.Models.Messages;
using eBooks.MessageHandlers;
using eBooks.Subscriber.Services;
using eBooks.Subscriber.Interfaces;

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
        services.AddTransient<IMessageHandler<BookDiscounted>, BookDiscountHandler>();
        services.AddTransient<IMessageHandler<EmailVerification>, EmailVerificationHandler>();
        services.AddTransient<IMessageHandler<PaymentCompleted>, PaymentCompletedHandler>();
        services.AddSingleton<MessageDispatcher>();
        var serviceProvider = services.BuildServiceProvider();
        var dispatcher = serviceProvider.GetRequiredService<MessageDispatcher>();
        try
        {
            Console.WriteLine("Listening for messages, press <return> key to exit...");
            var bus = RabbitHutch.CreateBus("host=localhost;username=guest;password=guest");
            await bus.PubSub.SubscribeAsync<BookDiscounted>("book_discounted", dispatcher.Dispatch);
            await bus.PubSub.SubscribeAsync<EmailVerification>("email_verification", dispatcher.Dispatch);
            await bus.PubSub.SubscribeAsync<PaymentCompleted>("payment_completed", dispatcher.Dispatch);
        }
        catch
        {
            Console.WriteLine("RabbitMQ is not activated...");
        }
        Console.ReadLine();
    }
}
