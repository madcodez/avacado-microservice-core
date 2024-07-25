using Avacado.Services.RewardAPI.Data;

using Avacado.Services.RewardAPI.Message;
using Avacado.Services.RewardAPI.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Avacado.Services.RewardAPI.Models;

namespace Avacado.Services.RewardAPI.Services
{
    public class RewardsService : IRewardsService
    {
        private DbContextOptions<AppDbContext> _options;

        public RewardsService(DbContextOptions<AppDbContext> options)
        {
            _options = options;
        }

     

     

        public async Task UpdateRewards(RewardsMessage message)
        {
            try
            {
                Rewards rewards = new()
                {
                    OrderId = message.OrderId,
                    RewardsActivity = message.RewardsActivity,
                    UserId = message.UserId,
                    RewardsDate = DateTime.Now
                };
                await using var _db = new AppDbContext(_options);
                await _db.Rewards.AddAsync(rewards);
                await _db.SaveChangesAsync();
              
            }
            catch (Exception ex)
            {
                
            }
        }

      
    }
}
