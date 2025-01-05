using MQTTunnelNet;
using System.Net;

await MQTTunnel.CreateServer("config.json").StartAsync();
await MQTTunnel.CreateTunnel("config.json", 9000, 8000).StartAsync();

Thread.Sleep(5000);

var testResult = new WebClient().DownloadString("http://localhost:9000");

Console.WriteLine(testResult);

Console.ReadLine();