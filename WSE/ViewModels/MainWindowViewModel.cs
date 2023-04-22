using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace WSE.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        // Set current page to first on startup
        _CurrentPage = Pages[0];

        // Create observables which will activate to deactivate our commands based on CurrentPage state
        var canNavNext = this.WhenAnyValue(x => x.CurrentPage.CanNavigateNext);
        var canNavPrev = this.WhenAnyValue(x => x.CurrentPage.CanNavigatePrevious);

        // Create commands to bind too
        NavigateEditorCommand = ReactiveCommand.Create(NavigateEditor);
        NavigateSettingsCommand = ReactiveCommand.Create(NavigateSettings);
    }

    private List<PageViewModelBase> _Pages = new()
    {
        new EditorViewModel(),
        new SettingsViewModel(),
    };

    public List<PageViewModelBase> Pages
    {
        get { return _Pages; }
        private set { _Pages = value; }
    }

    // Currently selected page to display
    private PageViewModelBase _CurrentPage;

    /// <summary>
    /// Gets the currently selected page
    /// </summary>
    public PageViewModelBase CurrentPage
    {
        get { return _CurrentPage; }
        private set { this.RaiseAndSetIfChanged(ref _CurrentPage, value); }
    }

    public ICommand NavigateEditorCommand { get; }

    private void NavigateEditor()
    {
        CurrentPage = Pages[0];
    }

    public ICommand NavigateSettingsCommand { get; }

    private void NavigateSettings()
    {
        CurrentPage = Pages[1];
    }

}
