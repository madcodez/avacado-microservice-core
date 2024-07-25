
using Avacado.Services.RewardAPI.Message;
using Avacado.Services.RewardAPI.Services;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;

namespace Avacado.Services.RewardAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string orderCreatedTopic;
        private readonly string rewardUpdateSubscription;

        private readonly IConfiguration _configuartion;
        private ServiceBusProcessor _rewardProcessor;
        
        private readonly RewardsService _rewardService; 
        public AzureServiceBusConsumer(IConfiguration configuartion, RewardsService rewardService) 
        {
            _configuartion = configuartion;

            serviceBusConnectionString = _configuartion.GetValue<string>("ServiceBusConnectionString");

            orderCreatedTopic = _configuartion.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");

            rewardUpdateSubscription = _configuartion.GetValue<string>("TopicAndQueueNames:OrderCreatedRewardsUpdateSubscription");

            var client = new ServiceBusClient(serviceBusConnectionString);

            _rewardProcessor = client.CreateProcessor(orderCreatedTopic, rewardUpdateSubscription);


            _rewardService = rewardService;


        }

        public async Task Start()
        {
            _rewardProcessor.ProcessMessageAsync += OnNewOrderRewardsRequestReceived;
            _rewardProcessor.ProcessErrorAsync += ErrorHandler;
            await _rewardProcessor.StartProcessingAsync();

            

        }

        private async Task OnNewOrderRewardsRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            RewardsMessage reward = JsonConvert.DeserializeObject<RewardsMessage>(body);

            try
            {
                //TODDO - try to log email
                await _rewardService.UpdateRewards(reward);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

       

        public async Task Stop()
        {
            await _rewardProcessor.StopProcessingAsync();  
            await _rewardProcessor.DisposeAsync();

        }
    }
}
