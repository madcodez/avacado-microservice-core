using Avacado.Services.EmailAPI.Message;
using Avacado.Services.EmailAPI.Models.Dto;
using Avacado.Services.EmailAPI.Services;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;

namespace Avacado.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string emailCartQueue;
        private readonly string emailUserRegisterQueue;
        private readonly string orderCreatedTopic;
        private readonly string orderCreatedSubscription;
        private readonly IConfiguration _configuartion;
        private ServiceBusProcessor _cartProcessor;
        private ServiceBusProcessor _regProcessor;
        private ServiceBusProcessor _orderProcessor;
        private readonly EmailService _emailService; 
        public AzureServiceBusConsumer(IConfiguration configuartion,EmailService emailService) 
        {
            _configuartion = configuartion;

            serviceBusConnectionString = _configuartion.GetValue<string>("ServiceBusConnectionString");

            emailCartQueue = _configuartion.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");

            emailUserRegisterQueue = _configuartion.GetValue<string>("TopicAndQueueNames:RegisterUserQueue");

            orderCreatedTopic = _configuartion.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");

            orderCreatedSubscription = _configuartion.GetValue<string>("TopicAndQueueNames:OrderCreatedEmailSubscription");

            var client = new ServiceBusClient(serviceBusConnectionString);

            _cartProcessor = client.CreateProcessor(emailCartQueue);
            _regProcessor = client.CreateProcessor(emailUserRegisterQueue);
            _orderProcessor = client.CreateProcessor(orderCreatedTopic, orderCreatedSubscription);

            _emailService = emailService;


        }

        public async Task Start()
        {
            _cartProcessor.ProcessMessageAsync += OnEmailCartReqRec;
            _cartProcessor.ProcessErrorAsync += ErrorHandler;
            await _cartProcessor.StartProcessingAsync();

            _regProcessor.ProcessMessageAsync += OnUserRegisterReqRec;
            _regProcessor.ProcessErrorAsync += ErrorHandler;
            await _regProcessor.StartProcessingAsync();

            _orderProcessor.ProcessMessageAsync += OnOrderPlacedRequestReceived;
            _orderProcessor.ProcessErrorAsync += ErrorHandler;
            await _orderProcessor.StartProcessingAsync();

        }

        private async Task OnOrderPlacedRequestReceived(ProcessMessageEventArgs args)
        {
            //this is where you will receive message
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            RewardsMessage objMessage = JsonConvert.DeserializeObject<RewardsMessage>(body);
            try
            {
                //TODO - try to log email
                await _emailService.EmailOrderCreated(objMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        private async Task OnUserRegisterReqRec(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            string email = JsonConvert.DeserializeObject<string>(body);

            try
            {
                //TODDO - try to log email
                await _emailService.EmailUserRegisterLog(email);
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

        private async Task OnEmailCartReqRec(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            CartDto obj = JsonConvert.DeserializeObject<CartDto>(body);

            try
            {
                //TODDO - try to log email
                await _emailService.EmailCartLog(obj);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex) 
            {
                throw;
            }
        }

        public async Task Stop()
        {
            await _cartProcessor.StopProcessingAsync();  
            await _cartProcessor.DisposeAsync();

            await _regProcessor.StopProcessingAsync();
            await _regProcessor.DisposeAsync();

            await _orderProcessor.StopProcessingAsync();
            await _orderProcessor.DisposeAsync();
        }
    }
}
