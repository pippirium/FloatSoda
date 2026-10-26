# Settings Widgets サンプル

`DashboardWindow` で再利用可能な設定画面を表示します。SteamVRのダッシュボードタブでレーザー入力を使用できます。

```bash
dotnet run --project samples/FloatSoda.Samples.Settings
dotnet run --project samples/FloatSoda.Samples.Settings -- --desktop
```

`--desktop` は表示先のみを変更します。どちらも SteamVR の起動と HMD 接続が必要です。

## 操作

- Switch: トリガーでON/OFFを切り替え
- Slider: トラック上を押すとその位置の値を選択し、押したまま動かすと値を更新
- Dropdown: 押して候補を展開し、候補を押して選択

`SettingsDemoState` が値を保持し、`SetState` で再構築します。保存先はユーザーのローカルアプリケーションデータ配下の `FloatSoda/settings-ui-sample.json` です。

画面は内容の高さに追従しますが、縦スクロールとオーバーレイ型ポップアップはありません。実機によるVRレーザー操作確認は別途必要です。
