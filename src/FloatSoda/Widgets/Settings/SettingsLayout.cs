using FloatSoda.Elements;
using FloatSoda.Geometrics;
using FloatSoda.Painting;
using FloatSoda.Widgets.Layout;

namespace FloatSoda.Widgets.Settings;

/// <summary>設定画面用の共通色。標準のSkiaSharp/BoxDecorationを使用します。</summary>
internal static class SettingsPalette
{
    internal static readonly Color Background = new(18, 24, 34);
    internal static readonly Color Section = new(30, 40, 54);
    internal static readonly Color Control = new(52, 66, 82);
    internal static readonly Color Accent = new(83, 181, 236);
    internal static readonly Color Text = new(242, 247, 252);
    internal static readonly Color SecondaryText = new(181, 197, 214);
}

/// <summary>SteamVRダッシュボード向けの設定ページを組み立てます。状態や永続化は所有しません。</summary>
/// <remarks>縦スクロールは未実装です。親ウィンドウが収まるサイズでセクションを配置してください。</remarks>
public sealed record SettingsPage : StatelessWidget
{
    /// <summary>画面内に表示する見出し。</summary>
    public required string Title { get; init; }

    /// <summary>表示するセクション。順序を維持します。</summary>
    public List<Widget> Sections { get; init; } = [];

    /// <inheritdoc/>
    public override Widget Build(IBuildContext context)
    {
        var children = new List<Widget>
        {
            new Text(Title)
            {
                Style = new TextStyle
                {
                    FontSize = 38,
                    FontWeight = 700,
                    Color = SettingsPalette.Text,
                },
            },
            new SizedBox { Height = 24 },
        };

        foreach (var section in Sections)
        {
            children.Add(section);
            children.Add(new SizedBox { Height = 20 });
        }

        return new Container
        {
            Color = SettingsPalette.Background,
            Padding = EdgeInsets.All(28),
            Child = new Column
            {
                MainAxisSize = MainAxisSize.Min,
                CrossAxisAlignment = CrossAxisAlignment.Stretch,
                Children = children,
            },
        };
    }
}

/// <summary>複数の設定項目を見出し付きでまとめます。</summary>
public sealed record SettingsSection : StatelessWidget
{
    /// <summary>カテゴリ名。</summary>
    public required string Title { get; init; }

    /// <summary>設定項目の一覧。</summary>
    public List<Widget> Children { get; init; } = [];

    /// <inheritdoc/>
    public override Widget Build(IBuildContext context)
    {
        var children = new List<Widget>
        {
            new Text(Title)
            {
                Style = new TextStyle
                {
                    FontSize = 28,
                    FontWeight = 700,
                    Color = SettingsPalette.Accent,
                },
            },
            new SizedBox { Height = 14 },
        };

        foreach (var child in Children)
        {
            children.Add(child);
            children.Add(new SizedBox { Height = 8 });
        }

        return new Container
        {
            Decoration = new BoxDecoration
            {
                Color = SettingsPalette.Section,
                BorderRadius = BorderRadius.Circular(12),
            },
            Padding = EdgeInsets.All(18),
            Child = new Column
            {
                CrossAxisAlignment = CrossAxisAlignment.Stretch,
                MainAxisSize = MainAxisSize.Min,
                Children = children,
            },
        };
    }
}

/// <summary>設定名・任意の説明・右側の操作ウィジェットを横並びにします。</summary>
public sealed record SettingsTile : StatelessWidget
{
    /// <summary>項目名。</summary>
    public required string Title { get; init; }

    /// <summary>項目の補足説明。</summary>
    public string? Description { get; init; }

    /// <summary>右側の操作。Switch / Slider / Dropdownなどを配置できます。</summary>
    public Widget? Trailing { get; init; }

    /// <inheritdoc/>
    public override Widget Build(IBuildContext context)
    {
        var labelChildren = new List<Widget>
        {
            new Text(Title)
            {
                Style = new TextStyle { FontSize = 25, Color = SettingsPalette.Text },
            },
        };
        if (!string.IsNullOrWhiteSpace(Description))
        {
            labelChildren.Add(new SizedBox { Height = 5 });
            labelChildren.Add(
                new Text(Description!)
                {
                    Style = new TextStyle { FontSize = 19, Color = SettingsPalette.SecondaryText },
                }
            );
        }

        var rowChildren = new List<Widget>
        {
            new Expanded
            {
                Child = new Column
                {
                    MainAxisSize = MainAxisSize.Min,
                    CrossAxisAlignment = CrossAxisAlignment.Start,
                    Children = labelChildren,
                },
            },
        };
        if (Trailing is not null)
        {
            rowChildren.Add(new SizedBox { Width = 20 });
            rowChildren.Add(Trailing);
        }

        return new Container
        {
            Padding = EdgeInsets.Symmetric(vertical: 12, horizontal: 8),
            Child = new Row
            {
                MainAxisSize = MainAxisSize.Max,
                CrossAxisAlignment = CrossAxisAlignment.Center,
                Children = rowChildren,
            },
        };
    }
}
