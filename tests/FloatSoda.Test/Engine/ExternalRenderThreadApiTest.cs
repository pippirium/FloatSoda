using FloatSoda;

namespace FloatSoda.Test.Engine;

public class ExternalRenderThreadApiTest
{
    [Fact]
    public void PostRender_外部描画タスク投入の公開APIを持つ()
    {
        Assert.NotNull(typeof(FloatSodaApp).GetMethod(
            nameof(FloatSodaApp.PostRender), [typeof(Action)]));
    }

    [Fact]
    public void RegisterRenderShutdown_OpenVR終了前の解放APIを持つ()
    {
        Assert.NotNull(typeof(FloatSodaApp).GetMethod(
            nameof(FloatSodaApp.RegisterRenderShutdown), [typeof(Action)]));
    }
}
