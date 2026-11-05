using System.ComponentModel;
using System.Diagnostics;

namespace FloatSoda.Samples.Settings;

/// <summary>別プロセスのScreenViewを起動します。設定サンプル終了時にもScreenViewは停止しません。</summary>
internal sealed class ScreenViewLauncher(
    string executablePath,
    Func<ProcessStartInfo, Process?>? startProcess = null
) : IDisposable
{
    private Process? _process;

    internal string Start()
    {
        try
        {
            if (_process is { HasExited: false })
                return "ScreenViewは起動済みです";

            if (!File.Exists(executablePath))
                return "ScreenViewの起動先が見つかりません";

            var fullPath = Path.GetFullPath(executablePath);
            var existing = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(fullPath));
            try
            {
                if (existing.Any(process => !process.HasExited))
                    return "ScreenViewは起動済みです";
            }
            finally
            {
                foreach (var process in existing)
                    process.Dispose();
            }

            _process?.Dispose();
            _process = null;
            _process = (startProcess ?? Process.Start)(
                new ProcessStartInfo
                {
                    FileName = fullPath,
                    WorkingDirectory = Path.GetDirectoryName(fullPath)!,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                }
            );
            return _process is null
                ? "ScreenViewを起動できませんでした"
                : "ScreenViewを起動しました";
        }
        catch (Exception exception)
            when (exception
                    is Win32Exception
                        or IOException
                        or InvalidOperationException
                        or UnauthorizedAccessException
            )
        {
            _process?.Dispose();
            _process = null;
            Console.Error.WriteLine($"ScreenViewの起動に失敗しました: {exception.Message}");
            return "ScreenViewを起動できませんでした";
        }
    }

    public void Dispose() => _process?.Dispose();
}
