using FloatSoda.OVR.Overlay;
using FloatSoda.Widgets;
using FloatSoda.Widgets.Layout;

namespace FloatSoda.Test.Widgets;

/// <summary>
/// <see cref="WindowWidget.KeepOverlayAfterExit"/> の設定値がウィジェットとして正しく保持されることを検証します。
/// </summary>
/// <remarks>
/// オーバーレイの実破棄／非破棄はOpenVRコンテキストとSteamVRを要するため、ここではヘッドレスで検証できる
/// 既定値と <c>with</c> 式での伝播、ウィンドウ種別ごとの独立設定に限定します。
/// 破棄が実際に跳过される挙動は HMD 実機のハーネスで確認します。
/// </remarks>
public class KeepOverlayAfterExitTest
{
    [Fact]
    public void KeepOverlayAfterExit_既定値_偽である()
    {
        var window = new DeviceTrackedWindow
        {
            Title = "Wrist",
            Target = TrackedDevice.LeftController,
            Child = new SizedBox { Width = 1, Height = 1 }
        };

        Assert.False(window.KeepOverlayAfterExit);
    }

    [Fact]
    public void KeepOverlayAfterExit_trueを指定_読み取れる()
    {
        var window = new DashboardWindow
        {
            Title = "Board",
            KeepOverlayAfterExit = true,
            Child = new SizedBox { Width = 1, Height = 1 }
        };

        Assert.True(window.KeepOverlayAfterExit);
    }

    [Fact]
    public void KeepOverlayAfterExit_with式で変更_変更分だけ変化する()
    {
        var original = new WorldSpaceWindow
        {
            Title = "Panel",
            Child = new SizedBox { Width = 1, Height = 1 }
        };

        var kept = original with { KeepOverlayAfterExit = true };

        Assert.False(original.KeepOverlayAfterExit);
        Assert.True(kept.KeepOverlayAfterExit);
        Assert.Equal(original.Title, kept.Title);
        Assert.Equal(original.Position, kept.Position);
    }

    [Fact]
    public void KeepOverlayAfterExit_ウィンドウ種別ごとに独立して設定できる()
    {
        var dashboard = new DashboardWindow
        {
            Title = "Board",
            KeepOverlayAfterExit = true,
            Child = new SizedBox { Width = 1, Height = 1 }
        };
        var world = new WorldSpaceWindow
        {
            Title = "Panel",
            Child = new SizedBox { Width = 1, Height = 1 }
        };

        Assert.True(dashboard.KeepOverlayAfterExit);
        Assert.False(world.KeepOverlayAfterExit);
    }

    [Fact]
    public void KeepOverlayAfterExit_WorldSpaceWindowとDeviceTrackedWindowで独立に設定できる()
    {
        var world = new WorldSpaceWindow
        {
            Title = "Panel",
            KeepOverlayAfterExit = true,
            Child = new SizedBox { Width = 1, Height = 1 }
        };
        var wrist = new DeviceTrackedWindow
        {
            Title = "Wrist",
            Target = TrackedDevice.LeftController,
            Child = new SizedBox { Width = 1, Height = 1 }
        };

        Assert.True(world.KeepOverlayAfterExit);
        Assert.False(wrist.KeepOverlayAfterExit);
    }
}
