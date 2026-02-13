using System.Text.Json;
using Maui2.Models;

namespace Maui2.Services;

public class TransferService
{
	private const string FileName = "transfer_records.json";

	private readonly AuthService _authService;
	private readonly PigeonService _pigeonService;

	private static readonly JsonSerializerOptions JsonOptions = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		WriteIndented = true
	};

	private List<TransferRecord>? _records;

	private string FilePath => Path.Combine(FileSystem.AppDataDirectory, FileName);

	public TransferService(AuthService authService, PigeonService pigeonService)
	{
		_authService = authService;
		_pigeonService = pigeonService;
	}

	public async Task<(List<TransferRecord> Active, List<TransferRecord> Completed)> RefreshAsync()
	{
		await _pigeonService.LoadTranslationsAsync();

		var stored = await LoadAsync();

		var response = await _authService.HttpClient.GetAsync(
			"https://www.pigeonfancier.com/api/transfer?processed=false");
		response.EnsureSuccessStatusCode();

		var json = await response.Content.ReadAsStringAsync();
		var currentItems = JsonSerializer.Deserialize<List<TransferItem>>(json, JsonOptions) ?? [];

		var currentIds = new HashSet<int>(currentItems.Select(t => t.Id));
		var changed = false;

		// Find transfers that disappeared from the active list
		var disappeared = stored
			.Where(r => r.Status == TransferStatus.Active && !currentIds.Contains(r.TransferId))
			.ToList();

		foreach (var record in disappeared)
		{
			await ResolveCompletedTransferAsync(record);
			changed = true;
		}

		// Update or add current active transfers
		foreach (var item in currentItems)
		{
			var existing = stored.FirstOrDefault(r => r.TransferId == item.Id);
			var name = _pigeonService.ResolveName(item.Pigeon.FirstNameId, item.Pigeon.LastNameId);

			if (existing is not null)
			{
				existing.CurrentPrice = item.Price;
				existing.Buyer = item.Buyer?.DisplayName;
				existing.BidCount = item.Bidders.Count;
				changed = true;
			}
			else
			{
				stored.Add(TransferRecord.FromTransferItem(item, name));
				changed = true;
			}
		}

		if (changed)
			await SaveAsync();

		var active = stored
			.Where(r => r.Status == TransferStatus.Active)
			.OrderBy(r => r.End)
			.ToList();

		var completed = stored
			.Where(r => r.Status != TransferStatus.Active)
			.OrderByDescending(r => r.End)
			.ToList();

		return (active, completed);
	}

	private async Task ResolveCompletedTransferAsync(TransferRecord record)
	{
		try
		{
			var response = await _authService.HttpClient.GetAsync(
				$"https://www.pigeonfancier.com/api/transfer?processed=true&pigeonId={record.PigeonId}");
			response.EnsureSuccessStatusCode();

			var json = await response.Content.ReadAsStringAsync();
			var history = JsonSerializer.Deserialize<List<TransferItem>>(json, JsonOptions) ?? [];

			// Match by transfer ID to find the exact listing
			var match = history.FirstOrDefault(h => h.Id == record.TransferId);

			if (match is not null && match.Buyer is not null && match.Price is not null)
			{
				record.Status = TransferStatus.Sold;
				record.SoldPrice = match.Price;
				record.SoldTo = match.Buyer.DisplayName;
				record.BidCount = match.Bidders.Count;
			}
			else
			{
				record.Status = TransferStatus.Expired;
			}
		}
		catch
		{
			// If we can't reach the API, mark as expired rather than leaving it active
			record.Status = TransferStatus.Expired;
		}
	}

	private async Task<List<TransferRecord>> LoadAsync()
	{
		if (_records is not null)
			return _records;

		if (!File.Exists(FilePath))
		{
			_records = [];
			return _records;
		}

		var json = await File.ReadAllTextAsync(FilePath);
		_records = JsonSerializer.Deserialize<List<TransferRecord>>(json, JsonOptions) ?? [];
		return _records;
	}

	public async Task<List<TransferRecord>> GetCompletedAsync()
	{
		var records = await LoadAsync();
		return records
			.Where(r => r.Status != TransferStatus.Active)
			.OrderByDescending(r => r.End)
			.ToList();
	}

	public async Task DeleteCompletedAsync(int transferId)
	{
		var records = await LoadAsync();
		var record = records.FirstOrDefault(r => r.TransferId == transferId && r.Status != TransferStatus.Active);
		if (record is not null)
		{
			records.Remove(record);
			await SaveAsync();
		}
	}

	public async Task DeleteAllCompletedAsync()
	{
		var records = await LoadAsync();
		records.RemoveAll(r => r.Status != TransferStatus.Active);
		await SaveAsync();
	}

	private async Task SaveAsync()
	{
		var json = JsonSerializer.Serialize(_records, JsonOptions);
		await File.WriteAllTextAsync(FilePath, json);
	}
}
