using LXGaming.Common.Utilities;

namespace LXGaming.FloodAnalytics.Utilities;

public static class Constants {

    public static class Application {

        public const string Name = "FloodAnalytics";
        public const string Authors = "Alex Thomson";
        public const string Website = "https://lxgaming.me/";

        public static readonly string Version = AssemblyUtils.GetVersion(typeof(Constants).Assembly) ?? "Unknown";
        public static readonly string UserAgent = $"{Name}/{Version} (+{Website})";
    }
}