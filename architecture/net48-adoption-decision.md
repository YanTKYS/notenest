# .NET Framework 4.8 正式移行見送りメモ

## 目的

NestSuite v2.7.3 で整理した「.NET Framework 4.8 への正式移行は対応しない」という判断を、将来のために記録する。

本ドキュメントは、.NET Framework 4.8 版の検証結果と、正式採用を見送る理由を明文化するものである。

## 結論

NestSuite は、.NET Framework 4.8 への正式移行を行わない。

正式配布版は、引き続き .NET 8 self-contained single-file 版を基本とする。

.NET Framework 4.8 版は、実機で起動可能であることは確認したが、正式配布版としては採用しない。

## 背景

NestSuite は、閉域Windows端末で利用する小規模デスクトップツールとして開発している。

運用上、次の要件を重視している。

* 追加インストールをできるだけ避ける
* 利用者に配布しやすい
* 単体EXEに近い形で配布できる
* 配布物の構成が分かりやすい
* 一般職員が迷わず起動できる
* 管理者による展開・説明コストを抑える

この観点から、.NET Framework 4.8 版の可能性を検証した。

## 検証内容

v2.7.0〜v2.7.3 で、.NET Framework 4.8 向けの test build を追加し、起動可能性を確認した。

主な対応内容は次のとおり。

* `NestSuite.Net48Test.csproj` 相当の検証用プロジェクトを追加
* .NET 8 固有APIの一部を .NET Framework 4.8 互換に修正
* `Enum.GetValues<T>()` 等の非互換APIを置き換え
* range / index 構文等の互換性問題を修正
* `ToHashSet` 等の非互換APIを修正
* `OpenFolderDialog` 等の差異を整理
* .NET Framework 4.8 環境での起動を確認

## 検証結果

.NET Framework 4.8 版は、実機で起動可能であることを確認した。

ただし、正式採用には至らなかった。

理由は、.NET Framework 4.8 版では、現行の .NET 8 self-contained single-file 版と同等の単体EXE配布を標準機能だけで実現できないためである。

## 採用を見送る理由

### 1. 単体EXE配布の方針に合わない

NestSuite の配布では、利用者に分かりやすい単体EXEに近い構成を重視している。

.NET Framework 4.8 版では、DLL等を含む複数ファイル構成になりやすい。
これは、現在の配布方針と合わない。

複数ファイル構成になると、次の問題が出る。

* 利用者がどのファイルを起動すればよいか迷う
* ファイル一式を崩して配置される可能性がある
* 配布・更新時に説明が増える
* セキュリティ確認対象が増える
* 運用時の問い合わせ要因になる

### 2. 疑似単体EXE化はリスクが高い

Costura.Fody や ILRepack などを使えば、疑似的に単体EXE化できる可能性はある。

しかし、NestSuiteでは正式採用しない。

理由は次のとおり。

* 外部ツール依存が増える
* WPFリソースとの相性確認が必要になる
* ビルド手順が複雑になる
* セキュリティ評価・説明コストが増える
* 将来の保守負担が増える
* 期待どおりに動作しない場合の切り分けが難しい

閉域向け内部ツールとしては、配布容易性と保守性を優先する。

### 3. .NET 8版が既に正式配布方針を満たしている

現行の .NET 8 self-contained single-file 版は、配布物として扱いやすい。

容量は大きくなるが、利用者側の追加ランタイム導入を避けやすい。

NestSuite の現在の優先事項は、配布物サイズの最小化ではなく、次の点である。

* 起動しやすい
* 配布しやすい
* 説明しやすい
* 更新しやすい
* 利用者が迷いにくい

この観点では、.NET 8 self-contained single-file 版を継続する方が適している。

### 4. 互換対応を続けるコストが高い

.NET Framework 4.8 版を正式対応すると、今後のすべての実装で .NET 8 と .NET Framework 4.8 の両方を意識する必要がある。

これは、開発速度と保守性を下げる。

特に次の領域で負担が増える。

* 使用可能APIの差異
* WPF / XAML周辺の差異
* ファイルダイアログ等の差異
* テスト対象の増加
* GitHub Actions / release workflow の複雑化
* リリース成果物の増加
* 実機確認手順の増加

現時点では、この保守コストに見合う効果は小さい。

## 今後の方針

### 正式配布版

正式配布版は、引き続き .NET 8 self-contained single-file 版とする。

### .NET Framework 4.8版

.NET Framework 4.8 版の正式採用は行わない。

今後、原則として次の対応は行わない。

* .NET Framework 4.8 版の正式リリース
* .NET Framework 4.8 版の単体EXE化
* .NET Framework 4.8 互換のための追加実装
* .NET Framework 4.8 専用のUI調整
* .NET Framework 4.8 専用のrelease asset作成
* .NET Framework 4.8 を前提にした通常回帰テスト

### 検証履歴の扱い

.NET Framework 4.8 版の検証は、将来判断の参考として残す。

ただし、検証済みであることと、正式サポートすることは別である。

v2.7.3時点の判断は次のとおり。

* 起動可能性は確認済み
* 複数ファイル構成となるため正式採用しない
* .NET 8 self-contained single-file 版を正式配布版として継続する
* 通常リリース成果物から net48_test は外す
* net48関連の追加互換修正は原則停止する

## 例外的に再検討する条件

次のような条件がそろった場合のみ、再検討する余地がある。

* .NET 8 self-contained single-file 版が運用上どうしても利用できない
* 配布物サイズが重大な問題になる
* 対象環境で .NET Framework 4.8 版の複数ファイル配布が許容される
* 疑似単体EXE化の安全性・保守性が十分確認できる
* .NET 8 版との二重保守コストを受け入れる明確な理由がある

ただし、現時点では再検討予定はない。

## 他のクロスプラットフォーム検討との関係

.NET Framework 4.8 への正式移行見送りは、Android版、Mac版、HTML版、.NET MAUI版などの将来検討とは別の判断である。

.NET Framework 4.8 版は、あくまでWindows向けの別ターゲットである。
クロスプラットフォーム展開の解決策にはならない。

将来的に他プラットフォームを検討する場合は、.NET Framework 4.8 ではなく、次のような方向を別途検討する。

* NestSuite.Core の切り出し
* HTML Lite
* Avalonia
* .NET MAUI
* MAUI Blazor Hybrid

## 判断まとめ

NestSuite は、.NET Framework 4.8 への正式移行を行わない。

理由は次のとおり。

* 単体EXE配布の方針に合わない
* 複数ファイル構成は利用者向け配布で事故要因になる
* 疑似単体EXE化は保守・セキュリティ評価上のリスクがある
* .NET 8 self-contained single-file 版が現行方針に合っている
* .NET Framework 4.8 互換を維持する保守コストが高い

今後は、.NET 8 self-contained single-file 版を正式配布版として継続する。
