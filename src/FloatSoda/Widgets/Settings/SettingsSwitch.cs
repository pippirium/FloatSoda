using FloatSoda.Elements;
using FloatSoda.Geometrics;
using FloatSoda.Gesture;
using FloatSoda.Painting;
using FloatSoda.Widgets.Gesture;
using FloatSoda.Widgets.Layout;

namespace FloatSoda.Widgets.Settings;

/// <summary>アプリが所有するbool値をタップで変更する制御済みの設定スイッチです。</summary>
/// <remarks>OnChanged内でアプリのStateを更新し、更新済みのValueを渡して再構築してください。</remarks>
public sealed record SettingsSwitch : StatelessWidget
{
    /// <summary>現在のON/OFF状態。</summary>
    public bool Value { get; init; }

    /// <summary>ユーザー操作による新しい値を通知します。nullでは読み取り専用です。</summary>
    public Action<bool>? OnChanged { get; init; }

    /// <inheritdoc/>
    public override Widget Build(IBuildContext context)
    {
        var track = new SizedBox
        {
            Width = 112,
            Height = 60,
            Child = new Container
            {
                Decoration = new BoxDecoration
                {
                    Color = Value ? SettingsPalette.Accent : SettingsPalette.Control,
                    BorderRadius = BorderRadius.Circular(30)
                },
                Padding = EdgeInsets.All(8),
                Child = new Align
                {
                    Alignment = Value ? Alignment.CenterRight : Alignment.CenterLeft,
                    Child = new Container
                    {
                        Width = 44,
                        Height = 44,
                        Decoration = new BoxDecoration
                        {
                            Color = SettingsPalette.Text,
                            BorderRadius = BorderRadius.Circular(22)
                        }
                    }
                }
            }
        };
        return new GestureDetector
        {
            Behaviour = HitTestBehaviour.Opaque,
            OnTap = OnChanged is null ? null : () => OnChanged(!Value),
            Child = track
        };
    }
}
