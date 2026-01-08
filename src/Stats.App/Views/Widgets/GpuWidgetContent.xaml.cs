using Microsoft.UI.Xaml.Controls;
using Stats.App.ViewModels;

namespace Stats.App.Views.Widgets;

public sealed partial class GpuWidgetContent : UserControl
{
    public MainViewModel ViewModel { get; }

    public GpuWidgetContent(MainViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
    }
}
