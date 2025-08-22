using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.Input;
using Semi.Avalonia;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.ToolkitStore.ViewModels;

public partial class MainViewModel : ViewModelBase, ITransientDependency
{
    public string DocumentationUrl => "https://docs.irihi.tech/semi";
    public string RepoUrl => "https://github.com/irihitech/Semi.Avalonia";
    public IReadOnlyList<MenuItemViewModel> MenuItems { get; }

    public MainViewModel()
    {
        MenuItems =
        [
            new MenuItemViewModel
            {
                Header = "High Contrast Theme",
                Items =
                [
                    new MenuItemViewModel
                    {
                        Header = "Aquatic",
                        Command = SelectThemeCommand,
                        CommandParameter = SemiTheme.Aquatic
                    },
                    new MenuItemViewModel
                    {
                        Header = "Desert",
                        Command = SelectThemeCommand,
                        CommandParameter = SemiTheme.Desert
                    },
                    new MenuItemViewModel
                    {
                        Header = "NightSky",
                        Command = SelectThemeCommand,
                        CommandParameter = SemiTheme.NightSky
                    },
                ]
            }
        ];
    }

    [RelayCommand]
    private void ToggleTheme()
    {
        var app = App.Current;
        var theme = app.ActualThemeVariant;
        app.RequestedThemeVariant = theme == ThemeVariant.Dark ? ThemeVariant.Light : ThemeVariant.Dark;
    }

    [RelayCommand]
    private void SelectTheme(object? obj)
    {
        var app = App.Current;
        app.RequestedThemeVariant = obj as ThemeVariant;
    }

    [RelayCommand]
    private async Task OpenUrlAsync(string url)
    {
        var launcher = ResolveDefaultTopLevel()?.Launcher;
        if (launcher is not null)
        {
            await launcher.LaunchUriAsync(new Uri(url));
        }
    }

    private static TopLevel? ResolveDefaultTopLevel()
    {
        return App.Current.ApplicationLifetime switch
        {
            IClassicDesktopStyleApplicationLifetime desktopLifetime => desktopLifetime.MainWindow,
            ISingleViewApplicationLifetime singleView => TopLevel.GetTopLevel(singleView.MainView),
            _ => null
        };
    }
}