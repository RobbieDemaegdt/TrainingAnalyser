using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
	}

	public ObservableCollection<PigeonHistory> Pigeons { get; } = [];
	public ObservableCollection<HistoryRow> Snapshots { get; } = [];

	public PigeonHistory? SelectedPigeon
	{
		get => _selectedPigeon;
		set
		{
			_selectedPigeon = value;
			OnPropertyChanged();
			UpdateSnapshots();
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
