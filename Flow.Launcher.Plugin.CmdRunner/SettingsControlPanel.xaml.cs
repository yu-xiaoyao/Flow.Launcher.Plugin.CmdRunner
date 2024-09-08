using System;
using System.Windows;
using System.Windows.Controls;

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
    }

    private void Btn_Add_Cmd(object sender, RoutedEventArgs e)
    {
        var totpAdd = new CmdRunnerWindows(addCmd =>
        {
            Console.WriteLine(addCmd);
        })
        {
            Title = "添加命令(Add Cmd)",
            Topmost = true,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            ResizeMode = ResizeMode.NoResize,
            ShowInTaskbar = false,
        };
        totpAdd.ShowDialog();
    }
}