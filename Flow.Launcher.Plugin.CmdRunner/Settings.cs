using System.Collections.ObjectModel;

namespace Flow.Launcher.Plugin.CmdRunner;

public class Settings : BaseModel
{
    public Settings()
    {
    }

    public ObservableCollection<Command> Commands { get; set; } = new();
}