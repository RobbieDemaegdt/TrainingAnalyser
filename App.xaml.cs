using Maui2.Services;

namespace Maui2;

public partial class App : Application
{
	private readonly AuthService _authService;

    const int WindowWidth = 1300;
    const int WindowHeight = 768;

    public App(AuthService authService)
	{
		InitializeComponent();
		_authService = authService;
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		var shell = new AppShell();
		var window = new Window(shell)
		{
			Width = WindowWidth,
			Height = WindowHeight
		};

		shell.Loaded += async (s, e) =>
		{
			var success = await _authService.TryAutoLoginAsync();
			if (!success)
			{
				await shell.GoToAsync("LoginPage");
			}
		};

		return window;
	}
}