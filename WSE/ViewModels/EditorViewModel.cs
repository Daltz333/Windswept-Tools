using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WSE.Models;

namespace WSE.ViewModels
{
    public class EditorViewModel : PageViewModelBase
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public EditorViewModel() {
            LoadSavesCommand = ReactiveCommand.CreateFromTask(LoadSaves);
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public static string Title => "Editor";

        public override bool CanNavigateNext
        {
            get => true;
            protected set => throw new NotSupportedException();
        }

        public override bool CanNavigatePrevious
        {
            get => false;
            protected set => throw new NotSupportedException();
        }

        private ICommand _LoadSavesCommand;

        public ICommand LoadSavesCommand
        {
            get => _LoadSavesCommand;
            set => _LoadSavesCommand = value;
        }

        private Task LoadSaves()
        {
            return Task.Run(() =>
            {
                GlobalState.Instance.GameSaves = GameSaveUtil.GetGameSaves().ToList();
            });
        }
    }
}
