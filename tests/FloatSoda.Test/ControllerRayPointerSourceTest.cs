using FloatSoda.Abstractions.Geometries;
using FloatSoda.Abstractions.Input;
using FloatSoda.Engine;
using FloatSoda.OVR;
using SkiaSharp;

namespace FloatSoda.Test;

/// <summary>
/// コントローラーレイ入力源の生イベント合成をテストします。
/// ポール関数を直接注入するコンストラクターを使い、OpenVR なしで状態遷移を検証します。
/// </summary>
public class ControllerRayPointerSourceTest
{
    private static readonly Func<SKSizeI> TileSize = () => new SKSizeI(400, 300);

    private static List<RawPointerEvent> Pump(ControllerRayPointerSource source)
    {
        var events = new List<RawPointerEvent>();
        void OnEvent(RawPointerEvent e) => events.Add(e);

        source.OnPointerEvent += OnEvent;
        source.Pump();
        source.OnPointerEvent -= OnEvent;
        return events;
    }

    /// <summary>状態を持つポーラーを作成します。Poll() を呼ぶたびに next を返します。</summary>
    private static Func<PointerData?> Poller(params PointerData?[] steps)
    {
        var index = 0;
        return () => index < steps.Length ? (PointerData?)steps[index++] : null;
    }

    [Fact]
    public void Pump_未ヒット時_イベントを何も発火しない()
    {
        var source = new ControllerRayPointerSource(TileSize, [() => null]);

        var events = Pump(source);

        Assert.Empty(events);
    }

    [Fact]
    public void Pump_初ヒット時_EnterとMoveをその順で発火する()
    {
        var data = new PointerData(0.5f, 0.25f, PointerChange.Hover);
        var source = new ControllerRayPointerSource(TileSize, [Poller(data, data)]);

        var first = Pump(source);
        var second = Pump(source);

        Assert.All(first.Take(2), e => Assert.NotEqual(RawPointerKind.Leave, e.Kind));
        Assert.Equal(RawPointerKind.Enter, first[0].Kind);
        Assert.Equal(new Offset(200, 75), first[0].Position);
        Assert.Equal(RawPointerKind.Move, first[1].Kind);
        Assert.Equal(first[0].Position, first[1].Position);
        Assert.All(second, e => Assert.Equal(RawPointerKind.Move, e.Kind));
    }

    [Fact]
    public void Pump_押下検知時_Move後にButtonDownを発火する()
    {
        var hovering = new PointerData(0.5f, 0.5f, PointerChange.Hover);
        var pressed = hovering with { Change = PointerChange.Down };
        var source = new ControllerRayPointerSource(TileSize, [Poller(hovering, pressed)]);

        Pump(source);
        var events = Pump(source);

        // Hover 停留分の Move に加え、押下遷移フレームで Move → ButtonDown が来る
        Assert.Equal(RawPointerKind.ButtonDown, events[^1].Kind);
        Assert.Equal(RawPointerKind.Move, events[^2].Kind);
    }

    [Fact]
    public void Pump_解放検知時_Move後にButtonUpを発火する()
    {
        var hovering = new PointerData(0.5f, 0.5f, PointerChange.Hover);
        var pressed = hovering with { Change = PointerChange.Down };
        var released = hovering with { Change = PointerChange.Up };
        var source = new ControllerRayPointerSource(TileSize, [Poller(pressed, released)]);

        Pump(source);
        var events = Pump(source);

        Assert.Equal(RawPointerKind.ButtonUp, events[^1].Kind);
        Assert.Equal(RawPointerKind.Move, events[^2].Kind);
    }

    [Fact]
    public void Pump_ヒット解除時_Leaveだけを発火する()
    {
        var hovering = new PointerData(0.5f, 0.5f, PointerChange.Hover);
        var source = new ControllerRayPointerSource(TileSize, [Poller(hovering, null)]);

        Pump(source);
        var events = Pump(source);

        var leave = Assert.Single(events);
        Assert.Equal(RawPointerKind.Leave, leave.Kind);
        Assert.Equal(new Offset(200, 150), leave.Position);
    }

    [Fact]
    public void Pump_ヒット解除後の再ヒット_Enterから繰り返す()
    {
        var hovering = new PointerData(0.5f, 0.5f, PointerChange.Hover);
        var source = new ControllerRayPointerSource(TileSize, [Poller(hovering, null, hovering, hovering)]);

        Pump(source);
        var second = Pump(source);
        var third = Pump(source);

        Assert.Equal(new[] { RawPointerKind.Leave }, second.Select(e => e.Kind));
        Assert.Equal(RawPointerKind.Enter, third[0].Kind);
        Assert.Contains(third, e => e.Kind == RawPointerKind.Move);
    }

    [Fact]
    public void Pump_別デバイスでヒットしたとき_oldのLeaveとnewのEnterを順に発火する()
    {
        // leftPoller は2番目の Pump で miss、rightPoller も同じく miss(Leave のみのフレーム)、
        // 3番目の Pump で right が初ヒットする前提。
        var a = new PointerData(0.1f, 0.1f, PointerChange.Hover);
        var b = new PointerData(0.9f, 0.9f, PointerChange.Hover);
        var source = new ControllerRayPointerSource(TileSize, [Poller(a, null), Poller(null, null, b, b)]);

        Pump(source);
        var second = Pump(source);
        var third = Pump(source);

        // 2番目: a が miss したため a の Leave は出るが、b はヒットしていないので何も起きない
        var leave = Assert.Single(second);
        Assert.Equal(RawPointerKind.Leave, leave.Kind);
        Assert.Equal(new Offset(40, 30), leave.Position);

        // 3番目: b 初ヒット → Enter + Move
        Assert.Equal(RawPointerKind.Enter, third[0].Kind);
        Assert.Equal(new Offset(360, 270), third[0].Position);
    }
}
