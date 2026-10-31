using FloatSoda;
using FloatSoda.Abstractions.Geometries;
using FloatSoda.OVR;
using FloatSoda.Samples.Settings;
using FloatSoda.Widgets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SkiaSharp;

// --desktopでもSteamVR/OpenVRとHMDの接続が必要です。
var useDesktop = false;
var screenViewPath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "FloatSoda",
    "Apps",
    "ScreenView",
    "FloatSoda.Apps.ScreenView.exe"
);
var hostArgs = new List<string>();
for (var index = 0; index < args.Length; index++)
{
    if (args[index] == "--desktop")
        useDesktop = true;
    else if (args[index] == "--screenview-exe")
    {
        if (index + 1 >= args.Length || args[index + 1].StartsWith("--", StringComparison.Ordinal))
            throw new ArgumentException(
                "--screenview-exeにはScreenViewのexeパスを指定してください。"
            );
        screenViewPath = args[++index];
    }
    else
        hostArgs.Add(args[index]);
}
var builder = Host.CreateApplicationBuilder(hostArgs.ToArray());
builder.Services.AddFloatSoda(
    new FloatSodaOptions { AppKey = new AppKey("FloatSoda.Samples.Settings") }
);

using var host = builder.Build();
var app = host.Services.GetRequiredService<FloatSodaApp>();
using var screenView = new ScreenViewLauncher(screenViewPath);
var demo = new SettingsDemo { LaunchScreenView = screenView.Start };

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
