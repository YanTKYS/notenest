using System.IO;
using System.Text;

namespace NestSuite.Services;

/// <summary>
/// 保存先ディレクトリ作成・tmp 経由 atomic write・finally tmp cleanup を共通化する。
/// NoteNest / IdeaNest / ChatNest の保存処理で共有する。
/// </summary>
public static class AtomicFileWriter
{
    /// <summary>
    /// path へ content を atomic に書き込む。
    /// 保存先ディレクトリがなければ作成し、tmp ファイル経由で置換または移動する。
    /// 保存成功・失敗を問わず finally で tmp を削除する。cleanup 失敗は ErrorLog に記録し
    /// 本来の保存例外を隠さない。
    /// </summary>
    /// <param name="path">保存先ファイルパス。フルパス推奨。</param>
    /// <param name="content">書き込むテキスト。</param>
    /// <param name="encoding">文字エンコーディング。各サービスの既存方針を維持すること。</param>
    /// <param name="backupPath">
    /// 既存ファイル置換時のバックアップパス。null の場合はバックアップを作成しない。
    /// </param>
    public static void WriteAllText(
        string path,
        string content,
        Encoding encoding,
        string? backupPath = null)
    {
        var directory = Path.GetDirectoryName(Path.GetFullPath(path));
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        var tempPath = path + ".tmp";
        try
        {
            File.WriteAllText(tempPath, content, encoding);
            if (File.Exists(path))
                File.Replace(tempPath, path, backupPath);
            else
                File.Move(tempPath, path);
        }
        finally
        {
            TryDeleteTemp(tempPath);
        }
    }

    private static void TryDeleteTemp(string tempPath)
    {
        try
        {
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
        catch (Exception ex)
        {
            ErrorLogService.Log("AtomicFileWriterCleanup", ex);
        }
    }
}
