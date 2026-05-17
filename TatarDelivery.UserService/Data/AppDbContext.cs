using Microsoft.EntityFrameworkCore;
using TatarDelivery.UserService.Domain;

namespace TatarDelivery.UserService.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Address> Addresses => Set<Address>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(user => user.Id);
            entity.Property(user => user.Email).HasMaxLength(100).IsRequired();
            entity.Property(user => user.PasswordHash).IsRequired();
            entity.Property(user => user.PasswordSalt).IsRequired();
            entity.Property(user => user.FullName).HasMaxLength(100).IsRequired();
            entity.Property(user => user.Phone).HasMaxLength(20).IsRequired();
            entity.Property(user => user.AuthToken).HasMaxLength(64);
            entity.Property(user => user.CreatedAtUtc).IsRequired();
            entity.Property(user => user.UpdatedAtUtc).IsRequired();
            entity.HasIndex(user => user.Email).IsUnique();
        });

        modelBuilder.Entity<Address>(entity =>
        {
            entity.ToTable("Addresses");
            entity.HasKey(address => address.Id);
            entity.Property(address => address.City).HasMaxLength(50).IsRequired();
            entity.Property(address => address.Street).HasMaxLength(100).IsRequired();
            entity.Property(address => address.House).HasMaxLength(10).IsRequired();
            entity.Property(address => address.Apartment).HasMaxLength(10);
            entity.Property(address => address.Entrance).HasMaxLength(5);
            entity.Property(address => address.Comment).HasMaxLength(255);
            entity.Property(address => address.CreatedAtUtc).IsRequired();

            entity
                .HasOne(address => address.User)
                .WithMany(user => user.Addresses)
                .HasForeignKey(address => address.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
