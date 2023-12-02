using Microsoft.Maui.Controls;

namespace EspinhoAI;

public partial class ExtractPage : ContentPage
{
	public ExtractPage()
	{
		InitializeComponent();
		BindingContext = new ExtractViewModel();
	}
}
