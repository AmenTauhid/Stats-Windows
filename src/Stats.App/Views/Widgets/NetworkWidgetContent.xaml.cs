using Microsoft.UI.Xaml.Controls;
using Stats.App.ViewModels;

namespace Stats.App.Views.Widgets;

public sealed partial class NetworkWidgetContent : UserControl
{
    public MainViewModel ViewModel { get; }

    public NetworkWidgetContent(MainViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
    }
}
