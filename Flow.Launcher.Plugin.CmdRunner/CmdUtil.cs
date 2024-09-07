using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Flow.Launcher.Plugin.CmdRunner;

public class ExecuteParamWrapper
{
    public Command Command { get; set; }
    public List<string> InputParams { get; set; }

    public ExecuteParamWrapper(Command command, List<string> inputParams)
    {
        Command = command;
        InputParams = new List<string>(inputParams);
    }
}

public class CmdUtil
{
    public static void ExecuteCmd(Command command, List<string> paramList, bool waitForExit = false)
    {
        var process = new Process();
        if (!string.IsNullOrEmpty(command.WorkingDirectory))
            process.StartInfo.WorkingDirectory = command.WorkingDirectory;
        process.StartInfo.FileName = command.Path;

        var arguments = FillArguments(command.Arguments, command.ArgumentList, paramList);

        if (!string.IsNullOrEmpty(arguments))
            process.StartInfo.Arguments = arguments;

        process.StartInfo.UseShellExecute = false; //是否使用操作系统shell启动 
        process.StartInfo.CreateNoWindow = false; //是否在新窗口中启动该进程的值 (不显示程序窗口)
        process.Start();

        if (waitForExit)
            process.WaitForExit(); //等待程序执行完退出进程

        process.Close();
    }

    public static string FillArguments(string arguments, List<Argument> argumentList, List<string> paramList)
    {
        for (var i = 0; i < argumentList.Count; i++)
        {
            var paramItem = argumentList[i];
            var paramValue = paramList[i];
            arguments = arguments.Replace(paramItem.ArgsName, paramValue);
        }

        return arguments;
    }
}