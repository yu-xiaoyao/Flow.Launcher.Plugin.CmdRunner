using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Flow.Launcher.Plugin.CmdRunner;

public class CmdUtil
{
    public static void ExecuteCmd(Command command, List<string> paramList,
        Dictionary<string, string> internalArguments,
        bool waitForExit = false)
    {
        var process = new Process();
        if (!string.IsNullOrEmpty(command.WorkingDirectory))
            process.StartInfo.WorkingDirectory = command.WorkingDirectory;
        process.StartInfo.FileName = command.Path;

        var arguments = FillArguments(command.Arguments, command.ArgumentNames, paramList, internalArguments);

        if (!string.IsNullOrEmpty(arguments))
            process.StartInfo.Arguments = arguments;

        process.StartInfo.UseShellExecute = false; //是否使用操作系统shell启动 
        process.StartInfo.CreateNoWindow = false; //是否在新窗口中启动该进程的值 (不显示程序窗口)
        process.Start();

        if (waitForExit)
            process.WaitForExit(); //等待程序执行完退出进程

        process.Close();
    }

    public static string FillArguments(string arguments, List<string> argumentNames, List<string> paramList,
        Dictionary<string, string> internalArguments)
    {
        for (var i = 0; i < argumentNames.Count; i++)
        {
            var paramItem = argumentNames[i];
            var paramValue = paramList[i];
            arguments = arguments.Replace(paramItem, paramValue);
        }

        foreach (var kv in internalArguments)
        {
            arguments = arguments.Replace(kv.Key, kv.Value);
        }

        return arguments;
    }


    public static Dictionary<string, string> GetDefaultInternalArguments()
    {
        var dict = new Dictionary<string, string>();

        dict.Add("${desktop}", Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
        dict.Add("${programFiles}", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
        dict.Add("${applicationData}", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
        dict.Add("${localApplicationData}", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));

        return dict;
    }
}