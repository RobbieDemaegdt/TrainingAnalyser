using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Maui2.Models;
using Maui2.Services;

namespace Maui2.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
	private readonly AuthService _authService;
	private readonly PigeonService _pigeonService;
	private readonly HistoryService _historyService;

	private bool _isBusy;
	private bool _isLoaded;
	private string? _errorMessage;

	public MainViewModel(AuthService authService, PigeonService pigeonService, HistoryService historyService)
	{
		_authService = authService;
		_pigeonService = pigeonService;
		_historyService = historyService;
		_authService.LoggedIn += async () => await LoadAsync();

		RefreshCommand = new Command(async () => await RefreshAsync());
	}

	public ObservableCollection<PigeonRow> Pigeons { get; } = [];

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
		if (_isLoaded || !_authService.IsLoggedIn || IsBusy)
			return;

		await FetchPigeonsAsync();
		_isLoaded = true;
	}

	private async Task RefreshAsync()
	{
		if (!_authService.IsLoggedIn || IsBusy)
			return;

		await FetchPigeonsAsync();
	}

	private async Task FetchPigeonsAsync()
	{
		IsBusy = true;
		ErrorMessage = null;

		try
		{
			var rows = await _pigeonService.GetPigeonsAsync();
			Pigeons.Clear();
			foreach (var row in rows)
				Pigeons.Add(row);

			await _historyService.TrackPigeonsAsync(
				_pigeonService.LastFetchedPigeons,
				p => _pigeonService.ResolveName(p.FirstNameId, p.LastNameId));
		}
		catch (Exception ex)
		{
			ErrorMessage = $"Failed to load pigeons: {ex.Message}";
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
