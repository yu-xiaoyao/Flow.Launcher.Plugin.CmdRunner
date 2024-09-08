using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace Flow.Launcher.Plugin.CmdRunner;

public delegate void OnCmdAdd(Command command);

public delegate void OnCmdUpdate(Command command, Command oldCommand, int oldIndex);

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
    protected int updateIndex = -1;
    private Command _udpateCommand;

    public CmdRunnerWindows(OnCmdUpdate onCmdUpdate, Command command, int index)
    {
        _isUpdate = true;
        _onCmdUpdate = onCmdUpdate;
        _udpateCommand = command;
        updateIndex = index;
        InitializeComponent();
    }

    private void Btn_Save(object sender, RoutedEventArgs e)
    {
        var command = new Command();
        if (_isUpdate)
            _onCmdUpdate?.Invoke(command, _udpateCommand, updateIndex);
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
        throw new System.NotImplementedException();
    }

    private void Btn_Select_Working_Dir(object sender, RoutedEventArgs e)
    {
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