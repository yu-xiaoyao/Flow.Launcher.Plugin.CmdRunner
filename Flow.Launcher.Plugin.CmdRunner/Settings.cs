using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Flow.Launcher.Plugin.CmdRunner;

public class Settings : BaseModel
{
    public Dictionary<string, string> InternalArguments { get; set; }
    public ObservableCollection<Command> Commands { get; set; } = new();
}