using System.ComponentModel;
using System.Diagnostics;
using FloatSoda.Samples.Settings;

namespace FloatSoda.Test.Samples.Settings;

public class ScreenViewLauncherTest
{
    [Fact]
    public void Start_起動先がない_プロセスを作らず結果を返す()
    {
        var calls = 0;
        using var launcher = new ScreenViewLauncher(
            Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".exe"),
            _ =>
            {
                calls++;
                return null;
            }
        );
        Assert.Equal("ScreenViewの起動先が見つかりません", launcher.Start());
        Assert.Equal(0, calls);
    }

    [Fact]
    public void Start_空白を含むパスで連続起動_シェルを使わず一度だけ起動する()
    {
        var file = Path.Combine(Path.GetTempPath(), "screenview test " + Guid.NewGuid() + ".exe");
        File.WriteAllText(file, "起動処理はテスト用の関数へ差し替えるため実行されません。");
        try
        {
            ProcessStartInfo? captured = null;
            var calls = 0;
            using var launcher = new ScreenViewLauncher(
                file,
                info =>
                {
                    captured = info;
                    calls++;
                    return Process.GetCurrentProcess();
                }
            );
            Assert.Equal("ScreenViewを起動しました", launcher.Start());
            Assert.Equal("ScreenViewは起動済みです", launcher.Start());
            Assert.Equal(1, calls);
            Assert.NotNull(captured);
            Assert.Equal(file, captured.FileName);
            Assert.Equal(Path.GetDirectoryName(file), captured.WorkingDirectory);
            Assert.False(captured.UseShellExecute);
            Assert.True(captured.CreateNoWindow);
        }
        finally
        {
            File.Delete(file);
        }
    }

    [Fact]
    public void Start_初回の起動に失敗_例外を画面へ漏らさず再試行できる()
    {
        var file = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".exe");
        File.WriteAllText(file, "実行されないテスト用ファイル");
        try
        {
            var calls = 0;
            using var launcher = new ScreenViewLauncher(
                file,
                _ =>
                {
                    if (++calls == 1)
                        throw new Win32Exception("起動失敗の再現");
                    return Process.GetCurrentProcess();
                }
            );
            Assert.Equal("ScreenViewを起動できませんでした", launcher.Start());
            Assert.Equal("ScreenViewを起動しました", launcher.Start());
            Assert.Equal(2, calls);
        }
        finally
        {
            File.Delete(file);
        }
    }
}
