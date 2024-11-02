using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var serviceBusConnectionString = config["AzureSecrets:ServiceBusConnectionString"];

string queueName = "mygenralqueue";
var client = new ServiceBusClient(serviceBusConnectionString);
var reciever = client.CreateReceiver(queueName);
while (true)
{
    var message = await reciever.ReceiveMessageAsync();
    if(message == null) break;
    Console.Write(message.Body.ToString());
    await reciever.CompleteMessageAsync(message);
}
await reciever.CloseAsync();
Console.WriteLine("Hello, World!");
Console.ReadLine();
