using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;

namespace EspinhoAI;

public partial class MainPage : ContentPage
{

    int count = 0;
    IConfiguration configuration;
    public MainPage(IConfiguration config, ILogger<MainPage> logger, MainViewModel viewModel)
    {
        InitializeComponent();

        configuration = config;
        logger.LogInformation("Test");
        //configuration = MauiProgram.Services.GetService<IConfiguration>();

        BindingContext = viewModel;
	}
}

