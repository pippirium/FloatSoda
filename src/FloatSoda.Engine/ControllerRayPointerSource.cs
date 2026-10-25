using FloatSoda.Abstractions.Geometries;
using FloatSoda.Abstractions.Input;
using FloatSoda.OVR;
using FloatSoda.OVR.Overlay;
using SkiaSharp;

namespace FloatSoda.Engine;

/// <summary>
/// <see cref="IMovableOverlay"/>（デバイス追従・ワールド固定）に対して、指定デバイスのコントローラーレイを
/// 1フレームごとに交差させ、交差 UV とトリガー状態を <see cref="RawPointerEvent"/> へ変換する入力源。
/// </summary>
/// <remarks>
/// <para>
/// <c>VROverlayInputMethod.Mouse</c> で受けるダッシュボード経路と異なり
/// <see cref="VROverlayFlags.MakeOverlaysInteractiveIfVisible"/> を設定しないため、
/// SteamVR による入力横取り（レーザーポインターの全域ブロック）が発生しません。
/// </para>
/// <para>
/// 複数デバイスが指定された場合はリストの先頭から交差を確認し、最初にヒットしたデバイスの座標を採用します。
/// 未ヒットのデバイスも毎フレームポールします（トリガー押下遷移の <see cref="ControllerPointerSystem"/> 内部
/// 状態を鮮明に保つため）。
/// </para>
/// <para>
/// <see cref="Pump"/> を呼んだスレッドでイベントが発火します（オーバーレイポンプ元のスレッドを想定）。
/// UI スレッドへのマーシャリングは購読側の責務です。
/// </para>
/// </remarks>
public sealed class ControllerRayPointerSource : IRawPointerSource
{
    private readonly Func<SKSizeI> _tileSize;
    private readonly IReadOnlyList<Func<PointerData?>> _pollers;

    private int _activePoller = -1;
    private Offset _lastPosition;

    /// <inheritdoc />
    public event Action<RawPointerEvent>? OnPointerEvent;

    /// <summary>
    /// オーバーレイの交差サービスから入力源を生成します。既定では両手コントローラー（左・右の順）をポールします。
    /// </summary>
    /// <param name="intersection">交差判定を行う対象オーバーレイのサービス。</param>
    /// <param name="tileSize">交差 UV をピクセル座標へ換算するための、描画先テクスチャの現在サイズを返す処理。</param>
    /// <param name="devices">ポールするトラッキングデバイス。省略時は両手コントローラーです。</param>
    public ControllerRayPointerSource(OverlayIntersection intersection, Func<SKSizeI> tileSize,
        IReadOnlyList<TrackedDevice>? devices = null)
    {
        devices ??= [TrackedDevice.LeftController, TrackedDevice.RightController];
        _tileSize = tileSize;

        var pollers = new List<Func<PointerData?>>(devices.Count);
        foreach (var device in devices)
        {
            var system = new ControllerPointerSystem(intersection);
            pollers.Add(() => system.Poll(device));
        }

        _pollers = pollers;
    }

    /// <summary>
    /// 交差ポール関数を直接指定するコンストラクター。テストおよび OpenVR 以外での再利用を想定します。
    /// </summary>
    /// <param name="tileSize">交差 UV をピクセル座標へ換算するための、描画先テクスチャの現在サイズを返す処理。</param>
    /// <param name="pollers">1台ぶんの交差ポール関数のリスト。未ヒットのときは<see langword="null"/>を返します。</param>
    public ControllerRayPointerSource(Func<SKSizeI> tileSize, IReadOnlyList<Func<PointerData?>> pollers)
    {
        _tileSize = tileSize;
        _pollers = pollers;
    }

    /// <summary>
    /// 1フレーム分の交差ポールを実行し、ヒット状態の変化に沿って <see cref="RawPointerEvent"/> を発火します。
    /// </summary>
    /// <remarks>
    /// <see cref="Engine.OverlayWindow"/> からは入力を持つ movable オーバーレイに対して毎フレーム呼ばれます。
    /// ヒットから外れたフレームでは <see cref="RawPointerKind.Leave"/> のみを合成します
    /// （ダッシュボードの VREvent_FocusLeave と同一挙動。押下中の Up / Cancel は合成されません）。
    /// </remarks>
    public void Pump()
    {
        var hitPoller = -1;
        PointerData? hit = null;

        for (var index = 0; index < _pollers.Count; index++)
        {
            var result = _pollers[index]();

            if (result is null || hitPoller >= 0) continue;

            hitPoller = index;
            hit = result;
        }

        if (hit is not { } hitData)
        {
            if (_activePoller < 0) return;

            OnPointerEvent?.Invoke(new RawPointerEvent(RawPointerKind.Leave, _lastPosition));
            _activePoller = -1;
            return;
        }

        var tile = _tileSize();
        var position = ToLocalPosition(hitData, tile);

        if (_activePoller != hitPoller)
        {
            if (_activePoller >= 0)
            {
                OnPointerEvent?.Invoke(new RawPointerEvent(RawPointerKind.Leave, _lastPosition));
            }

            OnPointerEvent?.Invoke(new RawPointerEvent(RawPointerKind.Enter, position));
            _activePoller = hitPoller;
        }

        _lastPosition = position;

        OnPointerEvent?.Invoke(new RawPointerEvent(RawPointerKind.Move, position));

        switch (hitData.Change)
        {
            case PointerChange.Down:
                OnPointerEvent?.Invoke(new RawPointerEvent(RawPointerKind.ButtonDown, position));
                break;
            case PointerChange.Up:
                OnPointerEvent?.Invoke(new RawPointerEvent(RawPointerKind.ButtonUp, position));
                break;
        }
    }

    /// <summary>
    /// 交差 UV を描画先ローカルのピクセル座標（原点左上）へ変換します。
    /// Dashboard 経路（<see cref="OverlayMouseRawPointerSource"/> 参照）と同様に、GL テクスチャの
    /// ボトムアップ格納と SteamVR 表示時の v 軸反転が相殺されるため、UV は反転せずテクスチャサイズで
    /// 拡大するだけとします。
    /// </summary>
    private static Offset ToLocalPosition(in PointerData data, in SKSizeI tile) =>
        new(data.X * tile.Width, data.Y * tile.Height);

    /// <summary>アクティブなポーラーの記憶を破棄します。次の <see cref="Pump"/> からはヒットを新規 Enter として扱います。</summary>
    public void Dispose()
    {
        _activePoller = -1;
        _lastPosition = default;
        OnPointerEvent = null;
    }
}
