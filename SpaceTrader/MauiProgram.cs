using Microsoft.Extensions.Logging;
using Serilog;
using SpaceTrader.Data;

namespace SpaceTrader;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif
		var envPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MAUI-SpaceTraders");
		Directory.CreateDirectory(envPath);

        const string template = @"{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] ({ThreadId}) {Message:lj}{NewLine}{Exception}";
        builder.Services.AddLogging(logging => logging.AddSerilog(new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Verbose()
            .WriteTo.Console(outputTemplate: template)
#else
            .MinimumLevel.Information()
#endif
            .WriteTo.Async(a => a.File(Path.Combine(envPath, $"logs{Path.DirectorySeparatorChar}{DateTime.Now.Date:yyyy-MM-dd}.log"),
                outputTemplate: template))
			.Enrich.WithThreadId()
            .CreateLogger()));

        var path = Path.Combine(envPath, "Agents.db");
		builder.Services.AddSingleton(s => ActivatorUtilities.CreateInstance<AgentDbController>(s, path));
		builder.Services.AddSingleton(s => ActivatorUtilities.CreateInstance<ShipDbController>(s, path));
		builder.Services.AddSingleton(s => ActivatorUtilities.CreateInstance<SystemDbController>(s, path));

		builder.Services.AddSingleton(s => ActivatorUtilities.CreateInstance<ApiClient>(s));

		return builder.Build();
	}
}
