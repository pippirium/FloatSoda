using FloatSoda.Elements;
using FloatSoda.Geometrics;
using FloatSoda.Gesture;
using FloatSoda.Painting;
using FloatSoda.Widgets.Gesture;
using FloatSoda.Widgets.Layout;

namespace FloatSoda.Widgets.Settings;

/// <summary>選択肢を画面内に展開する制御済みドロップダウンです。</summary>
/// <remarks>Popup/Overlayメニューが未実装のため、リストは後続の要素を下へ押し下げます。</remarks>
public sealed record SettingsDropdown : StatefulWidget<SettingsDropdown>
{
    /// <summary>現在の選択値。</summary>
    public required string Value { get; init; }

    /// <summary>選択可能な値の一覧。重複しない値を渡してください。</summary>
    public IReadOnlyList<string> Options { get; init; } = [];

    /// <summary>選択が変更されたときの通知先。nullの場合は読み取り専用です。</summary>
    public Action<string>? OnChanged { get; init; }

    /// <inheritdoc/>
    public override State<SettingsDropdown> CreateState() => new SettingsDropdownState();
}

/// <summary>選択候補の開閉状態のみ保持します。選択値は常に親が保持します。</summary>
public sealed class SettingsDropdownState : State<SettingsDropdown>
{
    private bool _open;

    /// <inheritdoc/>
    public override Widget Build(IBuildContext context)
    {
        var model = Widget!;
        var label = model.Options.Contains(model.Value) ? model.Value : "未選択";
        var children = new List<Widget>
        {
            Button(label + (_open ? " ▲" : " ▼"), model.OnChanged is null || model.Options.Count == 0
                ? null
                : () => SetState(() => _open = !_open))
        };

        if (_open && model.OnChanged is not null)
        {
            foreach (var option in model.Options)
            {
                var choice = option;
                children.Add(new SizedBox { Height = 6 });
                children.Add(Button(choice, () =>
                {
                    SetState(() => _open = false);
                    if (choice != Widget!.Value) Widget.OnChanged?.Invoke(choice);
                }));
            }
        }

        return new SizedBox
        {
            Width = 228,
            Child = new Column
            {
                MainAxisSize = MainAxisSize.Min,
                CrossAxisAlignment = CrossAxisAlignment.Stretch,
                Children = children
            }
        };
    }

    private static Widget Button(string text, Action? action) => new GestureDetector
    {
        Behaviour = HitTestBehaviour.Opaque,
        OnTap = action,
        Child = new Container
        {
            Decoration = new BoxDecoration
            {
                Color = SettingsPalette.Control,
                BorderRadius = BorderRadius.Circular(8)
            },
            Child = new SizedBox
            {
                Height = 56,
                Child = new Center
                {
                    Child = new Text(text)
                    {
                        Style = new TextStyle { FontSize = 22, Color = SettingsPalette.Text }
                    }
                }
            }
        }
    };
}
