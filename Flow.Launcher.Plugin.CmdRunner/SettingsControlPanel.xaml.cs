using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
        InitSettingData();
    }

    private void InitSettingData()
    {
        CmdDataGrid.IsReadOnly = true;
        CmdDataGrid.ItemsSource = _settings.Commands;

        AddContextMenu();
    }


    private void AddContextMenu()
    {
        var updateItem = new MenuItem
        {
            Header = "更新(Update)",
        };
        updateItem.Click += (o, args) =>
        {
            if (CmdDataGrid.SelectedIndex == -1) return;
            var item = CmdDataGrid.SelectedItem;
            if (item is Command command)
            {
                _openUpdateDialogAndUpdate(command, CmdDataGrid.SelectedIndex);
            }
        };
        var deleteItem = new MenuItem
        {
            Header = "删除(Delete)",
        };
        deleteItem.Click += (o, args) =>
        {
            if (CmdDataGrid.SelectedIndex == -1) return;
            var item = CmdDataGrid.SelectedItem;
            if (item is Command param)
            {
                _settings.Commands.RemoveAt(CmdDataGrid.SelectedIndex);
            }
        };

        var moveUp = new MenuItem
        {
            Header = "向上移动(Move Up)",
        };
        moveUp.Click += (o, args) =>
        {
            var index = CmdDataGrid.SelectedIndex;
            if (index <= 0) return;

            var cur = _settings.Commands[index];
            var pre = _settings.Commands[index - 1];

            _settings.Commands[index] = pre;
            _settings.Commands[index - 1] = cur;
        };
        var moveDown = new MenuItem
        {
            Header = "向下移动(Move Down)",
        };
        moveDown.Click += (o, args) =>
        {
            var index = CmdDataGrid.SelectedIndex;
            if (index < _settings.Commands.Count - 1)
            {
                var cur = _settings.Commands[index];
                var next = _settings.Commands[index + 1];

                _settings.Commands[index] = next;
                _settings.Commands[index + 1] = cur;
            }
        };

        CmdDataGrid.ContextMenu = new ContextMenu
        {
            Items =
            {
                updateItem,
                deleteItem,
                moveUp,
                moveDown,
            },
            StaysOpen = true
        };
    }


    private void Save_Settings(object sender, RoutedEventArgs e)
    {
        _context.API.SavePluginSettings();
    }

    private void Btn_Add_Cmd(object sender, RoutedEventArgs e)
    {
        var totpAdd = new CmdRunnerWindows(addCmd =>
        {
            addCmd.ResolveArgumentNames();
            _settings.Commands.Add(addCmd);
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


    private void CmdDataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        var dg = (sender as DataGrid)!;

        var index = dg.SelectedIndex;

        if (index < 0) return;

        var item = dg.SelectedItem;
        if (item is not Command command) return;

        _openUpdateDialogAndUpdate(command, index);
    }

    private void _openUpdateDialogAndUpdate(Command command, int index)
    {
        var totpAdd = new CmdRunnerWindows((newCmd, oldIndex) =>
            {
                if (oldIndex == -1) return;

                newCmd.ResolveArgumentNames();
                _settings.Commands[oldIndex] = newCmd;
            },
            command, index)
        {
            Title = "编辑命令(Edit Cmd)",
            Topmost = true,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            ResizeMode = ResizeMode.NoResize,
            ShowInTaskbar = false,
        };
        totpAdd.ShowDialog();
    }
}