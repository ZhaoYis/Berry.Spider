using Avalonia.Controls;
using Berry.Spider.AIGen.ViewModels;

namespace Berry.Spider.AIGen.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel mainWindowViewModel)
    {
        DataContext = mainWindowViewModel;
        InitializeComponent();
    }
}