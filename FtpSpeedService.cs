using FluentFTP;
using System.Diagnostics;

namespace Сетевое_программирование;

public class FtpSpeedService
{
    private readonly FtpSettings _settings;

    public FtpSpeedService(FtpSettings settings)
    {
        _settings = settings;
    }

    public async Task<SpeedTestResult> TestDownloadAsync(string fileName, IProgress<int>? progress = null)
    {
        var result = new SpeedTestResult();
        
        try
        {
            using var client = new AsyncFtpClient(_settings.Host, _settings.Username, _settings.Password, _settings.Port);
            await client.AutoConnect();

            if (!await client.FileExists(fileName))
            {
                result.ErrorMessage = $"Файл '{fileName}' не найден на сервере";
                return result;
            }

            var fileSize = await client.GetFileSize(fileName);
            result.FileSizeBytes = fileSize;

            var tempFile = Path.GetTempFileName();
            var progressHandler = progress != null ? new Progress<FtpProgress>(p => progress.Report((int)p.Progress)) : null;

            var stopwatch = Stopwatch.StartNew();
            
            var status = await client.DownloadFile(tempFile, fileName, FtpLocalExists.Overwrite, progress: progressHandler);
            
            stopwatch.Stop();
            result.Duration = stopwatch.Elapsed;

            if (status == FtpStatus.Success)
            {
                result.IsSuccess = true;
                File.Delete(tempFile); // Cleanup
            }
            else
            {
                result.ErrorMessage = $"Ошибка скачивания: {status}";
            }
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.Message;
        }

        return result;
    }

    public async Task<SpeedTestResult> TestUploadAsync(int fileSizeMB, IProgress<int>? progress = null)
    {
        var result = new SpeedTestResult();
        
        try
        {
            using var client = new AsyncFtpClient(_settings.Host, _settings.Username, _settings.Password, _settings.Port);
            await client.AutoConnect();

            var tempFile = CreateTestFile(fileSizeMB);
            result.FileSizeBytes = new FileInfo(tempFile).Length;

            var remoteFileName = $"test_{DateTime.Now:yyyyMMdd_HHmmss}.tmp";
            var progressHandler = progress != null ? new Progress<FtpProgress>(p => progress.Report((int)p.Progress)) : null;

            var stopwatch = Stopwatch.StartNew();
            
            var status = await client.UploadFile(tempFile, remoteFileName, progress: progressHandler);
            
            stopwatch.Stop();
            result.Duration = stopwatch.Elapsed;

            if (status == FtpStatus.Success)
            {
                result.IsSuccess = true;
                
                // Cleanup - удаляем файл с сервера
                try
                {
                    await client.DeleteFile(remoteFileName);
                }
                catch { /* Ignore cleanup errors */ }
            }
            else
            {
                result.ErrorMessage = $"Ошибка загрузки: {status}";
            }

            File.Delete(tempFile); // Cleanup local file
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.Message;
        }

        return result;
    }

    public async Task<List<string>> GetFileListAsync()
    {
        try
        {
            using var client = new AsyncFtpClient(_settings.Host, _settings.Username, _settings.Password, _settings.Port);
            await client.AutoConnect();

            var items = await client.GetListing();
            return items
                .Where(item => item.Type == FtpObjectType.File)
                .Select(item => $"{item.Name} ({FormatFileSize(item.Size)})")
                .ToList();
        }
        catch (Exception ex)
        {
            return [$"Ошибка: {ex.Message}"];
        }
    }

    private static string CreateTestFile(int sizeMB)
    {
        var tempFile = Path.GetTempFileName();
        var targetSize = sizeMB * 1024 * 1024;
        
        using var fs = new FileStream(tempFile, FileMode.Create);
        var buffer = new byte[8192];
        new Random().NextBytes(buffer);

        for (var written = 0; written < targetSize; written += buffer.Length)
        {
            var remaining = targetSize - written;
            var toWrite = Math.Min(buffer.Length, remaining);
            fs.Write(buffer, 0, toWrite);
        }

        return tempFile;
    }

    private static string FormatFileSize(long bytes) => bytes switch
    {
        < 1024 => $"{bytes} B",
        < 1024 * 1024 => $"{bytes / 1024.0:F1} KB", 
        < 1024 * 1024 * 1024 => $"{bytes / (1024.0 * 1024):F1} MB",
        _ => $"{bytes / (1024.0 * 1024 * 1024):F1} GB"
    };
} 