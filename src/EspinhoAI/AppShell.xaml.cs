namespace EspinhoAI;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute("docs/doc", typeof(DocumentPage));

	}
}
