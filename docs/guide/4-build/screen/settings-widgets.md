# Settings UI Widgets

SteamVRダッシュボードの設定画面を、FloatSodaの既存Widgetだけで構築するためのコンポーネントです。SkiaSharp / Element / RenderObject / Layerは既存の描画パイプラインをそのまま使います。

## コンポーネント

| Widget | 用途 | 主なプロパティ |
| --- | --- | --- |
| `SettingsPage` | タイトルとセクションのルート | `Title`, `Sections` |
| `SettingsSection` | カテゴリ見出しと項目を配置 | `Title`, `Children` |
| `SettingsTile` | ラベル、説明、右側操作エリア | `Title`, `Description`, `Trailing` |
| `SettingsSwitch` | タップで真偽値を切り替え | `Value`, `OnChanged` |
| `SettingsSlider` | レーザーで数値範囲を変更 | `Min`, `Max`, `Value`, `OnChanged` |
| `SettingsDropdown` | 複数の文字列からひとつを選択 | `Value`, `Options`, `OnChanged` |

操作Widgetは**制御済み**です。値の正本はアプリケーションが保持します。操作時に`OnChanged`が通知されるため、`State<T>.SetState`で値を更新してください。`OnChanged = null`では表示のみです。WidgetにファイルI/Oや設定モデルへの依存はありません。

## StatefulWidgetの使用例

```csharp
using FloatSoda.Elements;
using FloatSoda.Widgets;
using FloatSoda.Widgets.Settings;
using FloatSoda.Widgets.Layout;

public sealed record Preferences : StatefulWidget<Preferences>
{
    public override State<Preferences> CreateState() => new PreferencesState();
}

public sealed class PreferencesState : State<Preferences>
{
    private bool _enabled = true;
    private double _level = 0.5;
    private string _mode = "Normal";

    public override Widget Build(IBuildContext context) => new SizedBox
    {
        Width = 900,
        Child = new SettingsPage
        {
            Title = "Preferences",
            Sections =
            {
                new SettingsSection
                {
                    Title = "Controls",
                    Children =
                    {
                        new SettingsTile
                        {
                            Title = "Enabled",
                            Description = "Allow controller interactions",
                            Trailing = new SettingsSwitch
                            {
                                Value = _enabled,
                                OnChanged = v => SetState(() => _enabled = v)
                            }
                        },
                        new SettingsTile
                        {
                            Title = "Level",
                            Trailing = new SettingsSlider
                            {
                                Min = 0,
                                Max = 1,
                                Value = _level,
                                OnChanged = v => SetState(() => _level = v)
                            }
                        },
                        new SettingsTile
                        {
                            Title = "Mode",
                            Trailing = new SettingsDropdown
                            {
                                Value = _mode,
                                Options = ["Normal", "Fast"],
                                OnChanged = v => SetState(() => _mode = v)
                            }
                        }
                    }
                }
            }
        }
    };
}
```

## DashboardWindowへの配置

```csharp
app.CreateWindow(new DashboardWindow
{
    Title = "Preferences",
    Child = new Preferences()
});
```

`AppKey`の設定、ホスト初期化、デスクトップ切替方法は`samples/FloatSoda.Samples.Settings/Program.cs`を参照してください。

## 範囲と入力

`SettingsSlider`は`Min < Max`を要求します。NaN/Infinityは例外、有限の範囲外`Value`は描画時にクランプします。トラックへのポインターDownまたは押下中のMoveを値へ変換します。レーザーとマウスは既存の`Listener`の入力経路を共有します。

`SettingsDropdown`は文字列の選択肢を受け取り、選択後に`OnChanged`で値を通知します。外部から`Value`が変化すると、新しい値が再表示されます。存在しない値は「未選択」と表示します。

## 制限事項と検証

- `SingleChildScrollView`、`ListView`、汎用Popupは現状未実装です。SettingsPageはスクロールせず、Dropdownの候補は下方向に展開します。
- ページが大きい場合はセクション数を減らすか、アプリ側でページを分割してください。ダッシュボードの最終的な表示範囲はVR環境にも依存します。
- Sliderトラックの論理幅は300pxで固定です。タイル内の長いラベルや狭い画面幅は目視確認が必要です。
- 実機のSteamVRコントローラーレーザーの挙動は、ハードウェア環境で別途検証してください。
- データ保存はWidgetに含めず、サンプルの`SettingsDemoState`が任意のJSONファイルへ保存します。
