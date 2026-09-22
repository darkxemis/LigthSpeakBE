namespace LightSpeak.Backend.Infrastructure.Services.FileStorage;

public sealed class FileStorageSettings
{
    public const string SectionName = "FileStorage";

    public string ProfileImagesPath { get; set; } = "uploads/profile-images";

    public string BaseUrl { get; set; } = string.Empty;
}
