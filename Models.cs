namespace Сетевое_программирование;

public record FtpSettings
{
    public string Host { get; init; } = "ftp.dlptest.com";
    public string Username { get; init; } = "dlpuser";
    public string Password { get; init; } = "rNrKYTX9g7z3RgJRmxWuGHbeu";
    public int Port { get; init; } = 21;
}

public class SpeedTestResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public long FileSizeBytes { get; set; }
    public TimeSpan Duration { get; set; }
    public double SpeedMbps => FileSizeBytes / Duration.TotalSeconds / (1024 * 1024);
    public double SpeedBytesPerSecond => FileSizeBytes / Duration.TotalSeconds;
    
    public string GetFormattedSize() => FileSizeBytes switch
    {
        < 1024 => $"{FileSizeBytes} B",
        < 1024 * 1024 => $"{FileSizeBytes / 1024.0:F1} KB",
        < 1024 * 1024 * 1024 => $"{FileSizeBytes / (1024.0 * 1024):F1} MB",
        _ => $"{FileSizeBytes / (1024.0 * 1024 * 1024):F1} GB"
    };
} 