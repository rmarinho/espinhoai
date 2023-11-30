namespace EspinhoAI;

public partial class MainPage : ContentPage
{

	public MainPage()
	{
		InitializeComponent();
		BindingContext = new MainViewModel();
	}
}

