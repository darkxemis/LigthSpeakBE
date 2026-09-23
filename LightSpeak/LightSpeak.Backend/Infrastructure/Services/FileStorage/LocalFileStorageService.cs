namespace LightSpeak.Backend.Infrastructure.Services.FileStorage;

using LightSpeak.Backend.Common.Interfaces;
using Microsoft.Extensions.Options;

public sealed class LocalFileStorageService(IOptions<FileStorageSettings> options) : IFileStorageService
{
    private readonly FileStorageSettings _settings = options.Value;

    private static readonly HashSet<string> AllowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp"];

    public async Task<string> SaveProfileImageAsync(
        Guid userId,
        Stream fileStream,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        return await SaveImageAsync(
            _settings.ProfileImagesPath,
            "profile-images",
            userId,
            fileStream,
            fileName,
            cancellationToken);
    }

    public Task DeleteProfileImageAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        return DeleteImageAsync(_settings.ProfileImagesPath, fileUrl);
    }

    public async Task<string> SaveServerIconAsync(
        Guid serverId,
        Stream fileStream,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        return await SaveImageAsync(
            _settings.ServerIconsPath,
            "server-icons",
            serverId,
            fileStream,
            fileName,
            cancellationToken);
    }

    public Task DeleteServerIconAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        return DeleteImageAsync(_settings.ServerIconsPath, fileUrl);
    }

    private async Task<string> SaveImageAsync(
        string directoryPath,
        string urlSegment,
        Guid ownerId,
        Stream fileStream,
        string fileName,
        CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        if (!AllowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException($"File extension '{extension}' is not allowed.");
        }

        var directory = Path.GetFullPath(directoryPath);
        Directory.CreateDirectory(directory);

        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var safeFileName = $"{ownerId}_{timestamp}{extension}";
        var filePath = Path.Combine(directory, safeFileName);

        await using var file = File.Create(filePath);
        await fileStream.CopyToAsync(file, cancellationToken);

        var baseUrl = _settings.BaseUrl.TrimEnd('/');
        return $"{baseUrl}/{urlSegment}/{safeFileName}";
    }

    private Task DeleteImageAsync(string directoryPath, string fileUrl)
    {
        if (string.IsNullOrWhiteSpace(fileUrl))
        {
            return Task.CompletedTask;
        }

        var fileName = Path.GetFileName(new Uri(fileUrl).LocalPath);
        var directory = Path.GetFullPath(directoryPath);
        var filePath = Path.Combine(directory, fileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }
}
