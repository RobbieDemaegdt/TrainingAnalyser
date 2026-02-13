using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Maui2.Models;
using Maui2.Services;

namespace Maui2.ViewModels;

public class CompletedTransferViewModel : INotifyPropertyChanged
{
	private readonly TransferService _transferService;

	private bool _isBusy;
	private string? _errorMessage;

	public CompletedTransferViewModel(TransferService transferService)
	{
		_transferService = transferService;

		RefreshCommand = new Command(async () => await LoadAsync());
		DeleteRowCommand = new Command<TransferRow>(async row => await DeleteRowAsync(row));
		DeleteAllCommand = new Command(async () => await DeleteAllAsync());
	}

	public ObservableCollection<TransferRow> CompletedTransfers { get; } = [];

	public ICommand RefreshCommand { get; }
	public ICommand DeleteRowCommand { get; }
	public ICommand DeleteAllCommand { get; }

	public bool IsBusy
	{
		get => _isBusy;
		set { _isBusy = value; OnPropertyChanged(); }
	}

	public string? ErrorMessage
	{
		get => _errorMessage;
		set { _errorMessage = value; OnPropertyChanged(); }
	}

	public async Task LoadAsync()
	{
		if (IsBusy)
			return;

		IsBusy = true;
		ErrorMessage = null;

		try
		{
			var completed = await _transferService.GetCompletedAsync();

			CompletedTransfers.Clear();
			foreach (var record in completed)
				CompletedTransfers.Add(TransferRow.FromRecord(record));
		}
		catch (Exception ex)
		{
			ErrorMessage = $"Failed to load completed transfers: {ex.Message}";
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task DeleteRowAsync(TransferRow row)
	{
		await _transferService.DeleteCompletedAsync(row.TransferId);
		CompletedTransfers.Remove(row);
	}

	private async Task DeleteAllAsync()
	{
		await _transferService.DeleteAllCompletedAsync();
		CompletedTransfers.Clear();
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
