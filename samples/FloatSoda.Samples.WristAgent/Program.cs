using System.Numerics;
using FloatSoda;
using FloatSoda.Abstractions.Geometries;
using FloatSoda.OVR;
using FloatSoda.OVR.Input;
using FloatSoda.OVR.Overlay;
using FloatSoda.Samples.WristAgent;
using FloatSoda.Widgets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddFloatSoda(new FloatSodaOptions
{
    AppKey = new AppKey("FloatSoda.Samples.WristAgent"),
});

using var host = builder.Build();

// ブローカーと HTTP サーバーの生成。
// コマンドライン/環境変数でポートを変えられるようにしておく。
var port = args.Length > 0 && int.TryParse(args[0], out var parsed) ? parsed : 8797;
var app = host.Services.GetRequiredService<FloatSodaApp>();

var broker = new ApprovalBroker(a => app.Post(a));
var input = new WristInputService(broker, a => app.Post(a));
var server = new WristHttpServer(broker, port);

// user run
app.CreateWindow(new DeviceTrackedWindow
{
    Title = "WristAgent",
    Target = TrackedDevice.LeftController,
    Offset = new Vector3(-0.12f, 0, 0.22f),
    Dpm = new Dpm(4000),
    Child = new WristAgentCard(broker, input),
});

server.Start();

await host.RunAsync();
