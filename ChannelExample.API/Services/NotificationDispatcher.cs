using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;

namespace ChannelExample.API.Services;

public class NotificationDispatcher : BackgroundService
{
    private readonly Channel<string> channel;
    private readonly ILogger<NotificationDispatcher> logger;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly IServiceProvider serviceProvider;

    public NotificationDispatcher(
        Channel<string> channel,
        ILogger<NotificationDispatcher> logger,
        IHttpClientFactory httpClientFactory,
        IServiceProvider serviceProvider)
    {
        this.channel = channel;
        this.logger = logger;
        this.httpClientFactory = httpClientFactory;
        this.serviceProvider = serviceProvider;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (channel.Reader.Completion.IsCompleted is false)//if not complete
        {
            //read from channel
            var msg = await channel.Reader.ReadAsync();
            //send notification

            try
            {
                using var scope = serviceProvider.CreateScope();
                var database = scope.ServiceProvider.GetRequiredService<Database>();
                if (await database.Users.AnyAsync(cancellationToken: stoppingToken) is false)
                {
                    database.Users.Add(new Data.User());
                    await database.SaveChangesAsync(stoppingToken);
                }
                var user = await database.Users.FirstOrDefaultAsync(cancellationToken: stoppingToken);
                var client = httpClientFactory.CreateClient();
                var response = await client.GetStringAsync("https://docs.microsoft.com", stoppingToken);
                user.Message = response;
                await database.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Notification failed");
            }

        }
    }
}
