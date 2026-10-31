using System.Runtime.InteropServices;

namespace FloatSoda.OVR;

/// <summary>
/// OpenVRのクライアントログ出力先を初期化する前に準備します。
/// </summary>
/// <remarks>
/// SteamVRのクライアントDLLは、プロセスの実行ファイル名から決まるログファイル
/// （例: <c>vrclient_MyApp.txt</c>）をSteamVRランタイムの親にある <c>logs</c> ディレクトリへ開こうとし、
/// 開けない場合は <see cref="EVRInitError.Init_NoLogPath"/> を返して初期化自体を失敗させます。
/// その <c>logs</c> ディレクトリへ書き込めない配置（Steamのインストール先が管理者専用など）では、
/// 実行ファイル名のログファイルが作られないまま毎回起動に失敗し続けます。
/// OpenVRはログ出力先を <c>VR_LOG_PATH</c> 環境変数で上書きできるため、既定のディレクトリへ
/// 書き込めないときだけ書き込み可能な代替ディレクトリへ退避させます。
/// </remarks>
internal static class VRLogPath
{
    /// <summary>OpenVRへログ出力先を指定する環境変数名。</summary>
    internal const string EnvironmentVariable = "VR_LOG_PATH";

    /// <summary>退避先としてアプリと同じディレクトリに作るディレクトリ名。</summary>
    internal const string FallbackDirectoryName = "vrlogs";

    /// <summary>OpenVRが既定のログ出力先として使うディレクトリ名。</summary>
    internal const string LogsDirectoryName = "logs";

    /// <summary>
    /// OpenVRのログ出力先が書き込み可能なディレクトリになるよう <c>VR_LOG_PATH</c> を準備します。
    /// </summary>
    /// <returns>退避させた場合は退避先のパス。退避が不要、または準備に失敗した場合は<see langword="null"/>。</returns>
    /// <remarks>
    /// すでに <c>VR_LOG_PATH</c> が設定されている場合はユーザーの設定を尊重して何もしません。
    /// 準備に失敗しても OpenVR の初期化は続行します。事前準備の失敗で初期化を潰さず、
    /// 本来の失敗理由を隠さないためです。
    /// </remarks>
    public static string? EnsureUsableLogsDirectory()
    {
        try
        {
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(EnvironmentVariable)))
            {
                return null;
            }

            // OpenVRは既定の logs ディレクトリへ「この実行ファイル名のログファイル」を開こうとします。
            // 同じ名前を同じ開き方で開けるかを確認し、開けないときだけ退避させます。
            // ディレクトリ自体への書き込み可否では、名前の衝突や親ディレクトリの権限違いを捉えられません。
            var defaultLogs = GetDefaultLogsDirectory();
            if (defaultLogs is not null && CanCreateLogFile(defaultLogs))
            {
                return null;
            }

            var fallback = GetFallbackLogsDirectory();
            if (fallback is null || !CanCreateLogFile(fallback))
            {
                return null;
            }

