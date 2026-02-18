using Microsoft.EntityFrameworkCore;
using server.Models;

namespace server.Dal
{
    public class SaleContext : DbContext
    {
        public DbSet<Donor> Donors { get; set; }
        public DbSet<Gift> Gifts { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<User> Users { get; set; }

        public SaleContext(DbContextOptions<SaleContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Gift>()
                .HasOne(g => g.Donor)
                .WithMany(d => d.Gifts)
                .HasForeignKey(g => g.DonorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Gift)
                .WithMany()
                .HasForeignKey(t => t.GiftId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>().HasIndex(e => e.Email).IsUnique();
            modelBuilder.Entity<Donor>().HasIndex(e => e.Email).IsUnique();

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username= "Avigail Maayani",
                    Email = "a0583290762@gmail.com",
                    Phone = "0583290762",
                    PasswordHash = "$2a$11$kWQWRcW0yZzTfcteU0tW4.hUxb6OWhRvLybxoCM21Sg4rEKnAvuO6",
                    Role = "manager"
                });

           base.OnModelCreating(modelBuilder);
        }
    }
}
