using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Serilog;
using System;
using System.ComponentModel;
using WSE.Models;

namespace WSE.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        GlobalState.Instance.GameSaves = GameSaveUtil.GetGameSaves();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);

        Log.CloseAndFlush();
    }
}