using LXGaming.Common.Utilities;

namespace LXGaming.TorrentAnalytics.Services.InfluxDb.Models;

public class Fields {

    public Dictionary<string, double> FloatingPoint { get; } = new();

    public Dictionary<string, long> Integral { get; } = new();

    public Fields AddOrIncrement(string key, double value) {
        CollectionUtils.AddOrIncrement(FloatingPoint, key, Math.Max(value, 0));
        return this;
    }

    public Fields AddOrIncrement(string key, long value) {
        CollectionUtils.AddOrIncrement(Integral, key, Math.Max(value, 0));
        return this;
    }
}