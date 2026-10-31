using FloatSoda.Core;
using FloatSoda.Elements;
using FloatSoda.RenderObjects;
using FloatSoda.Widgets;
using FloatSoda.Widgets.Layout;
using SkiaSharp;

namespace FloatSoda.Test.Widgets;

public class WindowWidgetTest
{
    /// <summary>Build時に祖先のWindowWidgetを探索して記録するプローブ。</summary>
    private record Probe : StatelessWidget
    {
        public required Action<IBuildContext> OnBuild { get; init; }

        public override Widget Build(IBuildContext context)
        {
            OnBuild(context);
            return new SizedBox { Width = 10, Height = 10 };
        }
    }

    private static (RenderPipeline Pipeline, RenderView RenderView) MountTree(Widget widget) =>
        MountTree(widget, new BuildOwner(() => { }));

    private static (RenderPipeline Pipeline, RenderView RenderView) MountTree(
        Widget widget,
        BuildOwner owner
    )
    {
        var renderView = new RenderView();
        var pipeline = new RenderPipeline
        {
            OnNeedVisualUpdate = () => { },
            RenderView = renderView,
        };
        renderView.PrepareInitialFrame();

        new RenderObjectToWidgetAdapter
        {
            Container = renderView,
            Child = widget,
        }.AttachToRenderTree(owner, null);

        return (pipeline, renderView);
    }

    [Fact]
    public void Of_FindsConcreteWindow_ViaBaseTypeLookup()
    {
        WindowWidget? found = null;

        MountTree(
            new DashboardWindow
            {
                Title = "TestWindow",
                Child = new Probe { OnBuild = ctx => found = WindowWidget.Of(ctx) },
            }
        );

        // ScopeType により具象型（DashboardWindow）でも基底型 WindowWidget で lookup できる
        Assert.IsType<DashboardWindow>(found);
        Assert.Equal("TestWindow", found!.Title);
    }

    [Fact]
    public void OverlayWindowOf_FindsConcreteOverlayWindow()
    {
        OverlayWindow? found = null;

        MountTree(
            new DashboardWindow
            {
                Title = "TestWindow",
                Child = new Probe { OnBuild = ctx => found = OverlayWindow.Of(ctx) },
            }
        );

        // オーバーレイ系の中間基底 OverlayWindow でも lookup できる（Swap 拡張の前提）
        Assert.IsType<DashboardWindow>(found);
    }

    [Fact]
    public void DesktopWindowOf_FindsConcreteDesktopWindow()
    {
        DesktopWindow? found = null;

        MountTree(
            new DesktopWindow
            {
                Title = "TestWindow",
                Child = new Probe { OnBuild = ctx => found = DesktopWindow.Of(ctx) },
            }
        );

        Assert.IsType<DesktopWindow>(found);
        Assert.Equal("TestWindow", found!.Title);
    }

    [Fact]
    public void OverlayWindowOf_ThrowsForDesktopWindow()
    {
        MountTree(
            new DesktopWindow
            {
                Title = "TestWindow",
                Child = new Probe
                {
                    OnBuild = ctx =>
                        Assert.Throws<InvalidOperationException>(() => OverlayWindow.Of(ctx)),
                },
            }
        );
    }

    [Fact]
    public void SizeUnset_RenderViewShrinkWrapsToChild()
    {
        var (pipeline, renderView) = MountTree(
            new DashboardWindow
            {
                Title = "TestWindow",
                Child = new SizedBox { Width = 320, Height = 240 },
            }
        );

        pipeline.FlushLayout();

        Assert.Null(renderView.FixedSize);
        Assert.Equal(new SKSize(320, 240), renderView.Size);
    }

