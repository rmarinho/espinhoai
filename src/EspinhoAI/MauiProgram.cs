using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Maui.Hosting;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace EspinhoAI;

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
        builder.Services.AddTransient<AppShell>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<ExtractPage>();
        builder.Services.AddTransient<InferancePage>();
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<ExtractViewModel>();

#if DEBUG
        var a = Assembly.GetExecutingAssembly();
        using var stream = a.GetManifestResourceStream("EspinhoAI.appsettings.json");

        var config = new ConfigurationBuilder()
            .AddJsonStream(stream)
            .Build();


        builder.Configuration.AddConfiguration(config);
        builder.Logging.AddDebug();
#endif
		return builder.Build();
	}

   
}
