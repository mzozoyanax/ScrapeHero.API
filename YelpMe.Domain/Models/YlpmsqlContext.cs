using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace YelpMe.Domain.Models;

public partial class YlpmsqlContext : DbContext
{
    public YlpmsqlContext()
    {
    }

    public YlpmsqlContext(DbContextOptions<YlpmsqlContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Blocker> Blockers { get; set; }

    public virtual DbSet<Business> Businesses { get; set; }

    public virtual DbSet<Cloud> Clouds { get; set; }

    public virtual DbSet<NameApiKey> NameApiKeys { get; set; }

    public virtual DbSet<NameApiSetting> NameApiSettings { get; set; }

    public virtual DbSet<Setting> Settings { get; set; }

    public virtual DbSet<Template> Templates { get; set; }

    public virtual DbSet<VerifyLogger> VerifyLoggers { get; set; }

    public virtual DbSet<WhatsAppIntergation> WhatsAppIntergations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("workstation id=TWSVnetDatabas.mssql.somee.com;packet size=4096;user id=Qhama_SQLLogin_1;pwd=6hl5oawm98;data source=TWSVnetDatabas.mssql.somee.com;persist security info=False;initial catalog=TWSVnetDatabas;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Business>(entity =>
        {
            entity.ToTable("Business");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
