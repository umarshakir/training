using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var serviceBusConnectionString = config["AzureSecrets:ServiceBusConnectionString"];

string queueName = "mygeneralqueue";

var adminClient = new ServiceBusAdministrationClient(serviceBusConnectionString);

if (!await adminClient.QueueExistsAsync(queueName))
{
    await adminClient.CreateQueueAsync(queueName);
}

var client = new ServiceBusClient(serviceBusConnectionString);
var sender = client.CreateSender(queueName);
foreach(var item in queueName)
{
    var message = new ServiceBusMessage(item.ToString());
    sender.SendMessageAsync(message).Wait();
}
await sender.CloseAsync();

Console.WriteLine("Hello, World!");
Console.ReadLine();
