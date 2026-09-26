using Quartz;

namespace LXGaming.TorrentAnalytics.Tests.Services.Quartz;

public class TestSchedulerFactory : ISchedulerFactory {

    public ValueTask<List<IScheduler>> GetAllSchedulers(CancellationToken cancellationToken = default) {
        throw new InvalidOperationException("Scheduler is unavailable.");
    }

    public ValueTask<IScheduler> GetScheduler(CancellationToken cancellationToken = default) {
        throw new InvalidOperationException("Scheduler is unavailable.");
    }

    public ValueTask<IScheduler?> LookupScheduler(string schedulerName, CancellationToken cancellationToken = default) {
        throw new InvalidOperationException("Scheduler is unavailable.");
    }
}