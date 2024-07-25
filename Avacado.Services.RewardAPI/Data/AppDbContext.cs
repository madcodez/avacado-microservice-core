﻿using Avacado.Services.RewardAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Avacado.Services.RewardAPI.Data
{
    public class AppDbContext :DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Rewards> Rewards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           


        }

    }
}
