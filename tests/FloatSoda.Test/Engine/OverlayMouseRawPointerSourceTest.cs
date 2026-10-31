using FloatSoda.Abstractions.Geometries;
using FloatSoda.Abstractions.Input;
using FloatSoda.Engine;
using FloatSoda.OVR;
using SkiaSharp;

namespace FloatSoda.Test.Engine;

public class OverlayMouseRawPointerSourceTest
{
    private sealed class Dispatcher : VREventDispatcher
    {
        public void Send(EVREventType type, float x, float y) =>
            Dispatch(
                new VREvent_t
                {
                    eventType = (uint)type,
                    data = new VREvent_Data_t
                    {
                        mouse = new VREvent_Mouse_t
                        {
                            x = x,
                            y = y,
                            button = (uint)EVRMouseButton.Left,
                        },
                    },
                }
            );

        public override void PollEvents() { }
    }

    [Fact]
    public void PointerEvents_正規化入力とキャンバス変更_各時点のUI座標へ変換する()
    {
        var dispatcher = new Dispatcher();
        var size = new SKSizeI(960, 722);
        using var source = new OverlayMouseRawPointerSource(
            dispatcher,
            position => new Offset(position.X * size.Width, position.Y * size.Height)
        );
        using var controller = new PointerController(source);
        var events = new List<PointerEvent>();
        controller.OnPointerEvent += events.Add;

        dispatcher.Send(EVREventType.VREvent_MouseMove, 0.5f, 0.5f);
        dispatcher.Send(EVREventType.VREvent_MouseButtonDown, 0.5f, 0.5f);
        controller.Flush();
        var down = Assert.Single(events, e => e.Phase == PointerEventPhase.Down);
        Assert.Equal(new Offset(480, 361), down.Position);

        size = new SKSizeI(960, 1000);
        dispatcher.Send(EVREventType.VREvent_MouseMove, 0.75f, 0.5f);
        dispatcher.Send(EVREventType.VREvent_MouseButtonUp, 0.75f, 0.5f);
        controller.Flush();
        Assert.Equal(
            new Offset(720, 500),
            events.Last(e => e.Phase == PointerEventPhase.Move).Position
        );
        var up = Assert.Single(events, e => e.Phase == PointerEventPhase.Up);
        Assert.Equal(new Offset(720, 500), up.Position);
        Assert.Equal(down.PointerId, up.PointerId);
    }

    [Fact]
    public void PointerEvents_従来のピクセル入力_座標をそのまま渡す()
    {
        var dispatcher = new Dispatcher();
        using var source = new OverlayMouseRawPointerSource(dispatcher);
        var events = new List<RawPointerEvent>();
        source.OnPointerEvent += events.Add;
        dispatcher.Send(EVREventType.VREvent_MouseButtonDown, 756, 520);
        Assert.Equal(new Offset(756, 520), Assert.Single(events).Position);
    }
}
