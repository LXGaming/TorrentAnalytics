using LXGaming.Common.Utilities;

namespace LXGaming.TorrentAnalytics.Utilities;

public static class Constants {

    public static class Application {

        public const string Name = "TorrentAnalytics";
        public const string Authors = "Alex Thomson";
        public const string Website = "https://lxgaming.me/";

        public static readonly string Version = AssemblyUtils.GetVersion(typeof(Constants).Assembly) ?? "Unknown";
        public static readonly string UserAgent = $"{Name}/{Version} (+{Website})";
    }
}