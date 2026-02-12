using System.Net;
using System.Text.Json;

namespace Maui2.Services;

public class AuthService
{
	private const string EmailKey = "saved_email";
	private const string PasswordKey = "saved_password";

	private HttpClient? _httpClient;

	public bool IsLoggedIn { get; private set; }

	public event Action? LoggedIn;

	public HttpClient HttpClient
	{
		get
		{
			EnsureHttpClient();
			return _httpClient!;
		}
	}

	private void EnsureHttpClient()
	{
		if (_httpClient is not null)
			return;

		var handler = new HttpClientHandler
		{
			CookieContainer = new CookieContainer(),
			UseCookies = true
		};

		_httpClient = new HttpClient(handler);
	}

	public async Task<bool> LoginAsync(string email, string password)
	{
		EnsureHttpClient();

		var jsonContent = new StringContent(
			JsonSerializer.Serialize(new { email, password }),
			System.Text.Encoding.UTF8,
			"application/json");

		var response = await _httpClient!.PostAsync(
			"https://www.pigeonfancier.com/api/user/login?useCookies=true&useSessionCookies=false",
			jsonContent);

		IsLoggedIn = response.IsSuccessStatusCode;
		if (IsLoggedIn)
			LoggedIn?.Invoke();
		return IsLoggedIn;
	}

	public async Task SaveCredentialsAsync(string email, string password)
	{
		await SecureStorage.Default.SetAsync(EmailKey, email);
		await SecureStorage.Default.SetAsync(PasswordKey, password);
	}

	public void ClearCredentials()
	{
		SecureStorage.Default.Remove(EmailKey);
		SecureStorage.Default.Remove(PasswordKey);
	}

	public async Task<(string? Email, string? Password)> LoadCredentialsAsync()
	{
		var email = await SecureStorage.Default.GetAsync(EmailKey);
		var password = await SecureStorage.Default.GetAsync(PasswordKey);
		return (email, password);
	}

	public bool HasSavedCredentials()
	{
		var task = LoadCredentialsAsync();
		task.Wait();
		return task.Result.Email is not null && task.Result.Password is not null;
	}

	public async Task<bool> TryAutoLoginAsync()
	{
		var (email, password) = await LoadCredentialsAsync();
		if (email is null || password is null)
			return false;

		return await LoginAsync(email, password);
	}
}
