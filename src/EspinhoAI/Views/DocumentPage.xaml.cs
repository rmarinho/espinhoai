namespace EspinhoAI;

public partial class DocumentPage : ContentPage
{
	public DocumentPage(DocumentViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}