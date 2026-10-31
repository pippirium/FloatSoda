using FloatSoda.Abstractions.Geometries;
using FloatSoda.Abstractions.Input;
using FloatSoda.OVR;

namespace FloatSoda.Engine;

/// <summary>
/// OpenVR オーバーレイのマウスイベント（ダッシュボードのレーザーポインタ）を
/// <see cref="RawPointerEvent"/> に変換する入力源。
/// 事前に <c>SetOverlayInputMethod(Mouse)</c> と <c>SetOverlayMouseScale</c> が
/// 設定されている前提です。公開コンストラクターは入力座標をそのまま渡します。
/// エンジンのダッシュボード経路は0〜1の入力範囲をUI座標へ変換します。
/// OpenVR の仕様上マウス座標は原点左下だが、本プロジェクトの OpenGL テクスチャ
/// （ボトムアップ格納）では SteamVR の表示時 v 軸反転と相殺され原点左上で届くことを
/// 実測で確認済みのため、座標はそのまま使用します（y 反転すると上下鏡映になる）。
/// イベントは <see cref="VREventDispatcher.PollEvents"/> を呼んだスレッドで発火します。
/// </summary>
public sealed class OverlayMouseRawPointerSource : IRawPointerSource
{
    private readonly VREventDispatcher _dispatcher;
    private Offset _currentPosition;
    private readonly Func<Offset, Offset> _toLocalPosition;

    /// <inheritdoc />
    public event Action<RawPointerEvent>? OnPointerEvent;

    /// <summary>
    /// 指定したイベントディスパッチャへ OpenVR のマウスイベントハンドラーを登録します。
    /// </summary>
    /// <param name="dispatcher">対象オーバーレイのイベントディスパッチャ。</param>
    public OverlayMouseRawPointerSource(VREventDispatcher dispatcher)
        : this(dispatcher, static position => position) { }

    internal OverlayMouseRawPointerSource(
        VREventDispatcher dispatcher,
        Func<Offset, Offset> toLocalPosition
    )
    {
        _dispatcher = dispatcher;
        _toLocalPosition = toLocalPosition;

        _dispatcher.Register(EVREventType.VREvent_FocusEnter, OnFocusEnter);
        _dispatcher.Register(EVREventType.VREvent_FocusLeave, OnFocusLeave);
        _dispatcher.Register(EVREventType.VREvent_MouseMove, OnMouseMove);
        _dispatcher.Register(EVREventType.VREvent_MouseButtonDown, OnMouseButtonDown);
        _dispatcher.Register(EVREventType.VREvent_MouseButtonUp, OnMouseButtonUp);
    }

    private void OnFocusEnter(in VREvent_t vrEvent) =>
        OnPointerEvent?.Invoke(new RawPointerEvent(RawPointerKind.Enter, _currentPosition));

    private void OnFocusLeave(in VREvent_t vrEvent) =>
        OnPointerEvent?.Invoke(new RawPointerEvent(RawPointerKind.Leave, _currentPosition));

    private void OnMouseMove(in VREvent_t vrEvent)
    {
        var position = ToLocalPosition(vrEvent.data.mouse);

        _currentPosition = position;
        OnPointerEvent?.Invoke(new RawPointerEvent(RawPointerKind.Move, position));
    }

    private void OnMouseButtonDown(in VREvent_t vrEvent) =>
        OnMouseButton(vrEvent.data.mouse, RawPointerKind.ButtonDown);

    private void OnMouseButtonUp(in VREvent_t vrEvent) =>
        OnMouseButton(vrEvent.data.mouse, RawPointerKind.ButtonUp);

    private void OnMouseButton(in VREvent_Mouse_t mouse, RawPointerKind kind)
    {
        if (TryMapButton((EVRMouseButton)mouse.button) is not { } button)
            return;

        var position = ToLocalPosition(mouse);

        _currentPosition = position;
        OnPointerEvent?.Invoke(new RawPointerEvent(kind, position, button));
    }

    /// <summary>
    /// マウススケール座標を描画先ローカル座標へ変換します。
    /// Y軸の向きは変えず、入力範囲からUI座標への変換のみ適用します。
    /// </summary>
    private Offset ToLocalPosition(in VREvent_Mouse_t mouse) =>
        _toLocalPosition(new Offset(mouse.x, mouse.y));

    private static PointerButton? TryMapButton(EVRMouseButton button) =>
        button switch
        {
            EVRMouseButton.Left => PointerButton.Left,
            EVRMouseButton.Middle => PointerButton.Middle,
            EVRMouseButton.Right => PointerButton.Right,
            _ => null,
        };

    /// <summary>
    /// イベントディスパッチャから登録済みの OpenVR マウスイベントハンドラーを解除します。
    /// </summary>
    public void Dispose()
    {
        _dispatcher.Unregister(EVREventType.VREvent_FocusEnter, OnFocusEnter);
        _dispatcher.Unregister(EVREventType.VREvent_FocusLeave, OnFocusLeave);
        _dispatcher.Unregister(EVREventType.VREvent_MouseMove, OnMouseMove);
        _dispatcher.Unregister(EVREventType.VREvent_MouseButtonDown, OnMouseButtonDown);
        _dispatcher.Unregister(EVREventType.VREvent_MouseButtonUp, OnMouseButtonUp);
    }
}
