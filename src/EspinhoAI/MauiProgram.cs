using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Maui.Hosting;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using EspinhoAI.Services;
using UraniumUI;

namespace EspinhoAI;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
        var builder = MauiApp.CreateBuilder();
        builder
            .UseUraniumUI()
            .UseUraniumUIMaterial()
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFluentIconFonts();
            });
        builder.Services.AddSingleton<Repository>();
        builder.Services.AddTransient<AzureService>();
        builder.Services.AddTransient<PdfTextService>();
        builder.Services.AddTransient<AppShell>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<ExtractPage>();
        builder.Services.AddTransient<InferancePage>();
        builder.Services.AddTransient<DocsPage>();
        builder.Services.AddTransient<DocumentPage>();
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<ExtractViewModel>();
        builder.Services.AddTransient<DocsViewModel>();
        builder.Services.AddTransient<DocumentViewModel>();

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
