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
}