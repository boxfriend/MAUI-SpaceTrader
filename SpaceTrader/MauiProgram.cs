using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RestSharp;
using RestSharp.Serializers.Json;
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

		InitLogger(envPath);

        var clientOptions = new RestClientOptions(@"https://api.spacetraders.io/v2");
		var serializeOptions = new JsonSerializerOptions(new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        builder.Services.AddSingleton(s => new RestClient(clientOptions,configureSerialization: s => s.UseSystemTextJson(serializeOptions)));

		var path = Path.Combine(envPath, "Agents.db");
		builder.Services.AddSingleton(s => ActivatorUtilities.CreateInstance<AgentDbController>(s, path));

		return builder.Build();
	}

    private static void InitLogger (string path)
    {
		const string template = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] ({ThreadId}) {Message:lj}{NewLine}{Exception}";

        Log.Logger = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Verbose()
			.WriteTo.Async(a => a.Console(outputTemplate: template))
#else
            .MinimumLevel.Information()
#endif
            .WriteTo.Async(a => a.File(Path.Combine(path, "log.txt"),
                outputTemplate: template))
            .CreateLogger();

        Log.Information("Serilog Initialized. Logging started.");
    }
}
