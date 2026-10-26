using FloatSoda.Abstractions.Scheduling;
using FloatSoda.Engine;
using Microsoft.Extensions.Logging;

namespace FloatSoda.Test.Engine;

public class PostTaskRunnerTest
{
    private sealed class NoopFramePacer : IFramePacer
    {
        public void WaitForNextFrame(CancellationToken cancellationToken = default) { }
    }

    /// <summary>ログに記録されたエラー件数を数えるだけのロガー。</summary>
    private sealed class CountingLogger : ILogger
    {
        public int ErrorCount { get; private set; }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (logLevel == LogLevel.Error) ErrorCount++;
        }
    }

    /// <summary>GLFW を起動せずに <see cref="PostTaskRunner.DrainPendingTasks"/> だけを検証するための最小サブクラス。</summary>
    private sealed class TestPostTaskRunner(ILogger? logger)
        : PostTaskRunner("Test", new NoopFramePacer(), logger)
    {
        protected override void Update() { }

        public void Drain() => DrainPendingTasks();
    }

    private sealed class StoppingPostTaskRunner(ManualResetEventSlim releaseOnStop)
        : PostTaskRunner("StoppingTest", new CancellationFramePacer(), logger: null)
    {
        protected override void Update() { }

        protected override void OnStop() => releaseOnStop.Wait();

        public void StopAndJoin() => StopAndWait();
    }

    private sealed class CancellationFramePacer : IFramePacer
    {
        public void WaitForNextFrame(CancellationToken cancellationToken = default) =>
            cancellationToken.WaitHandle.WaitOne();
    }

    [Fact]
    public void DrainPendingTasks_タスクが例外を投げる_後続タスクは実行される()
    {
        var logger = new CountingLogger();
        var runner = new TestPostTaskRunner(logger);

        var ran = new List<int>();
        runner.PostTask(() => ran.Add(1));
        runner.PostTask(() => throw new InvalidOperationException("boom"));
        runner.PostTask(() => ran.Add(2));

        runner.Drain();

        // 例外を投げたタスクの後続も実行され、失敗はログに記録される
        Assert.Equal([1, 2], ran);
        Assert.Equal(1, logger.ErrorCount);
    }

    [Fact]
    public void DrainPendingTasks_複数タスクを投稿_FIFO順に実行される()
    {
        var runner = new TestPostTaskRunner(logger: null);

        var order = new List<int>();
        for (var i = 0; i < 5; i++)
        {
            var captured = i;
            runner.PostTask(() => order.Add(captured));
        }

        runner.Drain();

        Assert.Equal([0, 1, 2, 3, 4], order);
    }

    [Fact]
    public async Task StopAndWait_OnStopが完了するまで戻らない()
    {
        using var releaseOnStop = new ManualResetEventSlim();
        var runner = new StoppingPostTaskRunner(releaseOnStop);
        runner.Start(CancellationToken.None);

        var stop = Task.Run(runner.StopAndJoin);
        Assert.False(stop.Wait(TimeSpan.FromMilliseconds(50)));

        releaseOnStop.Set();
        await stop.WaitAsync(TimeSpan.FromSeconds(3));
    }
}
