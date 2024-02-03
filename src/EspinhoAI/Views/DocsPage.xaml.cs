using AsyncAwaitBestPractices;

namespace EspinhoAI;

public partial class DocsPage : ContentPage
{
	DocsViewModel _viewModel;
	public DocsPage(DocsViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = _viewModel = viewModel;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		_viewModel?.LoadDocs().SafeFireAndForget(onException: ex => Console.WriteLine(ex));
	}
}