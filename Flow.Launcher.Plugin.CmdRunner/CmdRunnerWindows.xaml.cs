using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace Flow.Launcher.Plugin.CmdRunner;

public delegate void OnCmdAdd(Command command);

public delegate void OnCmdUpdate(Command command, int oldIndex);

public partial class CmdRunnerWindows : Window
{
    private bool _isUpdate;

    private OnCmdAdd _onCmdAdd;

    public CmdRunnerWindows(OnCmdAdd onCmdAdd)
    {
        _isUpdate = false;
        _onCmdAdd = onCmdAdd;
        InitializeComponent();
    }

    private OnCmdUpdate _onCmdUpdate;
    private int updateIndex = -1;

    public CmdRunnerWindows(OnCmdUpdate onCmdUpdate, Command command, int index)
    {
        _isUpdate = true;
        _onCmdUpdate = onCmdUpdate;
        updateIndex = index;
        InitializeComponent();
        _renderInit(command);
    }

    private void _renderInit(Command command)
    {
        TbName.Text = command.Name;
        TbDescription.Text = command.Description;
        TbPath.Text = command.Path;
        TbWorkingDirectory.Text = command.WorkingDirectory;
        TbArguments.Text = command.Arguments;
    }

    private void Btn_Save(object sender, RoutedEventArgs e)
    {
        TbTip.Text = "";

        var name = TbName.Text.Trim();
        if (string.IsNullOrEmpty(name))
        {
            TbTip.Text = "名称不能为空（Name cannot be empty）";
            return;
        }

        var path = TbPath.Text.Trim();
        if (string.IsNullOrEmpty(path))
        {
            TbTip.Text = "路径不能为空（Path cannot be empty）";
            return;
        }

        var command = new Command()
        {
            Name = name,
            Description = TbDescription.Text.Trim(),
            Path = path,
            WorkingDirectory = TbWorkingDirectory.Text.Trim(),
            Arguments = TbArguments.Text.Trim(),
        };
        if (_isUpdate)
            _onCmdUpdate?.Invoke(command, updateIndex);
        else
            _onCmdAdd?.Invoke(command);
        Close();
    }

    private void Btn_Cancel(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Btn_View_Inner_Arguments(object sender, RoutedEventArgs e)
    {
        //TODO 
    }

    private void Btn_Select_Working_Dir(object sender, RoutedEventArgs e)
    {
        //TODO
    }

    private void Btn_Select_Path(object sender, RoutedEventArgs e)
    {
        var ofd = new OpenFileDialog
        {
            Filter = "Execute Files |*.exe;*.bat;*.sys;*.msi;*.sh|All Files (*.*)|*.*",
            ShowReadOnly = true,
            Multiselect = false
        };

        var showDialog = ofd.ShowDialog();
        if (showDialog is not true) return;

        var fileName = ofd.FileName;
        if (!File.Exists(fileName)) return;

        TbPath.Text = fileName;
    }
}