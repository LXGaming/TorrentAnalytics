using System.Runtime.Serialization;

namespace LXGaming.TorrentAnalytics.Services.QBittorrent.Models;

public enum TorrentFilter {

    [EnumMember(Value = "all")]
    All,

    [EnumMember(Value = "downloading")]
    Downloading,

    [EnumMember(Value = "seeding")]
    Seeding,

    [EnumMember(Value = "completed")]
    Completed,

    /// <remarks>
    /// 4.6.7, for 5.0.0+ use <see cref="Running"/>
    /// </remarks>
    [EnumMember(Value = "resumed")]
    Resumed,

    /// <remarks>
    /// 5.0.0+, for 4.6.7 use <see cref="Resumed"/>
    /// </remarks>
    [EnumMember(Value = "running")]
    Running,

    /// <remarks>
    /// 4.6.7, for 5.0.0+ use <see cref="Stopped"/>
    /// </remarks>
    [EnumMember(Value = "paused")]
    Paused,

    /// <remarks>
    /// 5.0.0+, for 4.6.7 use <see cref="Paused"/>
    /// </remarks>
    [EnumMember(Value = "stopped")]
    Stopped,

    [EnumMember(Value = "active")]
    Active,

    [EnumMember(Value = "inactive")]
    Inactive,

    [EnumMember(Value = "stalled")]
    Stalled,

    [EnumMember(Value = "stalled_uploading")]
    StalledUploading,

    [EnumMember(Value = "stalled_downloading")]
    StalledDownloading,

    [EnumMember(Value = "checking")]
    Checking,

    [EnumMember(Value = "moving")]
    Moving,

    [EnumMember(Value = "errored")]
    Errored
}