using LightSpeak.Backend;
using LightSpeak.Backend.Features.Auth;
using LightSpeak.Backend.Features.Users;
using LightSpeak.Backend.Infrastructure;
using LightSpeak.Backend.Infrastructure.Persistence;
using LightSpeak.Backend.Infrastructure.Services.FileStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Global Exceptions
builder.Services.AddGlobalExceptions();

// Configure OpenAPI documentation
builder.Services.AddPresentation();

// MediatR (CQRS) + FluentValidation pipeline
builder.Services.AddApplication();

// EF Core + JWT authentication
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [])
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithExposedHeaders("WWW-Authenticate");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.EnableDarkMode();
        options.WithTheme(ScalarTheme.BluePlanet);
        options.AddPreferredSecuritySchemes("Bearer");
    });
}

app.UseSerilogRequestLogging();

app.UseExceptionHandler();
app.UseCors("Frontend");
app.UseHttpsRedirection();

// Serve uploaded profile images as static files
var fileStorageSettings = builder.Configuration
    .GetSection(FileStorageSettings.SectionName)
    .Get<FileStorageSettings>() ?? new FileStorageSettings();

var profileImagesPath = Path.GetFullPath(fileStorageSettings.ProfileImagesPath);
Directory.CreateDirectory(profileImagesPath);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(profileImagesPath),
    RequestPath = "/profile-images"
});

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapUsersEndpoints();

// Automatically apply pending EF Core migrations at startup and seed dev data.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    var retry = 30;

    while (true)
    {
        try
        {
            db.Database.Migrate();
            break;
        }
        catch
        {
            retry--;
            if (retry == 0) throw;

            Thread.Sleep(2000);
        }
    }

    await DbSeeder.SeedAsync(scope.ServiceProvider);
}

app.Run();
