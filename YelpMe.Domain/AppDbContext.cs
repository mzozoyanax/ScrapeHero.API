using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YelpMe.Domain.Models;

namespace YelpMe.Domain
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=YelpMeLocalDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Business> Business { get; set; }

        public DbSet<Blocker> Blockers { get; set; }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<Template> Templates { get; set; }

        public DbSet<Setting> Settings { get; set; }

        public DbSet<Cloud> Clouds { get; set; }

        public DbSet<NameApiKey> NameApiKeys { get; set; }

        public DbSet<NameApiSetting> NameApiSettings { get; set; }

        public DbSet<WhatsAppIntergation> WhatsAppIntergations { get; set; }

        public DbSet<VerifyLogger> VerifyLoggers { get; set; }
    }
}