            Environment.SetEnvironmentVariable(EnvironmentVariable, fallback);
            return fallback;
        }
        catch (Exception)
        {
            // ログ出力先の準備に失敗してもOpenVRの初期化自体は成功しうるため、致命的扱いにしない。
            return null;
        }
    }

    /// <summary>
    /// 指定ディレクトリへ、OpenVRが実際に使う名前のログファイルを作成できるか確認します。
    /// </summary>
    /// <param name="directory">確認するディレクトリ。存在しない場合は作成を試みます。</param>
    /// <returns>OpenVRと同じ名前のログファイルをOpenVRと同じ開き方で開けた場合は<see langword="true"/>。</returns>
    /// <remarks>
    /// OpenVRは実行ファイル名から決まる名前のログファイルを <c>CREATE_ALWAYS</c> で開くため、
    /// 同じ名前・同じ開き方・同じ共有モードで確認します。別名の一時ファイルで確認すると、
    /// ディレクトリへ書き込めても目的のファイルが開けない配置で誤って「書き込める」と判定します。
    /// 既存ファイルは残し、存在しなかったときだけ作成して削除します。
    /// </remarks>
    internal static bool CanCreateLogFile(string directory)
    {
        var logFile = Path.Combine(directory, LogFileName);
        var existedBefore = File.Exists(logFile);
        try
        {
            Directory.CreateDirectory(directory);
            using var _ = new FileStream(
                logFile,
                FileMode.Create,
                FileAccess.Write,
                FileShare.Read
            );
            return true;
        }
        catch (IOException)
        {
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
        finally
        {
            if (!existedBefore)
            {
                TryDelete(logFile);
            }
        }
    }

    /// <summary>
    /// このプロセスに対するOpenVRのログファイル名。
    /// </summary>
    /// <remarks>
    /// OpenVRのローダースタブはプロセスの実行ファイル名（拡張子を除く）から
    /// <c>vrclient_&lt;ファイル名&gt;.txt</c> の名前を作ります。<c>dotnet App.dll</c> のように実行ファイル名が
    /// 異なる起動方法では別名のログファイルになるため、この確認は apphost から起動した場合の挙動に一致します。
    /// </remarks>
    internal static string LogFileName
    {
        get
        {
            var processPath = Environment.ProcessPath;
            var name = string.IsNullOrEmpty(processPath)
                ? Path.GetFileNameWithoutExtension(AppContext.BaseDirectory)
                : Path.GetFileNameWithoutExtension(processPath);
            if (string.IsNullOrEmpty(name))
            {
                name = "unknown";
            }

            return $"vrclient_{name}.txt";
        }
    }

    /// <summary>
    /// OpenVRが既定のログ出力先として使うディレクトリを決定します。
    /// </summary>
    /// <returns>OpenVRランタイムの位置から決めた logs ディレクトリ。決定できない場合は<see langword="null"/>。</returns>
    /// <remarks>
    /// OpenVRのローダースタブは <c>VR_LOG_PATH</c> が未設定のとき、<c>VR_GetRuntimePath</c> で得た
    /// SteamVRランタイムの階層から <c>runtime</c> と <c>bin</c> を除いたディレクトリの <c>logs</c> を
    /// 既定のログ出力先として使います。環境変数ではなくこのAPIで照会するのは、Steamの位置が
    /// 環境変数に現れない実行環境でもDLLが実際に選ぶ出力先を特定できるためです。
    /// </remarks>
    internal static string? GetDefaultLogsDirectory()
    {
        return GetLogsDirectory(QueryRuntimePath()) is { } logs ? logs : null;
    }

    /// <summary>
    /// OpenVRランタイムのパスから、既定のログ出力先を導きます。
    /// </summary>
    /// <param name="runtimePath"><c>VR_GetRuntimePath</c> が返したSteamVRランタイムのパス。</param>
    /// <returns>ログ出力先の logs ディレクトリ。導けな場合は<see langword="null"/>。</returns>
    /// <remarks>
    /// ローダースタブの <c>VRServerRuntimePath</c> は SteamVRランタイムの階層から末尾の <c>runtime</c> または
    /// <c>bin</c> を1つ取り除いたディレクトリの <c>logs</c> をログ出力先にするため、同じ階層をたどります。
    /// 例: <c>C:\Steam\steamapps\common\SteamVR</c> と <c>C:\Steam\steamapps\common\SteamVR\runtime</c> は、
    /// いずれも <c>C:\Steam\logs</c> になります。
    /// </remarks>
    internal static string? GetLogsDirectory(string? runtimePath)
    {
        if (string.IsNullOrWhiteSpace(runtimePath))
        {
            return null;
        }

        var current = runtimePath.TrimEnd(
            Path.DirectorySeparatorChar,
            Path.AltDirectorySeparatorChar
        );
        var leaf = Path.GetFileName(current);
        if (
            string.Equals(leaf, "runtime", StringComparison.OrdinalIgnoreCase)
            || string.Equals(leaf, "bin", StringComparison.OrdinalIgnoreCase)
        )
        {
            current = Path.GetDirectoryName(current) ?? string.Empty;
        }

        return string.IsNullOrEmpty(current) ? null : Path.Combine(current, LogsDirectoryName);
    }

    /// <summary>
    /// 既定のログ出力先へ書き込めない場合に使う代替ディレクトリのパスを返します。
    /// </summary>
    /// <returns>アプリと同じディレクトリの <c>vrlogs</c>。パスを決定できない場合は<see langword="null"/>。</returns>
    /// <remarks>存在しないディレクトリでも返します。作成と書き込み可否の確認は呼び出し元が行います。</remarks>
    internal static string? GetFallbackLogsDirectory()
    {
        var baseDirectory = AppContext.BaseDirectory;
        if (string.IsNullOrWhiteSpace(baseDirectory))
        {
            return null;
        }

        return Path.Combine(baseDirectory, FallbackDirectoryName);
    }

    /// <summary>
    /// OpenVRのローダースタブからSteamVRランタイムの位置を照会します。
    /// </summary>
    /// <returns>ランタイムのパス。ローダースタブを読み込めない、または SteamVR がインストールされていない場合は<see langword="null"/>。</returns>
    /// <remarks>
    /// OpenVRの初期化より前に呼ぶため、この照会自体はOpenVRの初期化を必要としません。
    /// バインディングが提供する <c>RuntimePath()</c> は内部で失敗時に非推奨の
    /// <c>VR_RuntimePath</c> へ退避するため、古いランタイムでも決まります。
    /// </remarks>
    private static string? QueryRuntimePath()
    {
        var path = OpenVR.RuntimePath();
        return string.IsNullOrWhiteSpace(path) ? null : path;
    }

    private static void TryDelete(string path)
    {
        try
        {
            File.Delete(path);
        }
        catch (IOException)
        {
            // 確認用のログファイルは削除できても削除できなくても書き込み可否の判定結果には影響しない。
        }
        catch (UnauthorizedAccessException)
        {
            // 同上。削除-onlyの権限設定でも判定結果は変わらない。
        }
    }
}
