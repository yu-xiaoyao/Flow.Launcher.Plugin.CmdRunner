using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Flow.Launcher.Plugin.CmdRunner;

public partial class Command
{
    public string Name { get; set; }
    public string Description { get; set; } = "";
    public string WorkingDirectory { get; set; } = "";
    public string Path { get; set; } = "";

    private string arguments;

    public string Arguments
    {
        get => arguments;
        set
        {
            arguments = value;
            ArgumentNames = _resolveArgumentNames(arguments);
        }
    }

    [JsonIgnore] public List<string> ArgumentNames { get; private set; }


    [GeneratedRegex("\\${.*?}")]
    private static partial Regex ArgumentRegex();

    public void ResolveArgumentNames()
    {
        ArgumentNames = _resolveArgumentNames(arguments);
    }

    private static List<string> _resolveArgumentNames(string cmd)
    {
        var list = new List<string>();
        if (string.IsNullOrEmpty(cmd))
            return list;
        var mc = ArgumentRegex().Matches(cmd);

        var count = mc.Count;
        for (var i = 0; i < count; i++)
        {
            var match = mc[i];
            // var mv = match.Value.Trim();
            // var name = mv.Substring(2, mv.Length - 3);
            var name = match.Value.Trim();
            list.Add(name);
        }

        return list;
    }
}