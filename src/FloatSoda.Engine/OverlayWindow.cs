using FloatSoda.Abstractions.Engine;
using FloatSoda.Abstractions.Geometries;
using FloatSoda.Abstractions.Input;
using FloatSoda.OVR;
using FloatSoda.OVR.Overlay;
using FloatSoda.Rendering.Layers;
using SkiaSharp;

namespace FloatSoda.Engine;

/// <summary>SteamVRオーバーレイを描画先とする<see cref="IEngineWindow"/>の実装です。</summary>
/// <seealso cref="IOverlay"/>
public class OverlayWindow : IEngineWindow
{
    private readonly Renderer renderer;
    private readonly Dpm dpm;

    /// <summary>このウィンドウが描画先とするOpenVRオーバーレイを取得します。</summary>
    public IOverlay Overlay { get; }

    /// <summary>既定のコントローラーを使用するオーバーレイウィンドウを作成します。</summary>
    /// <param name="overlay">描画先となるOpenVRオーバーレイ。</param>
    /// <param name="renderer">レイヤーツリーを描画するレンダラー。</param>
    /// <param name="dpm">オーバーレイの物理幅へ換算する密度。</param>
    public OverlayWindow(IOverlay overlay, Renderer renderer, Dpm dpm)
        : this(overlay, renderer, dpm, null) { }

    /// <summary>指定されたオーバーレイを描画先とするウィンドウを作成します。</summary>
    /// <param name="overlay">描画先となるOpenVRオーバーレイ。</param>
    /// <param name="renderer">レイヤーツリーをテクスチャへ描画するレンダラー。</param>
    /// <param name="dpm">ピクセルサイズをオーバーレイの物理幅へ換算する際に使用する密度。</param>
    /// <param name="pointerDevices">movableオーバーレイでコントローラーレイをポールするデバイスのリスト。
    /// 省略時は両手コントローラーの順です。</param>
    /// <remarks>
    /// <paramref name="overlay"/>がダッシュボードオーバーレイの場合、入力方式をマウスへ設定し、
    /// SteamVRのレーザーポインター入力を受け取る<see cref="PointerSource"/>を生成します。
    /// デバイス追従・ワールド固定（movable）オーバーレイの場合は、コントローラーレイを毎フレーム
    /// 交差させる<see cref="ControllerRayPointerSource"/>を生成します（入力横取りフラグは使用しません）。
    /// どちらでもないオーバーレイでは<see cref="PointerSource"/>は<see langword="null"/>のままです。
    /// </remarks>
    public OverlayWindow(
        IOverlay overlay,
        Renderer renderer,
        Dpm dpm,
        IReadOnlyList<TrackedDevice>? pointerDevices = null
    )
    {
        Overlay = overlay;
        this.renderer = renderer;
        this.dpm = dpm;

        // ダッシュボードはSteamVRのレーザーがマウスイベントとして届く。
        if (overlay is IDashboardOverlay)
        {
            overlay.Input.Method = VROverlayInputMethod.Mouse;
            // 物理的なパネル倍率とUIのピクセル座標を分離する。
            overlay.Input.SetMouseScale(1, 1);
            PointerSource = new OverlayMouseRawPointerSource(
                overlay.EventDispatcher,
                position => new FloatSoda.Abstractions.Geometries.Offset(
                    position.X * renderer.GLView.Size.Width,
                    position.Y * renderer.GLView.Size.Height
                )
            );
        }
        // デバイス追従・ワールド固定はコントローラーレイを毎フレーム交差させて届く。
        // MakeOverlaysInteractiveIfVisible を使わないため入力横取りは発生しない。
        else if (overlay is IMovableOverlay movable)
        {
            PointerSource = new ControllerRayPointerSource(
                movable.Intersection,
                () =>
                    new SKSizeI((int)renderer.GLView.Size.Width, (int)renderer.GLView.Size.Height),
                pointerDevices
            );
        }
    }

    /// <summary>
    /// 指定されたレイヤーツリーを描画し、結果のテクスチャをオーバーレイへ反映します。
    /// </summary>
    /// <param name="layer">描画するレイヤーツリー。<see langword="null"/>は指定できません。</param>
    /// <remarks>レンダースレッド上で呼び出します。</remarks>
    public void Present(ILayer layer)
    {
        renderer.Render(layer);

        var texture = new Texture_t
        {
            handle = renderer.GetTextureHandle(),
            eType = ETextureType.OpenGL,
            eColorSpace = EColorSpace.Auto,
        };

        Overlay.Texture.FromTexture_t(texture);
    }

    /// <summary>
    /// 描画先テクスチャをリサイズし、ピクセル幅と物理密度（dots per meter）から
    /// オーバーレイの物理幅（WidthInMeters = width / dpm）を更新します。
    /// ダッシュボードのマウススケールは0〜1を維持し、入力時に描画先サイズへ変換します。
    /// </summary>
    public void Resize(SkiaSharp.SKSizeI size)
    {
        renderer.Resize(size.Width, size.Height);
        Overlay.WidthInMeters.Value = dpm.ToMeters(size.Width);

        if (PointerSource is OverlayMouseRawPointerSource)
        {
            Overlay.Input.SetMouseScale(1, 1);
        }
    }

    /// <summary>このウィンドウの生ポインター入力源を取得します。</summary>
    /// <value>ダッシュボードオーバーレイの場合はSteamVRレーザー入力源、デバイス追従・ワールド固定
    /// オーバーレイの場合はコントローラーレイ入力源。それ以外では<see langword="null"/>です。</value>
    public IRawPointerSource? PointerSource { get; }

    /// <summary>
    /// このオーバーレイ宛のOpenVRイベントをポーリングし、<see cref="PointerSource"/> へ流します。
    /// メインスレッドから毎フレーム呼びます。入力を持たないオーバーレイでは何もしません。
    /// </summary>
    public void PollInputEvents()
    {
        if (PointerSource is null)
            return;

        Overlay.EventDispatcher.PollEvents();

        // コントローラーレイ経路はイベント駆動でなくポーリングなので、ここで1フレーム分を追加で実行する。
        if (PointerSource is ControllerRayPointerSource controllerRay)
        {
            controllerRay.Pump();
        }
    }

    /// <summary>
    /// オーバーレイをSteamVR上に残したまま、このウィンドウが保持する入力とOpenGLのリソースだけを解放します。
    /// </summary>
    /// <remarks>
    /// 最後のフレームをオーバーレイへ転送した後に呼ぶと、表示内容は最後に転送されたフレームのまま残ります。
    /// 破棄を免れたオーバーレイの所有はOpenVRコンテキストへ移り、プロセスが <c>VREvent_Quit</c> を受けて
    /// 終了するときやSteamVRが終了したときにSteamVRにより破棄されます。
    /// 以降このインスタンスへの <see cref="Present"/>、<see cref="Resize"/>、
    /// <see cref="PollInputEvents"/> は呼び出さないでください。
    /// </remarks>
    public void AbandonOverlay()
    {
        try
        {
            PointerSource?.Dispose();
        }
        finally
        {
            try
            {
                Overlay.Identity.Detach();
            }
            finally
            {
                renderer.Dispose();
            }
        }
    }

    /// <summary>このウィンドウの生ポインター入力源とオーバーレイを破棄します。</summary>
    public void Dispose()
    {
        try
        {
            PointerSource?.Dispose();
        }
        finally
        {
            try
            {
                Overlay.Dispose();
            }
            finally
            {
                renderer.Dispose();
            }
        }
    }
}
