using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Flow.Launcher.Plugin.CmdRunner;

public partial class SettingsControlPanel : UserControl
{
    private PluginInitContext _context;
    private Settings _settings;

    public SettingsControlPanel(PluginInitContext context, Settings settings)
    {
        _context = context;
        _settings = settings;
        InitializeComponent();
        InitSettingData();
    }

    private void InitSettingData()
    {
        CmdDataGrid.IsReadOnly = true;
        CmdDataGrid.ItemsSource = _settings.Commands;
    }

    private void Save_Settings(object sender, RoutedEventArgs e)
    {
        _context.API.SavePluginSettings();
    }

    private void Btn_Add_Cmd(object sender, RoutedEventArgs e)
    {
        var totpAdd = new CmdRunnerWindows(addCmd => { _settings.Commands.Add(addCmd); })
        {
            Title = "添加命令(Add Cmd)",
            Topmost = true,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            ResizeMode = ResizeMode.NoResize,
            ShowInTaskbar = false,
        };
        totpAdd.ShowDialog();
    }


    private void CmdDataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        var dg = (sender as DataGrid)!;

        var index = dg.SelectedIndex;

        if (index < 0) return;

        var item = dg.SelectedItem;
        if (item is not Command command) return;

        var totpAdd = new CmdRunnerWindows((newCmd, oldIndex) =>
            {
                if (oldIndex != -1)
                {
                    _settings.Commands[oldIndex] = newCmd;
                }
            },
            command, index)
        {
            Title = "编辑命令(Edit Cmd)",
            Topmost = true,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            ResizeMode = ResizeMode.NoResize,
            ShowInTaskbar = false,
        };
        totpAdd.ShowDialog();
    }
}