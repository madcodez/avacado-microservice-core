using Avacado.Services.RewardAPI.Message;


namespace Avacado.Services.RewardAPI.Services
{
    public interface IRewardsService
    {
        Task UpdateRewards(RewardsMessage message);
        
    }
}

