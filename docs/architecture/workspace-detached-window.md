# Workspace 別ウィンドウ表示（DetachedWorkspaceWindow）

v2.9.0〜v2.9.8 にわたって実装された、NoteNest / IdeaNest / ChatNest のタブを Shell から切り離して独立ウィンドウに表示する機能のアーキテクチャ概説です。

---

## 対応 Workspace

| Workspace | 別ウィンドウ対応 |
|-----------|----------------|
| NoteNest  | ○ |
| IdeaNest  | ○ |
| ChatNest  | ○ |
| TempNest  | × （固定タブのため対象外） |

---

## 基本設計

### 同一プロセス・別ウィンドウ

別ウィンドウ（`DetachedWorkspaceWindow`）は Shell と同じプロセス内で動作します。別プロセスは起動しません。

### ViewModel 共有・View 再作成

タブの ViewModel（`NestSuiteDocumentTab` および各 Workspace の ViewModel）は Shell タブとして管理し続けます。`DetachedWorkspaceWindow` に渡されるのは ViewModel の参照のみです。View（`NoteNestWorkspaceView` / `IdeaNestWorkspaceView` / `ChatNestWorkspaceView`）はウィンドウ側で新たに生成されます。

```
Shell タブ (NestSuiteDocumentTab)
  └─ ViewModel（共有・変更なし）
       └─ DetachedWorkspaceWindow
            └─ View（新規生成）
```

### Shell タブが管理の基本単位

タブの開閉・保存確認・セッション管理はすべて Shell 側の `NestSuiteDocumentTab` を通じて行います。別ウィンドウはあくまで「表示場所」の変更であり、タブの所有権は Shell に残ります。

---

## 操作フロー

### 分離（Detach）

タブのコンテキストメニュー → 「別ウィンドウで表示(_D)」を選択します。Shell のタブ領域からそのタブの View が取り除かれ、`DetachedWorkspaceWindow` が開いて View を再構築します。Shell のタブストリップにはタブ項目が残り（分離中を示す状態）、クリックするとウィンドウをフォーカスできます。

### 再統合（Reattach）

以下のいずれかの操作で再統合されます。

- 別ウィンドウの × ボタンを押す（閉じる操作 = 再統合）
- タブのコンテキストメニュー → 「このタブへ戻す(_R)」

再統合時は `DetachedWorkspaceWindow` が閉じられ、View が Shell のタブ領域に戻されます。未保存確認は **行いません**（再統合は保存を伴わない純粋な表示場所の変更であるため）。

### 保存・未保存確認

保存操作（`Ctrl+S` / 名前を付けて保存）は別ウィンドウ上でも従来どおり動作します。

タブを閉じる（Shell タブの × / アプリ終了）際の未保存確認は Shell 側で行います。別ウィンドウの × は再統合操作であるため、未保存確認のトリガーになりません。

---

## ウィンドウ所有（Owner）

`DetachedWorkspaceWindow.Owner` には Shell ウィンドウを設定しています。これにより以下の動作が得られます。

- Shell を最小化すると別ウィンドウも最小化される
- Shell を復元すると別ウィンドウも復元される
- タスクバーで Shell と同じグループに表示される

**制約:** Owner が設定された Owned ウィンドウは Owner より低い Z 順に移動できません。別ウィンドウを Shell の背後に送ることはできません。

---

## セッション保存

別ウィンドウの「分離状態」はセッションファイル（`session.json`）に保存しません。アプリ再起動時はすべてのタブが Shell に統合された状態で復元されます。セッション形式の変更はありません（`NestSuiteSessionState { FilePaths, ActiveFilePath }`）。

---

## 既知の制限

詳細は [docs/design/nestsuite-known-limitations.md](../design/nestsuite-known-limitations.md) の「Workspace 別ウィンドウ表示」節を参照してください。

- TempNest は非対応
- 別ウィンドウ状態・位置はセッション保存対象外
- 別ウィンドウは Shell より手前に固定（Owner 機構の制約）
- 複数ウィンドウのレイアウト保存なし
- Always on Top 非対応

---

## 実装履歴

| バージョン | 内容 |
|-----------|------|
| v2.9.0 | NoteNest 別ウィンドウ表示の初期実装 |
| v2.9.1 | タブ状態フラグ・スキーマ整合の確認テスト追加 |
| v2.9.4 | ChatNest 別ウィンドウ対応 |
| v2.9.5 | `DetachedWorkspaceWindow` 閉鎖時の NoteNest 補完クラッシュ修正 |
| v2.9.6 | `DetachedWorkspaceWindow` 最小幅 870 に統一、Owner Z-order 制約を既知制限として明記 |
