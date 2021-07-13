using System;
using Microsoft.EntityFrameworkCore;

namespace BrackeysBot.Database
{
    public static class BrackeysBotContextFactory
    {
        public static void Create(this DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                .EnableServiceProviderCaching()
                // This isn't permanent, I just don't want to setup a MySQL server on my PC, it's easy to migrate 
                // to MySQL when we're out of the testing phase though
                .UseSqlite("Data Source=database.db");
        }
    }

    public class BrackeysBotContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .Create();
        }
    }
}