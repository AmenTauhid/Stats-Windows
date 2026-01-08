using Microsoft.Extensions.DependencyInjection;

namespace Stats.App.Views;

public partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
    }

    public MainViewModel ViewModel => App.Services.GetRequiredService<MainViewModel>();
}
