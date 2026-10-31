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

論理キャンバスは960×1000で固定し、Dropdownの開閉でも入力座標の範囲は変わりません。
SteamVR側のパネル拡大・縮小は物理的な表示倍率を変更します。
このサンプルの「UI scale」は値の通知と保存を示す項目で、画面自体の倍率は変更しません。
縦スクロールとオーバーレイ型ポップアップはありません。

## exeで起動する

リポジトリのルートで次を実行します。.NET 10 Runtimeが必要です。

```powershell
./scripts/Publish-SettingsSample.ps1
& "$env:LOCALAPPDATA/FloatSoda/Samples/Settings/FloatSoda.Samples.Settings.exe"
```

発行先のexeはエクスプローラーからも起動できます。終了はコンソールのCtrl+Cを使用します。

実機テスト環境では、保護ACLのある作業フォルダーのexeは
`Init_HmdNotFoundPresenceFailed`で終了し、バイナリが一致する発行先のexeは起動しました。
そのため、exeの確認には上記の発行先を使用します。作業フォルダーのACLは変更しません。
配置条件による失敗の内部原因まで確定したものではありません。

作業フォルダーで開発中は、同じビルドを
`dotnet samples/FloatSoda.Samples.Settings/bin/Release/net10.0/FloatSoda.Samples.Settings.dll --Logging:EventLog:LogLevel:Default=None`
で起動する方法もあります。後ろの引数はWindowsイベントログへの出力をこの起動だけ無効にします。
