using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Maui2.Models;
using Maui2.Services;

namespace Maui2.ViewModels;

public class TransferViewModel : INotifyPropertyChanged
{
	private readonly AuthService _authService;
	private readonly TransferService _transferService;

	private bool _isBusy;
	private string? _errorMessage;

	public TransferViewModel(AuthService authService, TransferService transferService)
	{
		_authService = authService;
		_transferService = transferService;

		RefreshCommand = new Command(async () => await LoadAsync());
	}

	public ObservableCollection<TransferRow> Transfers { get; } = [];

	public ICommand RefreshCommand { get; }

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
		if (!_authService.IsLoggedIn || IsBusy)
			return;

		IsBusy = true;
		ErrorMessage = null;

		try
		{
			var (active, _) = await _transferService.RefreshAsync();

			Transfers.Clear();
			foreach (var record in active)
				Transfers.Add(TransferRow.FromRecord(record));
		}
		catch (Exception ex)
		{
			ErrorMessage = $"Failed to load transfers: {ex.Message}";
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
