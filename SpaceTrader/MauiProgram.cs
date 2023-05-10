using Microsoft.Extensions.Logging;
using RestSharp;
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

		var clientOptions = new RestClientOptions("https://api.spacetraders.io/v2");
		builder.Services.AddSingleton<RestClient>();

		var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Agents.db");
		builder.Services.AddSingleton(s => ActivatorUtilities.CreateInstance<AgentDbController>(s, path));

		return builder.Build();
	}
}
