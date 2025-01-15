using MQTTunnelNet;
using System.Net;

await MQTTunnel.CreateServer("config.json").StartAsync();
await MQTTunnel.CreateTunnel("config.json", 9000, 8000).StartAsync();

await MQTTunnel.CreateServerMem(File.ReadAllText("config.json")).StartAsync();
await MQTTunnel.CreateTunnelMem(File.ReadAllText("config.json"), 9100, 8100).StartAsync();

Thread.Sleep(5000);

Console.WriteLine(new WebClient().DownloadString("http://localhost:9000"));
Console.WriteLine(new WebClient().DownloadString("http://localhost:9100"));

Console.ReadLine();