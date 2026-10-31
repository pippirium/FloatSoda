using FloatSoda.OVR;

namespace FloatSoda.Test.OVR;

/// <summary>
/// OpenVRのログ出力先を準備する <see cref="VRLogPath"/> の階層決定と書き込み可否の判定を検証します。
/// </summary>
/// <remarks>
/// <c>VR_GetRuntimePath</c> の照会と <c>VR_LOG_PATH</c> への設定はOpenVRランタイムを要するため、
/// ここでは決定ロジックの純粋な部分だけをテストします。
/// </remarks>
public class VRLogPathTest
{
    [Fact]
    public void GetLogsDirectory_Runtime付きでないランタイムパス_同じ階層のlogsを返す()
    {
        var runtime = Path.Combine("C:", "Steam", "steamapps", "common", "SteamVR");

        var logs = VRLogPath.GetLogsDirectory(runtime);

        Assert.Equal(Path.Combine(runtime, "logs"), logs);
    }

    [Fact]
    public void GetLogsDirectory_末尾にruntimeを含む_1階層上へ上がったlogsを返す()
    {
        var runtime = Path.Combine("C:", "Steam", "steamapps", "common", "SteamVR", "runtime");

        var logs = VRLogPath.GetLogsDirectory(runtime);

        Assert.Equal(Path.Combine("C:", "Steam", "steamapps", "common", "SteamVR", "logs"), logs);
    }

    [Fact]
    public void GetLogsDirectory_末尾にbinを含む_1階層上へ上がったlogsを返す()
    {
        var runtime = Path.Combine("C:", "Steam", "steamapps", "common", "SteamVR", "bin");

        var logs = VRLogPath.GetLogsDirectory(runtime);

        Assert.Equal(Path.Combine("C:", "Steam", "steamapps", "common", "SteamVR", "logs"), logs);
    }

    [Fact]
    public void GetLogsDirectory_末尾区切り付き_末尾の区切りを無視して決定する()
    {
        var runtime =
            Path.Combine("C:", "Steam", "steamapps", "common", "SteamVR")
            + Path.DirectorySeparatorChar;

        Assert.Equal(
            Path.Combine("C:", "Steam", "steamapps", "common", "SteamVR", "logs"),
            VRLogPath.GetLogsDirectory(runtime)
        );
    }

    [Fact]
    public void GetLogsDirectory_大文字小文字の違いもruntime階層として扱う()
    {
        var runtime = Path.Combine("C:", "Steam", "steamapps", "common", "SteamVR", "RUNTIME");

        Assert.Equal(
            Path.Combine("C:", "Steam", "steamapps", "common", "SteamVR", "logs"),
            VRLogPath.GetLogsDirectory(runtime)
        );
    }

    [Fact]
    public void GetLogsDirectory_空とnullはnullを返す()
    {
        Assert.Null(VRLogPath.GetLogsDirectory(null));
        Assert.Null(VRLogPath.GetLogsDirectory(""));
        Assert.Null(VRLogPath.GetLogsDirectory("   "));
    }

    [Fact]
    public void GetLogsDirectory_区切りのみを渡されたらnullを返す()
    {
        // 階層をたどれない入力では決定しない。GetDirectoryName が null を返すための入力を扱う。
        Assert.Null(VRLogPath.GetLogsDirectory(Path.DirectorySeparatorChar.ToString()));
    }

    [Fact]
    public void LogFileName_vrclient接頭辞と実行ファイル名を持つ()
    {
        var name = VRLogPath.LogFileName;

        Assert.StartsWith("vrclient_", name);
        Assert.EndsWith(".txt", name);
        var stem = Path.GetFileNameWithoutExtension(Environment.ProcessPath);
        Assert.Equal($"vrclient_{stem}.txt", name);
    }

    [Fact]
    public void GetFallbackLogsDirectory_BaseDirectory配下のvrlogsを返す()
    {
        var fallback = VRLogPath.GetFallbackLogsDirectory();

        Assert.NotNull(fallback);
        Assert.Equal(Path.Combine(AppContext.BaseDirectory, "vrlogs"), fallback);
    }

    [Fact]
    public void CanCreateLogFile_書き込み可能な一時ディレクトリ_trueを返す()
    {
        var directory = Path.Combine(
            Path.GetTempPath(),
            "floatsoda-vrlogpath-" + Guid.NewGuid().ToString("N")
        );

        try
        {
            Assert.True(VRLogPath.CanCreateLogFile(directory));
            // 存在しないディレクトリは作成される。
            Assert.True(Directory.Exists(directory));
            // 確認で使ったログファイルは残さない。
            Assert.False(File.Exists(Path.Combine(directory, VRLogPath.LogFileName)));
        }
        finally
        {
            Directory.Delete(directory, true);
        }
    }

    [Fact]
    public void CanCreateLogFile_既存のログファイルは保持する()
    {
        var directory = Path.Combine(
            Path.GetTempPath(),
            "floatsoda-vrlogpath-" + Guid.NewGuid().ToString("N")
        );
        var logFile = Path.Combine(directory, VRLogPath.LogFileName);

        try
        {
            Directory.CreateDirectory(directory);
            File.WriteAllText(logFile, "既存のログ");

            Assert.True(VRLogPath.CanCreateLogFile(directory));
            Assert.True(File.Exists(logFile));
        }
        finally
        {
            Directory.Delete(directory, true);
        }
    }

    [Fact]
    public void CanCreateLogFile_階層の下に作成できる_trueを返す()
    {
        var root = Path.Combine(
            Path.GetTempPath(),
            "floatsoda-vrlogpath-" + Guid.NewGuid().ToString("N")
        );
        var directory = Path.Combine(root, "nested", "vrlogs");

        try
        {
            Assert.True(VRLogPath.CanCreateLogFile(directory));
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void CanCreateLogFile_ログファイル名がディレクトリと衝突すれば_falseを返す()
    {
        // 確認対象のログファイルと同じ名前のディレクトリを作成すると開けない。
        var root = Path.Combine(
            Path.GetTempPath(),
            "floatsoda-vrlogpath-" + Guid.NewGuid().ToString("N")
        );
        Directory.CreateDirectory(Path.Combine(root, VRLogPath.LogFileName));

        try
        {
            Assert.False(VRLogPath.CanCreateLogFile(root));
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void EnvironmentVariable_OpenVRが読む変数名を保持する()
    {
        Assert.Equal("VR_LOG_PATH", VRLogPath.EnvironmentVariable);
    }
}
