using Quartz;

namespace LXGaming.TorrentAnalytics.Tests.Services.Quartz;

public class TestSchedulerFactory : ISchedulerFactory {

    public Task<IReadOnlyList<IScheduler>> GetAllSchedulers(CancellationToken cancellationToken = default) {
        throw new InvalidOperationException("Scheduler is unavailable");
    }

    public Task<IScheduler> GetScheduler(CancellationToken cancellationToken = default) {
        throw new InvalidOperationException("Scheduler is unavailable");
    }

    public Task<IScheduler?> GetScheduler(string schedName, CancellationToken cancellationToken = default) {
        throw new InvalidOperationException("Scheduler is unavailable");
    }
}