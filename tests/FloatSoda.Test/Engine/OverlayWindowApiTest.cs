using FloatSoda.Geometry;
using FloatSoda.Engine;
using FloatSoda.Engine.OVR;
using FloatSoda.OVR.Overlay;

namespace FloatSoda.Test.Engine;

public class OverlayWindowApiTest
{
    [Fact]
    public void Constructor_既存の三引数シグネチャ_バイナリ互換性を維持する()
    {
        Assert.NotNull(
            typeof(OverlayWindow).GetConstructor([typeof(IOverlay), typeof(Renderer), typeof(Dpm)])
        );
    }
}
