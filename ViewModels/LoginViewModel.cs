using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Maui2.Services;

namespace Maui2.ViewModels;

public class LoginViewModel : INotifyPropertyChanged
{
	private readonly AuthService _authService;

	private string _email = string.Empty;
	private string _password = string.Empty;
	private bool _rememberMe;
	private bool _isBusy;
	private string? _errorMessage;

	public LoginViewModel(AuthService authService)
	{
		_authService = authService;
		LoginCommand = new Command(async () => await LoginAsync(), () => !IsBusy);
		_ = LoadSavedCredentialsAsync();
	}

	public string Email
	{
		get => _email;
		set { _email = value; OnPropertyChanged(); }
	}

	public string Password
	{
		get => _password;
		set { _password = value; OnPropertyChanged(); }
	}

	public bool RememberMe
	{
		get => _rememberMe;
		set { _rememberMe = value; OnPropertyChanged(); }
	}

	public bool IsBusy
	{
		get => _isBusy;
		set
		{
			_isBusy = value;
			OnPropertyChanged();
			((Command)LoginCommand).ChangeCanExecute();
		}
	}

	public string? ErrorMessage
	{
		get => _errorMessage;
		set { _errorMessage = value; OnPropertyChanged(); }
	}

	public ICommand LoginCommand { get; }

	private async Task LoadSavedCredentialsAsync()
	{
		var (email, password) = await _authService.LoadCredentialsAsync();
		if (email is not null && password is not null)
		{
			Email = email;
			Password = password;
			RememberMe = true;
		}
	}

	private async Task LoginAsync()
	{
		if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
		{
			ErrorMessage = "Please enter both email and password.";
			return;
		}

		IsBusy = true;
		ErrorMessage = null;

		try
		{
			var success = await _authService.LoginAsync(Email, Password);
			if (success)
			{
				if (RememberMe)
					await _authService.SaveCredentialsAsync(Email, Password);
				else
					_authService.ClearCredentials();

				await Shell.Current.GoToAsync("..");
			}
			else
			{
				ErrorMessage = "Invalid email or password.";
			}
		}
		catch (Exception ex)
		{
			ErrorMessage = $"Login failed: {ex.Message}";
		}
		finally
		{
			IsBusy = false;
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
