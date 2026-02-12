using Maui2.Services;
using Maui2.ViewModels;
using Maui2.Views;
using Microsoft.Extensions.Logging;

namespace Maui2;

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
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		builder.Services.AddSingleton<AuthService>();
		builder.Services.AddSingleton<Maui2.Services.PigeonService>();
		builder.Services.AddSingleton<Maui2.Services.HistoryService>();
		builder.Services.AddTransient<LoginViewModel>();
		builder.Services.AddTransient<Maui2.ViewModels.MainViewModel>();
		builder.Services.AddTransient<Maui2.ViewModels.HistoryViewModel>();
		builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient<MainPage>();
		builder.Services.AddTransient<Maui2.Views.HistoryPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
