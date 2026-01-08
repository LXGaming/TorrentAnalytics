using LXGaming.Configuration;
using LXGaming.Configuration.Hosting;
using LXGaming.TorrentAnalytics.Configuration;
using LXGaming.TorrentAnalytics.Services.Web;
using LXGaming.TorrentAnalytics.Services.Web.Utilities;
using LXGaming.TorrentAnalytics.Tests.Configuration;
using LXGaming.TorrentAnalytics.Tests.Services.Quartz;
using LXGaming.TorrentAnalytics.Tests.Services.Web;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace LXGaming.TorrentAnalytics.Tests.Utilities;

public static class Extensions {

    public static IServiceCollection AddConfiguration(this IServiceCollection services) {
        if (services.Any(descriptor => descriptor.ServiceType == typeof(IConfiguration))) {
            throw new InvalidOperationException("Configuration is already registered");
        }

        return services.AddConfiguration(new TestConfiguration<Config>());
    }

    public static IServiceCollection AddSchedulerFactory(this IServiceCollection services) {
        if (services.Any(descriptor => descriptor.ServiceType == typeof(ISchedulerFactory))) {
            throw new InvalidOperationException("SchedulerFactory is already registered");
        }

        return services.AddSingleton<ISchedulerFactory, TestSchedulerFactory>();
    }

    public static IServiceCollection AddWebService(this IServiceCollection services) {
        if (services.Any(descriptor => descriptor.ServiceType == typeof(WebService))) {
            throw new InvalidOperationException("WebService is already registered");
        }

        return services
            .AddConfiguration()
            .AddWebService<WebService, TestWebService>();
    }
}