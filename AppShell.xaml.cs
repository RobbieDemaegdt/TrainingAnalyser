namespace Maui2;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute("LoginPage", typeof(Views.LoginPage));
	}
}
