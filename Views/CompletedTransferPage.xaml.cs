using Maui2.ViewModels;

namespace Maui2.Views;

public partial class CompletedTransferPage : ContentPage
{
	private readonly CompletedTransferViewModel _viewModel;

	public CompletedTransferPage(CompletedTransferViewModel viewModel)
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
