namespace FloatSoda.Gesture;

/// <summary>ジェスチャ判定に使う既定のしきい値。Flutter の <c>gestures/constants.dart</c> 相当。</summary>
public static class GestureConstants
{
    /// <summary>タップとドラッグを区別する移動量の既定しきい値（論理ピクセル）。</summary>
    public const double TouchSlop = 18.0;

    /// <summary>
    /// ジェスチャ判定のスロップを上書きします。null なら <see cref="TouchSlop"/> を使います。
    /// VR レーザーポインターのように押下中に照準が数十ピクセル揺れる入力環境では、
    /// 18px の既定値はタップをほぼ確実にキャンセルするため、起動時に大きめの値（例: 80）を
    /// 設定してください。
    /// </summary>
    public static double? TouchSlopOverride { get; set; }

    /// <summary>認識器が使う実効スロップ。</summary>
    internal static double Slop => TouchSlopOverride ?? TouchSlop;
}
