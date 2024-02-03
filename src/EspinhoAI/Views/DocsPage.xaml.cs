namespace EspinhoAI;

public partial class DocsPage : ContentPage
{
	public DocsPage(DocsViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}