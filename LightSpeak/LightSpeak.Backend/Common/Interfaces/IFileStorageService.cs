namespace LightSpeak.Backend.Common.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveProfileImageAsync(Guid userId, Stream fileStream, string fileName, CancellationToken cancellationToken = default);

    Task DeleteProfileImageAsync(string fileUrl, CancellationToken cancellationToken = default);
}
