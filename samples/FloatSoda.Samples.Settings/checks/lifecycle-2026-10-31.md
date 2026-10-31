# 起動・正常終了の自動計測

## 正常終了

PR #6の`5b95726`を参照する診断Hostを実際のSteamVRへ接続し、
`IHostApplicationLifetime.StopApplication()`で停止した。強制停止は使用していない。

診断プログラム側で既存の`BeforeGLFWTerminate`コールバックを包み、
本来の解放処理の直後、OpenVRとGLFWの解放前に以下を確認した。

| 計測 | 結果 |
| --- | --- |
| 停止前のオーバーレイ検索 | `None`（存在） |
| リソース解放後の検索 | `UnknownOverlay`（破棄済み） |
| Skiaサーフェス | Handleが0（解放済み） |
| 現在のGLコンテキスト | なし |
| Host終了 | 完了、プロセス終了コード0 |

これはHMD内の目視判定とは別の自動計測である。
`KeepOverlayAfterExit=true`の保持動作はこの計測に含めていない。

## exe直接起動

最小のOpenVR初期化プログラムと、Valve公式の`vrcmd.exe`でも配置場所により結果が変わった。
同じexe・依存DLLを一時フォルダーへコピーすると直接起動に成功した。

Settingsサンプル`a4cb36c`を`Publish-SettingsSample.ps1`と同じ条件で
ユーザーのローカルアプリケーションデータ配下へ発行した結果、追加引数なしのexe直接起動で
Host開始とオーバーレイ作成を確認した。
作業フォルダー側と発行先のexe、サンプルDLL、FloatSoda.OVR.dll、openvr_api.dllはSHA-256が一致した。

作業フォルダーには発行先と異なる保護ACLがある。ACLの変更は行っていない。
配置条件への依存は確認できたが、OpenVR内部でどの条件が失敗を引き起こすかまでは未確定である。
発行先からの手動起動・Ctrl+C終了のユーザー判定は別途記録する。
