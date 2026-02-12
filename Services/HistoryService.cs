using System.Text.Json;
using Maui2.Models;

namespace Maui2.Services;

public class HistoryService
{
	private const string FileName = "pigeon_history.json";

	private static readonly JsonSerializerOptions JsonOptions = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		WriteIndented = true
	};

	private List<PigeonHistory>? _histories;

	private string FilePath => Path.Combine(FileSystem.AppDataDirectory, FileName);

	public async Task<List<PigeonHistory>> LoadAsync()
	{
		if (_histories is not null)
			return _histories;

		if (!File.Exists(FilePath))
		{
			_histories = [];
			return _histories;
		}

		var json = await File.ReadAllTextAsync(FilePath);
		_histories = JsonSerializer.Deserialize<List<PigeonHistory>>(json, JsonOptions) ?? [];
		return _histories;
	}

	public async Task TrackPigeonsAsync(List<Pigeon> pigeons, Func<Pigeon, string> nameResolver)
	{
		var histories = await LoadAsync();
		var changed = false;

		foreach (var pigeon in pigeons)
		{
			var history = histories.FirstOrDefault(h => h.PigeonId == pigeon.Id);
			if (history is null)
			{
				history = new PigeonHistory
				{
					PigeonId = pigeon.Id,
					Name = nameResolver(pigeon)
				};
				histories.Add(history);
			}

			history.Name = nameResolver(pigeon);

			var latestSnapshot = history.Snapshots
				.OrderByDescending(s => s.TotalMonths)
				.FirstOrDefault();

			var currentSnapshot = PigeonSnapshot.FromPigeon(pigeon);

			if (latestSnapshot is null
				|| latestSnapshot.TotalMonths != currentSnapshot.TotalMonths
				|| !latestSnapshot.HasSameStats(currentSnapshot))
			{
				history.Snapshots.Add(currentSnapshot);
				changed = true;
			}
		}

		if (changed)
			await SaveAsync();
	}

	public async Task<List<PigeonHistory>> GetAllHistoriesAsync()
	{
		return await LoadAsync();
	}

	private async Task SaveAsync()
	{
		var json = JsonSerializer.Serialize(_histories, JsonOptions);
		await File.WriteAllTextAsync(FilePath, json);
	}
}
