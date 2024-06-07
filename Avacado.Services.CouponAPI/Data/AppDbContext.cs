using Avacado.Services.CouponAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Avacado.Services.CouponAPI.Data
{
    public class AppDbContext :DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Coupon> Coupons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Coupon>().HasData(new Coupon
            {
                Id = 1,
                CouponCode = "10FF0",
                DiscountAmount = 10,
                MinAomunt = 20

            });

            modelBuilder.Entity<Coupon>().HasData(new Coupon
            {
                Id = 2,
                CouponCode = "12CC0",
                DiscountAmount = 15,
                MinAomunt = 40

            });
        }
    }
}
