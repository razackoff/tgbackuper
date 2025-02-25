using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TelegramBackupClient.Infrastructure;
using TelegramBackupClient.Services;

class Program
{
    static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Register services
        builder.Services.AddScoped<ITelegramService, TelegramService>();
        builder.Services.AddScoped<IBackupService, BackupService>();
        builder.Services.AddScoped<IFileStorageService, FileStorageService>();
        builder.Services.AddHostedService<BackupSchedulerService>();

        // Add CORS for React frontend
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowReactApp",
                builder => builder
                    .WithOrigins("http://localhost:3000")
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });

        // Add logging
        builder.Services.AddLogging();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowReactApp");
        app.UseAuthorization();
        app.MapControllers();

        // Start React development server
        await StartReactDevServer();

        // Open browser
        await OpenBrowser("http://localhost:3000");

        // Run the web application
        await app.RunAsync();
    }

    private static async Task StartReactDevServer()
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "cmd.exe" : "npm",
                Arguments = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "/c npm start" : "start",
                WorkingDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "frontend"),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };

        process.Start();
        await Task.Delay(3000); // Give React dev server time to start
    }

    private static async Task OpenBrowser(string url)
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to open browser: {ex.Message}");
        }
    }
} 