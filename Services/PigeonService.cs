using System.Text.Json;
using Maui2.Models;

namespace Maui2.Services;

public class PigeonService
{
	private readonly AuthService _authService;
	private Dictionary<string, string>? _firstNames;
	private Dictionary<string, string>? _lastNames;

	public PigeonService(AuthService authService)
	{
		_authService = authService;
	}

	public async Task LoadTranslationsAsync()
	{
		if (_firstNames is not null && _lastNames is not null)
			return;

		var response = await _authService.HttpClient.GetAsync(
			"https://www.pigeonfancier.com/api/translation/nl");
		response.EnsureSuccessStatusCode();

		var json = await response.Content.ReadAsStringAsync();
		using var doc = JsonDocument.Parse(json);

		_firstNames = ParseStringDictionary(doc.RootElement.GetProperty("first-name"));
		_lastNames = ParseStringDictionary(doc.RootElement.GetProperty("last-name"));
	}

	public List<Pigeon> LastFetchedPigeons { get; private set; } = [];

	public async Task<List<PigeonRow>> GetPigeonsAsync()
	{
		if (_firstNames is null || _lastNames is null)
			await LoadTranslationsAsync();

		var response = await _authService.HttpClient.GetAsync(
			"https://www.pigeonfancier.com/api/pigeon");
		response.EnsureSuccessStatusCode();

		var json = await response.Content.ReadAsStringAsync();
		var pigeons = JsonSerializer.Deserialize<List<Pigeon>>(json, new JsonSerializerOptions
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase
		}) ?? [];

		LastFetchedPigeons = pigeons;

		return pigeons
			.Select(p => PigeonRow.FromPigeon(p, ResolveName(p.FirstNameId, p.LastNameId)))
			.ToList();
	}

	public string ResolveName(int firstNameId, int lastNameId)
	{
		var firstName = _firstNames?.GetValueOrDefault(firstNameId.ToString()) ?? $"#{firstNameId}";
		var lastName = _lastNames?.GetValueOrDefault(lastNameId.ToString()) ?? $"#{lastNameId}";
		return $"{firstName} {lastName}";
	}

	private static Dictionary<string, string> ParseStringDictionary(JsonElement element)
	{
		var dict = new Dictionary<string, string>();
		foreach (var prop in element.EnumerateObject())
		{
			if (prop.Value.ValueKind == JsonValueKind.String)
				dict[prop.Name] = prop.Value.GetString()!;
		}
		return dict;
	}
}
