using System.Text.Json;
using FloatSoda.Elements;
using FloatSoda.Geometrics;
using FloatSoda.Gesture;
using FloatSoda.Painting;
using FloatSoda.Widgets;
using FloatSoda.Widgets.Gesture;
using FloatSoda.Widgets.Layout;
using FloatSoda.Widgets.Settings;

namespace FloatSoda.Samples.Settings;

/// <summary>設定画面の状態更新とアプリ側JSON保存を紹介するサンプルです。</summary>
public sealed record SettingsDemo : StatefulWidget<SettingsDemo>
{
    /// <summary>ScreenViewを起動し、画面に表示する結果メッセージを返します。</summary>
    public Func<string>? LaunchScreenView { get; init; }

    /// <inheritdoc/>
    public override State<SettingsDemo> CreateState() => new SettingsDemoState();
}

/// <summary>設定値の所有者。Settings Widget側では永続化を扱いません。</summary>
public sealed class SettingsDemoState : State<SettingsDemo>
{
    private bool _notifications = true;
    private bool _haptics = true;
    private double _scale = 0.6;
    private string _quality = "Standard";
    private string _launchMessage = "起動ボタンで画面共有を開始";

    private static readonly string SavePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "FloatSoda",
        "settings-ui-sample.json"
    );

    /// <inheritdoc/>
    public override void InitState()
    {
        try
        {
            if (!File.Exists(SavePath))
                return;
            var loaded = JsonSerializer.Deserialize<SettingsSnapshot>(File.ReadAllText(SavePath));
            if (loaded is null)
                return;
            _notifications = loaded.Notifications;
            _haptics = loaded.Haptics;
            _scale = Math.Clamp(double.IsFinite(loaded.Scale) ? loaded.Scale : 0.6, 0, 1);
            _quality = new[] { "Standard", "Comfort", "Performance" }.Contains(loaded.Quality)
                ? loaded.Quality
                : "Standard";
        }
        catch (Exception ex)
            when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            Console.Error.WriteLine($"設定の読み込みに失敗しました: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public override Widget Build(IBuildContext context) =>
        new SizedBox
        {
            Width = 960,
            Child = new SettingsPage
            {
                Title = "Overlay Settings",
                Sections =
                {
                    new SettingsSection
                    {
                        Title = "General",
                        Children =
                        {
                            new SettingsTile
                            {
                                Title = "Notifications",
                                Description = "Enable dashboard notifications",
                                Trailing = new SettingsSwitch
                                {
                                    Value = _notifications,
                                    OnChanged = value => Change(() => _notifications = value),
                                },
                            },
                            new SettingsTile
                            {
                                Title = "Controller haptics",
                                Description = "Vibration feedback during interaction",
                                Trailing = new SettingsSwitch
                                {
                                    Value = _haptics,
                                    OnChanged = value => Change(() => _haptics = value),
                                },
                            },
                            new SettingsTile
                            {
                                Title = "ScreenView",
                                Description = _launchMessage,
                                Trailing = new GestureDetector
                                {
                                    Behaviour = HitTestBehaviour.Opaque,
                                    OnTap = Widget!.LaunchScreenView is null
                                        ? null
                                        : () =>
                                            SetState(() =>
                                                _launchMessage = Widget!.LaunchScreenView!()
                                            ),
                                    Child = new Container
                                    {
                                        Width = 300,
                                        Height = 56,
                                        Decoration = new BoxDecoration
                                        {
                                            Color = new Color(52, 66, 82),
                                            BorderRadius = BorderRadius.Circular(8),
                                        },
                                        Child = new Center
                                        {
                                            Child = new Text("ScreenViewを起動")
                                            {
                                                Style = new TextStyle
                                                {
                                                    FontSize = 22,
                                                    Color = new Color(242, 247, 252),
                                                },
                                            },
                                        },
                                    },
                                },
                            },
                        },
                    },
                    new SettingsSection
                    {
                        Title = "Display",
                        Children =
                        {
                            new SettingsTile
                            {
                                Title = "UI scale",
                                Description = "Adjust the overlay's logical scale",
                                Trailing = new SettingsSlider
                                {
                                    Min = 0,
                                    Max = 1,
                                    Value = _scale,
                                    OnChanged = value => Change(() => _scale = value),
                                },
                            },
                            new SettingsTile
                            {
                                Title = "Quality preset",
                                Description = "Choose the rendering preset",
                                Trailing = new SettingsDropdown
                                {
                                    Value = _quality,
                                    Options = ["Standard", "Comfort", "Performance"],
                                    OnChanged = value => Change(() => _quality = value),
                                },
                            },
                        },
                    },
                },
            },
        };

    private void Change(Action update)
    {
        SetState(update);
        Save();
    }

    private void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SavePath)!);
            File.WriteAllText(
                SavePath,
                JsonSerializer.Serialize(
                    new SettingsSnapshot
                    {
                        Notifications = _notifications,
                        Haptics = _haptics,
                        Scale = _scale,
                        Quality = _quality,
                    }
                )
            );
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Console.Error.WriteLine($"設定の保存に失敗しました: {ex.Message}");
        }
    }

    private sealed record SettingsSnapshot
    {
        public bool Notifications { get; init; } = true;
        public bool Haptics { get; init; } = true;
        public double Scale { get; init; } = 0.6;
        public string Quality { get; init; } = "Standard";
    }
}
