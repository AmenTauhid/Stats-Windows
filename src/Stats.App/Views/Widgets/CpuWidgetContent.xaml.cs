using Microsoft.UI.Xaml.Controls;
using Stats.App.ViewModels;

namespace Stats.App.Views.Widgets;

public sealed partial class CpuWidgetContent : UserControl
{
    public MainViewModel ViewModel { get; }

    public CpuWidgetContent(MainViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
    }
}
