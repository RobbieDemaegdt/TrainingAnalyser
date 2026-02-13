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

			var currentSnapshot = PigeonSnapshot.FromPigeon(pigeon);

				var existing = history.Snapshots
					.FirstOrDefault(s => s.TotalMonths == currentSnapshot.TotalMonths);

				if (existing is not null)
				{
					if (!existing.HasSameStats(currentSnapshot))
					{
						history.Snapshots.Remove(existing);
						history.Snapshots.Add(currentSnapshot);
						changed = true;
					}
				}
				else
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

	public async Task DeleteSnapshotAsync(int pigeonId, int totalMonths)
	{
		var histories = await LoadAsync();
		var history = histories.FirstOrDefault(h => h.PigeonId == pigeonId);
		var snapshot = history?.Snapshots.FirstOrDefault(s => s.TotalMonths == totalMonths);
		if (snapshot is not null)
		{
			history!.Snapshots.Remove(snapshot);
			await SaveAsync();
		}
	}

	public async Task DeletePigeonHistoryAsync(int pigeonId)
	{
		var histories = await LoadAsync();
		var history = histories.FirstOrDefault(h => h.PigeonId == pigeonId);
		if (history is not null)
		{
			histories.Remove(history);
			await SaveAsync();
		}
	}

	public async Task DeleteAllAsync()
	{
		var histories = await LoadAsync();
		histories.Clear();
		await SaveAsync();
	}

	private async Task SaveAsync()
	{
		var json = JsonSerializer.Serialize(_histories, JsonOptions);
		await File.WriteAllTextAsync(FilePath, json);
	}
}