    [Fact]
    public void SizeSet_RenderViewUsesFixedSize()
    {
        var (pipeline, renderView) = MountTree(
            new DashboardWindow
            {
                Title = "TestWindow",
                Size = new SKSize(800, 600),
                Child = new SizedBox { Width = 320, Height = 240 },
            }
        );

        pipeline.FlushLayout();

        Assert.Equal(new SKSize(800, 600), renderView.FixedSize);
        Assert.Equal(new SKSize(800, 600), renderView.Size);
    }

    /// <summary>
    /// ウィンドウ宣言を後から作り替える要求(実測への追従)を検証する子孫。
    /// </summary>
    private record SizeResizer : StatefulWidget<SizeResizer>
    {
        /// <summary>宣言へ作り替えたい寸法(実測に相当する)。</summary>
        public required SKSize Size { get; init; }

        /// <summary>直近のビルドで読み出した宣言。</summary>
        internal WindowElement? WindowElement;

        public override State<SizeResizer> CreateState() => new SizeResizerState();

        private class SizeResizerState : State<SizeResizer>
        {
            /// <summary>
            /// 宣言の作り直しの通知を、その場の再構築にせずあとの再構築待ちへの予約として受け取る。
            /// 実測へ追従するアプリはこの予約を次のフレームで処理するため、予約された再構築が
            /// 新しい <see cref="RenderView.FixedSize"/> と組む必要がある。
            /// </summary>
            public override void DidChangeDependencies() => Element!.MarkNeedsBuild();

            public override Widget Build(IBuildContext context)
            {
                // 宣言に依存として登録する。作り直された宣言を読み直すために必要。
                context.DependOnInheritedWidgetOfExactType<WindowWidget>();
                Widget!.WindowElement =
                    Context.GetElementForInheritedWidgetOfExactType<WindowWidget>()
                    as WindowElement;
                return new SizedBox { Width = Widget!.Size.Width, Height = Widget!.Size.Height };
            }
        }
    }

    /// <summary>
    /// オーバーレイの寸法を決めるウィンドウ宣言(<see cref="WindowWidget.Size"/>)を、宣言より深い位置から
    /// 後発で作り替えたとき、その寸法がレイアウト結果（＝オーバーレイサイズ）に反映されることを検証する。
    /// </summary>
    /// <remarks>
    /// ScreenView はキャプチャ画像の実測に合わせて宣言を作り替える。宣言の作り直しは依存 Element の
    /// 再構築を予約するだけなので、次のフレーム(<see cref="BuildOwner.BuildScope"/> →
    /// <see cref="RenderPipeline.FlushLayout"/>)が新しい <see cref="RenderView.FixedSize"/> と組まないと、
    /// レイアウト結果は古い宣言の寸法に張り付く。
    /// </remarks>
    [Fact]
    public void Update_WindowWidgetSizeChangedByDescendant_LayoutUsesNewSize()
    {
        var owner = new BuildOwner(() => { });
        var resizer = new SizeResizer { Size = new SKSize(640, 360) };
        var (pipeline, renderView) = MountTree(
            new DashboardWindow
            {
                Title = "TestWindow",
                Size = new SKSize(800, 600),
                Child = resizer,
            },
            owner
        );

        pipeline.FlushLayout();

        // 初回は宣言どおりの寸法でレイアウトされていること。
        Assert.Equal(new SKSize(800, 600), renderView.Size);

        // キャプチャ画像の実測を受けた要求として、宣言を作り直す。
        // この時点では依存 Element の再構築が予約されるだけで、レイアウトは走らない。
        var windowElement = resizer.WindowElement!;
        windowElement.Update(
            (WindowWidget)windowElement.Widget with
            {
                Size = new SKSize(640, 360),
            }
        );
        Assert.Equal(new SKSize(640, 360), renderView.FixedSize);

        // 次のフレームと同じ手順(再構築 -> レイアウト)で処理する。
        owner.BuildScope();
        pipeline.FlushLayout();

        // 宣言の作り直しがレイアウト結果（＝オーバーレイサイズ）へ反映されていること。
        Assert.Equal(new SKSize(640, 360), renderView.Size);
    }
}
