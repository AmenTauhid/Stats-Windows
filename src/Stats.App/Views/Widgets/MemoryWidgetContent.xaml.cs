using Microsoft.UI.Xaml.Controls;
using Stats.App.ViewModels;

namespace Stats.App.Views.Widgets;

public sealed partial class MemoryWidgetContent : UserControl
{
    public MainViewModel ViewModel { get; }

    public MemoryWidgetContent(MainViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
    }
}
