using Maui2.ViewModels;

namespace Maui2.Views;

public partial class TransferPage : ContentPage
{
	private readonly TransferViewModel _viewModel;

	public TransferPage(TransferViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await _viewModel.LoadAsync();
	}
}
