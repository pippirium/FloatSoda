using FloatSoda.Abstractions.Geometries;
using FloatSoda.Abstractions.Input;
using FloatSoda.Core;
using FloatSoda.RenderObjects;
using FloatSoda.Widgets;
using FloatSoda.Widgets.Layout;
using FloatSoda.Widgets.Settings;

namespace FloatSoda.Test.Widgets.Settings;

public class SettingsWidgetsTest
{
    [Fact]
    public void ClampValue_範囲外の値_最小値と最大値に収める()
    {
        Assert.Equal(0, SettingsSliderState.ClampValue(-10, 0, 1));
        Assert.Equal(1, SettingsSliderState.ClampValue(10, 0, 1));
        Assert.Equal(0.75, SettingsSliderState.ClampValue(0.75, 0, 1));
    }

    [Fact]
    public void ClampValue_非有限値または無効範囲_例外を投げる()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => SettingsSliderState.ClampValue(double.NaN, 0, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => SettingsSliderState.ClampValue(0.5, 1, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => SettingsSliderState.ClampValue(0.5, double.NegativeInfinity, 1));
    }

    [Fact]
    public void OnTap_スイッチを押す_反対の値を通知する()
    {
        bool? changed = null;
        var binding = Mount(new SettingsSwitch { Value = false, OnChanged = value => changed = value });
        Tap(binding, 50, 30);
        Assert.True(changed);
    }

    [Fact]
    public void OnTap_外部から値を更新する_次の操作には更新後の値を使う()
    {
        var changes = new List<bool>();
        var binding = Mount(new SettingsSwitch { Value = false, OnChanged = changes.Add });
        Tap(binding, 50, 30);
        binding.AttachRootWidget(new SettingsSwitch { Value = true, OnChanged = changes.Add });
        binding.FlushBuildAndLayout();
        Tap(binding, 50, 30);
        Assert.Equal([true, false], changes);
    }

    [Fact]
    public void OnPointerDown_スライダートラックの両端_境界値を通知する()
    {
        var values = new List<double>();
        var binding = Mount(new SizedBox
        {
            Width = 300,
            Height = 100,
            Child = new SettingsSlider { Min = 10, Max = 20, Value = 15, OnChanged = values.Add }
        });

        Send(binding, PointerEventPhase.Add, 1, 55);
        Send(binding, PointerEventPhase.Down, 1, 55);
        Send(binding, PointerEventPhase.Up, 1, 55);
        Send(binding, PointerEventPhase.Down, 299, 55);
        Send(binding, PointerEventPhase.Up, 299, 55);
        Assert.Equal(2, values.Count);
        Assert.InRange(values[0], 10, 10.1);
        Assert.InRange(values[1], 19.9, 20);
    }

    [Fact]
    public void OnPointerMove_スライダー押下中_ドラッグ移動の値を通知する()
    {
        var values = new List<double>();
        var binding = Mount(new SizedBox
        {
            Width = 300,
            Height = 100,
            Child = new SettingsSlider { Min = 0, Max = 1, Value = 0, OnChanged = values.Add }
        });
        Send(binding, PointerEventPhase.Add, 10, 55);
        Send(binding, PointerEventPhase.Down, 10, 55);
        Send(binding, PointerEventPhase.Move, 150, 55);
        Send(binding, PointerEventPhase.Up, 150, 55);
        var count = values.Count;
        Send(binding, PointerEventPhase.Move, 250, 55);
        Assert.True(count >= 2);
        Assert.Equal(count, values.Count);
        Assert.InRange(values[^1], 0.48, 0.52);
    }

    [Fact]
    public void TrackWidth_狭いトラックの中央を押す_中間値を通知する()
    {
        var values = new List<double>();
        var binding = Mount(new SizedBox
        {
            Width = 140,
            Height = 100,
            Child = new SettingsSlider
            {
                Min = 0,
                Max = 1,
                Value = 0,
                TrackWidth = 140,
                OnChanged = values.Add
            }
        });

        Send(binding, PointerEventPhase.Add, 70, 55);
        Send(binding, PointerEventPhase.Down, 70, 55);
        Assert.NotEmpty(values);
        Assert.InRange(values[^1], 0.45, 0.55);
    }

    [Fact]
    public void TrackWidth_不正な幅_例外を投げる()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Mount(new SizedBox
        {
            Width = 140,
            Height = 100,
            Child = new SettingsSlider { Min = 0, Max = 1, Value = 0.5, TrackWidth = 0 }
        }));
    }

    [Fact]
    public void OnTap_ドロップダウンで候補を選ぶ_選択値を通知する()
    {
        string? selected = null;
        var binding = Mount(new SizedBox
        {
            Width = 228,
            Height = 260,
            Child = new SettingsDropdown
            {
                Value = "First",
                Options = ["First", "Second"],
                OnChanged = value => selected = value
            }
        });

        Tap(binding, 100, 28);
        binding.FlushBuildAndLayout();
        Tap(binding, 100, 152);
        Assert.Equal("Second", selected);
    }

    [Fact]
    public void ClampValue_有限値でも差が無限になる範囲_例外を投げる()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => SettingsSliderState.ClampValue(0, -1e308, 1e308));
    }

    [Fact]
    public void Build_ページ幅の変更_レイアウトを再計算できる()
    {
        static Widget CreatePage(double width) => new SizedBox
        {
            Width = width,
            Height = 350,
            Child = new SettingsPage
            {
                Title = "Settings",
                Sections =
                {
                    new SettingsSection
                    {
                        Title = "General",
                        Children =
                        {
                            new SettingsTile
                            {
                                Title = "Enabled",
                                Trailing = new SettingsSwitch { Value = true }
                            }
                        }
                    }
                }
            }
        };

        var binding = Mount(CreatePage(840));
        binding.AttachRootWidget(CreatePage(700));
        binding.FlushBuildAndLayout();
    }

    [Fact]
    public void OnTap_外部からDropdown値を変更する_次の選択が通知される()
    {
        var changes = new List<string>();
        Widget Build(string value) => new SizedBox
        {
            Width = 228,
            Height = 260,
            Child = new SettingsDropdown
            {
                Value = value,
                Options = ["First", "Second"],
                OnChanged = changes.Add
            }
        };

        var binding = Mount(Build("First"));
        binding.AttachRootWidget(Build("Second"));
        binding.FlushBuildAndLayout();
        Tap(binding, 100, 28);
        binding.FlushBuildAndLayout();
        Tap(binding, 100, 88);
        Assert.Equal(["First"], changes);
    }

    private static WidgetBinding Mount(Widget root)
    {
        var binding = new WidgetBinding(new RenderView());
        binding.AttachRootWidget(root);
        binding.FlushBuildAndLayout();
        return binding;
    }

    private static void Tap(WidgetBinding binding, double x, double y)
    {
        Send(binding, PointerEventPhase.Add, x, y);
        Send(binding, PointerEventPhase.Down, x, y);
        Send(binding, PointerEventPhase.Up, x, y);
    }

    private static void Send(WidgetBinding binding, PointerEventPhase phase, double x, double y)
        => binding.HandlePointerEvent(new PointerEvent(1, phase, new Offset(x, y)));
}
