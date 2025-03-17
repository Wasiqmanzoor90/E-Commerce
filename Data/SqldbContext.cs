using E_Commerce.Models.DomainModel;
using E_Commerce.Models.JunctionModel;
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
    public DbSet<Address> Addresses { get; set;}
    public DbSet<Review>Reviews { get; set;}
    public DbSet<OrderProduct> OrderProducts { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // One User has One Cart (Corrected to one-to-one)
        modelBuilder.Entity<User>()
            .HasOne(u => u.Cart)   //One User has one Cart
            .WithOne(c => c.user)  //One Cart belongs to one User
            .HasForeignKey<Cart>(c => c.UserId)  // ForeignKey from Cart to User
                   .OnDelete(DeleteBehavior.NoAction);  // ⬅️ Change to RESTRICT


        // One User can have many Orders (One-to-many)
        modelBuilder.Entity<User>()
            .HasMany(u => u.Orders)
            .WithOne(o => o.user)
            .HasForeignKey(o => o.UserId)
               .OnDelete(DeleteBehavior.NoAction);


        // One Cart can have many CartItems (One-to-many)
        modelBuilder.Entity<Cart>()
            .HasMany(c => c.CartItems)
            .WithOne(ci => ci.Cart)
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.NoAction);


        modelBuilder.Entity<CartItem>()
            .HasKey(ci => ci.CartItemId);

        // One Product can have many CartItems (One-to-many)
        modelBuilder.Entity<Product>()
            .HasMany(p => p.CartItems)
            .WithOne(ci => ci.Product)
            .HasForeignKey(ci => ci.ProductId)
            .OnDelete(DeleteBehavior.NoAction);

        // One Cart can have many Orders (One-to-many)
        modelBuilder.Entity<Cart>()
            .HasMany(c => c.Orders)
            .WithOne(o => o.cart)
            .HasForeignKey(o => o.CartId)
            .OnDelete(DeleteBehavior.NoAction);
        // ❌ Prevents multiple cascade paths

        modelBuilder.Entity<User>()
            .HasMany(a=>a.Addresses) //User can have many Adresses
            .WithOne(u=> u.User) // And the User is one
            .HasForeignKey(u => u.UserId) // ForeignKey from Adress to User
             .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<User>()
            .HasMany(r => r.Reviews)      //User can have many Reviews
            .WithOne(u => u.user)         // And the User is one
            .HasForeignKey(u => u.UserId)  // ForeignKey from Review to User
              .OnDelete(DeleteBehavior.NoAction);


        modelBuilder.Entity<OrderProduct>()
            .HasKey(op => op.OrderProductId);

        modelBuilder.Entity<OrderProduct>()
            .HasOne(op => op.order)
            .WithMany(o => o.OrderProducts) //has many
            .HasForeignKey(op => op.OrderId);






        modelBuilder.Entity<Order>()
    .HasOne(o => o.Address)   // One Order has One Address
    .WithMany()  // No navigation property needed in Address
    .HasForeignKey(o => o.AddressId) // Foreign key in Order
    .OnDelete(DeleteBehavior.NoAction);  // Prevent cascade delete issues




        modelBuilder.Entity<OrderProduct>()
            .HasOne(op => op.product)
            .WithMany(p => p.OrderProducts)             //same product can be in many orders
                .HasForeignKey(op => op.ProductId);
    }
}             