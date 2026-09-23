namespace LightSpeak.Backend.Common.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveProfileImageAsync(Guid userId, Stream fileStream, string fileName, CancellationToken cancellationToken = default);

    Task DeleteProfileImageAsync(string fileUrl, CancellationToken cancellationToken = default);

    Task<string> SaveServerIconAsync(Guid serverId, Stream fileStream, string fileName, CancellationToken cancellationToken = default);

    Task DeleteServerIconAsync(string fileUrl, CancellationToken cancellationToken = default);
}
