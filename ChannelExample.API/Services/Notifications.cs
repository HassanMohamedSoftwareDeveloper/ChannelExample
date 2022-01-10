using Microsoft.EntityFrameworkCore;

namespace ChannelExample.API.Services;

public class Notifications
{
    private readonly Database database;
    private readonly IHttpClientFactory httpClientFactory;

    public Notifications(Database database, IHttpClientFactory httpClientFactory)
    {
        this.database = database;
        this.httpClientFactory = httpClientFactory;
    }
    public async Task<bool> Send()
    {
        if (await database.Users.AnyAsync() is false)
        {
            database.Users.Add(new Data.User());
            await database.SaveChangesAsync();
        }
        var user = await database.Users.FirstOrDefaultAsync();
        var client = httpClientFactory.CreateClient();
        var response = await client.GetStringAsync("https://docs.microsoft.com");
        user.Message = response;
        await database.SaveChangesAsync();

        return true;

    }

    public bool SendA()
    {
        Task.Run(async () =>
        {
            try
            {
                if (await database.Users.AnyAsync() is false)
                {
                    database.Users.Add(new Data.User());
                    await database.SaveChangesAsync();
                }
                var user = await database.Users.FirstOrDefaultAsync();
                var client = httpClientFactory.CreateClient();
                var response = await client.GetStringAsync("https://docs.microsoft.com");
                user.Message = response;
                await database.SaveChangesAsync();
            }
            catch (Exception e)
            {
                var a = e;
            }
        });
        return true;

    }
}
