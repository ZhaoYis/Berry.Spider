using System.Collections.Generic;
using System.Windows.Input;

namespace Berry.Spider.AIGenPlus.ViewModels;

public class MenuItemViewModel
{
    public string? Header { get; set; }
    public ICommand? Command { get; set; }
    public object? CommandParameter { get; set; }
    public IList<MenuItemViewModel>? Items { get; set; }
}