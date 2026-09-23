namespace LightSpeak.Backend.Infrastructure;

using LightSpeak.Backend.Infrastructure.Services.FileStorage;
using Microsoft.Extensions.FileProviders;

public static class StaticFileExtensions
{
    public static WebApplication UseUploadedStaticFiles(this WebApplication app)
    {
        var settings = app.Configuration
            .GetSection(FileStorageSettings.SectionName)
            .Get<FileStorageSettings>() ?? new FileStorageSettings();

        app.UseUploadedStaticFiles(settings.ProfileImagesPath, "/profile-images");
        app.UseUploadedStaticFiles(settings.ServerIconsPath, "/server-icons");

        return app;
    }

    private static void UseUploadedStaticFiles(
        this WebApplication app,
        string folderPath,
        string requestPath)
    {
        var fullPath = Path.GetFullPath(folderPath);
        Directory.CreateDirectory(fullPath);

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(fullPath),
            RequestPath = requestPath
        });
    }
}
