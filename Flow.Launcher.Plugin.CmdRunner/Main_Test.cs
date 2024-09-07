using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Flow.Launcher.Plugin.CmdRunner;

public class Main_Test
{
    public static void Main()
    {
        // read_json_test();
        // test_regx_name();
        test_run_cmd();
    }

    private static void test_run_cmd()
    {
        string cmd = "";
        Process process = new Process();
        process.StartInfo.WorkingDirectory = @"C:\App\VLC\";
        process.StartInfo.FileName = "vlc.exe";
        process.StartInfo.Arguments = "http://127.0.0.1:8080/live/test.live.flv";
        process.StartInfo.UseShellExecute = false; //是否使用操作系统shell启动 
        process.StartInfo.CreateNoWindow = false; //是否在新窗口中启动该进程的值 (不显示程序窗口)
        process.Start();
        // process.WaitForExit(); //等待程序执行完退出进程
        process.Close();
        Console.WriteLine("CLOSE___END");
    }

    private static void read_json_test()
    {
        var path = @"C:\Users\KerryMiBook\Desktop\Settings.json";
        Settings settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText(path));
        foreach (var command in settings.Commands)
        {
            // update
            // command.Cmd = "format ${codec}";
            Console.WriteLine(
                $"Cmd: {command.Name} - {command.Description} - cmd: {command.Arguments} - {command.ArgumentList.Count}");
            var arguments = command.ArgumentList;
            foreach (var argument in arguments)
            {
                Console.WriteLine($"  --> arg: {argument.ArgsIndex} - {argument.ArgsName}");
            }
        }
    }

    private static void test_regx_name()
    {
        var template = "-format ${format}  -code ${code}";
        var mc = Regex.Matches(template, "\\${.*?}");

        var count = mc.Count;
        Console.WriteLine($"count = {mc.Count}");

        for (var i = 0; i < count; i++)
        {
            var g = mc[i];
            Console.WriteLine(g);
            Console.WriteLine(g.Value);
        }
    }
}