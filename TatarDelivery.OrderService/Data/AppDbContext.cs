using Microsoft.EntityFrameworkCore;
using TatarDelivery.OrderService.Domain;

namespace TatarDelivery.OrderService.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Order> Orders => Set<Order>();

    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    public DbSet<OrderStatusHistory> OrderStatusHistory => Set<OrderStatusHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Orders");
            entity.HasKey(order => order.Id);
            entity.Property(order => order.Status).HasMaxLength(50).IsRequired();
            entity.Property(order => order.TotalPrice).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(order => order.DeliveryPrice).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(order => order.CreatedAtUtc).IsRequired();
            entity.Property(order => order.UpdatedAtUtc).IsRequired();
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("OrderItems");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Price).HasColumnType("decimal(18,2)").IsRequired();

            entity
                .HasOne(item => item.Order)
                .WithMany(order => order.Items)
                .HasForeignKey(item => item.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OrderStatusHistory>(entity =>
        {
            entity.ToTable("OrderStatusHistory");
            entity.HasKey(history => history.Id);
            entity.Property(history => history.Status).HasMaxLength(50).IsRequired();
            entity.Property(history => history.ChangedBy).HasMaxLength(50).IsRequired();
            entity.Property(history => history.ChangedAtUtc).IsRequired();

            entity
                .HasOne(history => history.Order)
                .WithMany(order => order.StatusHistory)
                .HasForeignKey(history => history.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}