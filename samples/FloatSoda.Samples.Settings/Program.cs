using FloatSoda;
using FloatSoda.Abstractions.Geometries;
using FloatSoda.OVR;
using FloatSoda.Samples.Settings;
using FloatSoda.Widgets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SkiaSharp;

// --desktopでもSteamVR/OpenVRとHMDの接続が必要です。
var useDesktop = args.Contains("--desktop");
var hostArgs = args.Where(arg => arg != "--desktop").ToArray();
var builder = Host.CreateApplicationBuilder(hostArgs);
builder.Services.AddFloatSoda(
    new FloatSodaOptions { AppKey = new AppKey("FloatSoda.Samples.Settings") }
);

using var host = builder.Build();
var app = host.Services.GetRequiredService<FloatSodaApp>();
var demo = new SettingsDemo();

// SteamVR上での物理倍率とは別に、候補の開閉でも変わらない論理キャンバスを使う。
var canvasSize = new SKSize(960, 1000);

app.CreateWindow(
    useDesktop
        ? new DesktopWindow
        {
            Title = "Settings Widgets",
            Size = canvasSize,
            Child = demo,
        }
        : new DashboardWindow
        {
            Dpm = new Dpm(1000),
            Title = "Settings Widgets",
            Size = canvasSize,
            Child = demo,
        }
);

await host.RunAsync();
