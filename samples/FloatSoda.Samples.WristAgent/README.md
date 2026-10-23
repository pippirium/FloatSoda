# FloatSoda.Samples.WristAgent

左腕(左コントローラー)の腕時計位置に、AI エージェント(Codex など)の承認要求を
カードとして出し、レーザーポインターで はい / いいえ / 続行 を選べるサンプルです。

## 起動

```powershell
dotnet run --project samples/FloatSoda.Samples.WristAgent
# ポートを変える場合
dotnet run --project samples/FloatSoda.Samples.WristAgent -- 8801
```

SteamVR が起動している必要があります。起動すると左腕に `WristAgent` カードが出ます。

## HTTP プロトコル

`http://127.0.0.1:{port}/requests` へ POST します(ローカルのみ)。

```jsonc
// 要求
{ "id": "abc123", "kind": "approve", "agent": "codex", "tool": "Bash", "detail": "git push …" }
```

- `kind`: `approve`(承認) / `continue`(継続) / `question`(自由入力) / `notify`(通知のみ)
- 応答はユーザーが腕 UI で選択するまで保持され、選択後に返ります。
  最大待ち時間は既定 5 分で、超えると `{ "decision": "timeout" }` を返します。

```jsonc
// 応答
{ "decision": "allow" }          // はい / continue
{ "decision": "deny" }           // いいえ
{ "decision": "text", "value": "…" }   // フリーテキスト(後述の OSK 連携)
{ "decision": "timeout" }        // 未応答
```

試すには:

```powershell
curl http://127.0.0.1:8797/requests -d "{\"id\":\"t1\",\"kind\":\"approve\",\"agent\":\"test\",\"tool\":\"Bash\",\"detail\":\"echo hello\"}"
```

## エージェントへの接続(tools/WristAgentBridge)

`tools/WristAgentBridge` は stdin の hooks イベント JSON を腕 UI へ転送し、
ユーザーの選択を hooks 応答 JSON に変換する CLI です。応答が得られない場合は
何も出力せずに終了するため、エージェントは通常の承認フローにフォールバックします。

Codex の設定例(`~/.codex/hooks.json`)。

```json
{
  "hooks": {
    "PermissionRequest": [
      {
        "hooks": [
          {
            "type": "command",
            "command": "dotnet run --project C:/Users/haru/repo/FloatSoda/tools/WristAgentBridge --format codex",
            "timeout": 600
          }
        ]
      }
    ]
  }
}
```

Claude Code の設定例(`~/.claude/settings.json` の hooks)。

```json
{
  "hooks": {
    "PreToolUse": [
      {
        "matcher": "Bash|Edit|Write",
        "hooks": [
          {
            "type": "command",
            "command": "dotnet run --project C:/Users/haru/repo/FloatSoda/tools/WristAgentBridge --format claude",
            "timeout": 600
          }
        ]
      }
    ]
  }
}
```

注意:

- hooks を変更したら Codex では `/hooks` で trust review を通す必要があります。
- `timeout` は腕 UI の待ち時間(既定 5 分)より長めに設定してください。
- 本サンプルは `MakeOverlaysInteractiveIfVisible` を立てない
  `ControllerRayPointerSource` 経由の入力のため、SteamVR の入力横取りは発生しません。

## 既知の制限

- 入力は左トリガーのみ(右ボタン・スクロールなし)。
- フリーテキスト入力(手腕 UI の [入力] ボタン)は SteamVR OSK 連携を
  実装済み(このリポジトリの Phase 3)。`decision: "text"` は
  hooks 経由では deny + メッセージとして伝わります(MCP では自由テキスト)。
- 保留中の要求は複数もてますが、返答は常に最古の要求から選択する設計です。
