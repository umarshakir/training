using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var serviceBusConnectionString = config["AzureSecrets:ServiceBusConnectionString"];

string topicName = "privatechat";

Console.WriteLine("Enter your name");
string username = Console.ReadLine()??"";

var adminClient = new ServiceBusAdministrationClient(serviceBusConnectionString);

if (!await adminClient.TopicExistsAsync(topicName))
{
    await adminClient.CreateTopicAsync(topicName);
}

if(!await adminClient.SubscriptionExistsAsync(topicName, username))
{
    await adminClient.CreateSubscriptionAsync(new CreateSubscriptionOptions(topicName, username)
    {
        AutoDeleteOnIdle = TimeSpan.FromMinutes(5)
    });
}

var busClient = new ServiceBusClient(serviceBusConnectionString);

var sender = busClient.CreateSender(topicName);

var processor = busClient.CreateProcessor(topicName, username);
processor.ProcessMessageAsync += MessageHandler;
processor.ProcessErrorAsync += ErrorMessageHandler;
await processor.StartProcessingAsync();

var welcomeMessage = new ServiceBusMessage($"Welcome {username}!");
await sender.SendMessageAsync(welcomeMessage);

while (true)
{
    string userMessage = Console.ReadLine()??"";
    if (userMessage == "exit") break;

    await sender.SendMessageAsync(new ServiceBusMessage($"{username} > {userMessage}"));
}

await sender.SendMessageAsync(new ServiceBusMessage($"{username} has left the chat"));

await processor.StopProcessingAsync();
await processor.CloseAsync();
await sender.CloseAsync();





async Task ErrorMessageHandler(ProcessErrorEventArgs arg)
{
    throw new NotImplementedException();
}

async Task MessageHandler(ProcessMessageEventArgs arg)
{
    string message = arg.Message.Body.ToString();
    if (!message.StartsWith($"{username} >"))
    {
        Console.WriteLine(message);
    }    
    await arg.CompleteMessageAsync(arg.Message);
}

