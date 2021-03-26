using Microsoft.EntityFrameworkCore;
using Quisy.SqlDbRepository.Entities;
using System;

namespace Quisy.SqlDbRepository
{
    public class QuisyDbContext : DbContext
    {
            public DbSet<LogsEntity> Logs { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlServer
                    (@"Server=localhost;Database=Quisy;Trusted_Connection=true;",
                    builder => builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(5), null));
            }
        
    }
}
