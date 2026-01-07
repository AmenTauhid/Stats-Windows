namespace Stats.Core.Models;

public record NetworkInfo
{
    public string AdapterName { get; init; } = string.Empty;
    public long DownloadRate { get; init; }
    public long UploadRate { get; init; }
    public long TotalDownloaded { get; init; }
    public long TotalUploaded { get; init; }
    public bool IsConnected { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
