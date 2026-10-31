using System.Text;
using FloatSoda.OVR.Exceptions;

namespace FloatSoda.OVR.Overlay;

/// <summary>
/// OpenVR のオーバーレイ用仮想キーボード(SteamVR OSK)を操作する薄いラッパー。
/// </summary>
/// <remarks>
/// SteamVR の OSK は1アプリにつき1文字入力面のみです。他で使用中のときは
/// <see cref="TryShowForOverlay"/> が <see langword="false"/> を返します。
/// 入力確定は VREvent_KeyboardDone で届くため、チャネリングしたイベントディスパッチャーで
/// 受信し、その後 <see cref="GetText"/> で全文を取得してください。
/// </remarks>
public static class OverlayKeyboard
{
    /// <summary>入力モード: 1行のみ。OpenVR の EKeyboardInputMode.SingleLine 相当。</summary>
    public const int InputModeSingleLine = 1;

    /// <summary>入力モード: 複数行。OpenVR の EKeyboardInputMode.MultiLine 相当。</summary>
    public const int InputModeMultiLine = 2;

    /// <summary>指定したオーバーレイに紐づけて OSK を表示します。</summary>
    /// <param name="overlayHandle">紐づけるオーバーレイのハンドル。OSK はこのオーバーレイの矩形を避けて表示されます。</param>
    /// <param name="description">OSK に表示する用途の説明。</param>
    /// <param name="existingText">あらかじめ入力欄に置いておくテキスト。省略時は空。</param>
    /// <param name="flags">表示フラグ(<see cref="EKeyboardFlags"/>)。</param>
    /// <param name="inputMode">入力モード。既定は <see cref="InputModeSingleLine"/>。</param>
    /// <param name="maxChars">入力可能な最大文字数。</param>
    /// <param name="userValue">VREvent にそのまま返されるユーザー値。</param>
    /// <returns>表示に成功した場合 <see langword="true"/>。SteamVR の OSK が他で使用中の場合 <see langword="false"/>。</returns>
    public static bool TryShowForOverlay(
        ulong overlayHandle,
        string description,
        string? existingText = null,
        uint flags = (uint)EKeyboardFlags.KeyboardFlag_Modal,
        int inputMode = InputModeSingleLine,
        uint maxChars = 256,
        ulong userValue = 0
    )
    {
        var error = OpenVR.Overlay.ShowKeyboardForOverlay(
            overlayHandle,
            inputMode,
            0,
            flags,
            description,
            maxChars,
            existingText ?? "",
            userValue
        );

        switch (error)
        {
            case EVROverlayError.None:
                return true;
            case EVROverlayError.KeyboardAlreadyInUse:
                return false;
            default:
                error.ThrowIfError();
                return false;
        }
    }

    /// <summary>表示中の OSK を閉じます。</summary>
    public static void Hide() => OpenVR.Overlay.HideKeyboard();

    /// <summary>これまでに入力されたテキスト全文を取得します。</summary>
    /// <param name="maxLength">取得する最大文字数。VREvent_KeyboardDone 受信後の呼び出しを想定します。</param>
    public static string GetText(uint maxLength = 256)
    {
        var builder = new StringBuilder(checked((int)maxLength));
        var count = OpenVR.Overlay.GetKeyboardText(builder, maxLength);
        return count > 0 ? builder.ToString(0, checked((int)count)) : "";
    }
}
