using E_Commerce.Models.DomainModel;
using Microsoft.EntityFrameworkCore;
namespace E_Commerce.Data;

public class SqldbContext : DbContext
{
    public SqldbContext(DbContextOptions<SqldbContext> options) : base(options)
    {

    }


    public DbSet<User> Users {get; set;}
    public DbSet<Order> Orders { get; set;}
    public DbSet<Product> Products { get; set;}
    public DbSet<Cart> Carts {get; set;}
    public DbSet<CartItem> CartItems {get; set;}
    public DbSet<Adress> Adresses { get; set;}
    public DbSet<Review>Reviews { get; set;}


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // One User has One Cart (Corrected to one-to-one)
        modelBuilder.Entity<User>()
            .HasOne(u => u.Cart)   //One User has one Cart
            .WithOne(c => c.user)  //One Cart belongs to one User
            .HasForeignKey<Cart>(c => c.UserId)  // ForeignKey from Cart to User
                   .OnDelete(DeleteBehavior.Restrict);  // ⬅️ Change to RESTRICT


        // One User can have many Orders (One-to-many)
        modelBuilder.Entity<User>()
            .HasMany(u => u.Orders)
            .WithOne(o => o.user)
            .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);  // ✅ Keep CASCADE for CartItems


        // One Cart can have many CartItems (One-to-many)
        modelBuilder.Entity<Cart>()
            .HasMany(c => c.CartItems)
            .WithOne(ci => ci.Cart)
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade);  // ✅ If Cart is deleted, delete the CartItems

        // One Product can have many CartItems (One-to-many)
        modelBuilder.Entity<Product>()
            .HasMany(p => p.CartItems)
            .WithOne(ci => ci.Product)
            .HasForeignKey(ci => ci.ProductId)
            .OnDelete(DeleteBehavior.Cascade);  // ✅ If Product is deleted, delete the CartItems

        // One Cart can have many Orders (One-to-many)
        modelBuilder.Entity<Cart>()
            .HasMany(c => c.Orders)
            .WithOne(o => o.cart)
            .HasForeignKey(o => o.CartId)
             .OnDelete(DeleteBehavior.Restrict);  // ⬅️ Change to RESTRICT
                                                  // ❌ Prevents multiple cascade paths

        modelBuilder.Entity<User>()
            .HasMany(a=>a.Adresses) //User can have many Adresses
            .WithOne(u=> u.user) // And the User is one
            .HasForeignKey(u => u.UserId) // ForeignKey from Adress to User
            .OnDelete(DeleteBehavior.Cascade);//if the user is deleted Adress is also delted

        modelBuilder.Entity<User>()
            .HasMany(r => r.Reviews)      //User can have many Reviews
            .WithOne(u => u.user)         // And the User is one
            .HasForeignKey(u => u.UserId)  // ForeignKey from Review to User
            .OnDelete(DeleteBehavior.Cascade); //if the user is deleted review is also delted

    }
}             