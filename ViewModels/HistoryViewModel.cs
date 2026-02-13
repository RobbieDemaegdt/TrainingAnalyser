using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Maui2.Models;
using Maui2.Services;

namespace Maui2.ViewModels;

public class HistoryViewModel : INotifyPropertyChanged
{
	private readonly HistoryService _historyService;

	private PigeonHistory? _selectedPigeon;
	private bool _isBusy;

	public HistoryViewModel(HistoryService historyService)
	{
		_historyService = historyService;

		DeleteRowCommand = new Command<HistoryRow>(async row => await DeleteRowAsync(row));
		DeletePigeonCommand = new Command(async () => await DeletePigeonAsync(), () => SelectedPigeon is not null);
		DeleteAllCommand = new Command(async () => await DeleteAllAsync());
		RefreshCommand = new Command(async () => await LoadAsync());
	}

	public ObservableCollection<PigeonHistory> Pigeons { get; } = [];
	public ObservableCollection<HistoryRow> Snapshots { get; } = [];

	public ICommand DeleteRowCommand { get; }
	public ICommand DeletePigeonCommand { get; }
	public ICommand DeleteAllCommand { get; }
	public ICommand RefreshCommand { get; }

	public PigeonHistory? SelectedPigeon
	{
		get => _selectedPigeon;
		set
		{
			_selectedPigeon = value;
			OnPropertyChanged();
			UpdateSnapshots();
			((Command)DeletePigeonCommand).ChangeCanExecute();
		}
	}

	public bool IsBusy
	{
		get => _isBusy;
		set { _isBusy = value; OnPropertyChanged(); }
	}

	public async Task LoadAsync()
	{
		IsBusy = true;

		try
		{
			var histories = await _historyService.GetAllHistoriesAsync();

			Pigeons.Clear();
			foreach (var h in histories.OrderBy(h => h.Name))
				Pigeons.Add(h);

			if (SelectedPigeon is null && Pigeons.Count > 0)
				SelectedPigeon = Pigeons[0];
			else
				UpdateSnapshots();
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task DeleteRowAsync(HistoryRow row)
	{
		if (_selectedPigeon is null)
			return;

		await _historyService.DeleteSnapshotAsync(_selectedPigeon.PigeonId, row.TotalMonths);
		var match = _selectedPigeon.Snapshots.FirstOrDefault(s => s.TotalMonths == row.TotalMonths);
		if (match is not null)
			_selectedPigeon.Snapshots.Remove(match);
		UpdateSnapshots();
	}

	private async Task DeletePigeonAsync()
	{
		if (_selectedPigeon is null)
			return;

		var pigeon = _selectedPigeon;
		await _historyService.DeletePigeonHistoryAsync(pigeon.PigeonId);
		Pigeons.Remove(pigeon);
		SelectedPigeon = Pigeons.Count > 0 ? Pigeons[0] : null;
	}

	private async Task DeleteAllAsync()
	{
		await _historyService.DeleteAllAsync();
		Pigeons.Clear();
		Snapshots.Clear();
		SelectedPigeon = null;
	}

	private void UpdateSnapshots()
	{
		Snapshots.Clear();

		if (_selectedPigeon?.Snapshots is not { Count: > 0 })
			return;

		foreach (var row in HistoryRow.FromSnapshots(_selectedPigeon.Snapshots))
			Snapshots.Add(row);
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
