using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var serviceBusConnectionString = config["AzureSecrets:ServiceBusConnectionString"];

var adminClient = new ServiceBusAdministrationClient(serviceBusConnectionString);

string queueName = "mygeneralqueue";

if (await adminClient.QueueExistsAsync(queueName))
{
    var client = new ServiceBusClient(serviceBusConnectionString);
    var reciever = client.CreateReceiver(queueName);
    while (true)
    {
        var message = await reciever.ReceiveMessageAsync();
        if (message == null) break;
        Console.Write(message.Body.ToString());
        await reciever.CompleteMessageAsync(message);
    }
    await reciever.CloseAsync();
}

Console.WriteLine("there is no mygeneralqueue to recieve messages from");
Console.ReadLine();
