using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using System.IO;
using WSE.Models;

namespace WSE.Views
{
    public partial class EditorPageView : UserControl
    {
        public EditorPageView()
        {
            InitializeComponent();
        }

        public void OnSaveClicked(object? sender, RoutedEventArgs args)
        {
            string save = GameSaveUtil.ExportSaveFile(GlobalState.Instance.SelectedGameSave);

            SaveFileDialog saveFileBox = new SaveFileDialog();
            saveFileBox.Title = "Save as...";
            saveFileBox.InitialFileName = "export.save";
            saveFileBox.Directory = GlobalState.Instance.SaveSearchDir;

            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                saveFileBox.ShowAsync(desktop.MainWindow).ContinueWith(t =>
                {
                    var path = t.Result;
                    if (path != null)
                    {
                        File.WriteAllText(path, save);
                    }
                });
            }
        }
    }
}
