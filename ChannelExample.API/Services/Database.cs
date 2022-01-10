using ChannelExample.API.Services.Data;
using Microsoft.EntityFrameworkCore;

namespace ChannelExample.API.Services;

public class Database:DbContext
{
    public Database(DbContextOptions<Database> options) : base(options) { }

    public DbSet<User>  Users { get; set; }
}
