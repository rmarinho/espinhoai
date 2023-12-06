using Microsoft.Maui.Controls;

namespace EspinhoAI;

public partial class ExtractPage : ContentPage
{
	public ExtractPage(ExtractViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
