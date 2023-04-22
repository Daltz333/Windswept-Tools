using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Serilog;
using System;
using System.ComponentModel;

namespace WSE.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);

        Log.CloseAndFlush();
    }
}