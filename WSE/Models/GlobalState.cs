using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WSE.Models
{
    public class GlobalState : INotifyPropertyChanged
    {
        private static GlobalState? _Instance;

        public static GlobalState Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new GlobalState();
                }

                return _Instance;
            }
        }

        private List<GameSave>? _GameSaves;

        /// <summary>
        /// Represents the total game saves
        /// </summary>
        public List<GameSave> GameSaves
        {
#pragma warning disable CS8603 // Possible null reference return.
            get => _GameSaves;
#pragma warning restore CS8603 // Possible null reference return.
            set { _GameSaves = value; OnPropertyChanged(nameof(GameSaves)); }
        }

        private GameSave? _SelectedGameSave;
        
        /// <summary>
        /// Represents the selected game save
        /// </summary>
        public GameSave SelectedGameSave
        {
#pragma warning disable CS8603 // Possible null reference return.
            get => _SelectedGameSave;
#pragma warning restore CS8603 // Possible null reference return.
            set { _SelectedGameSave = value; OnPropertyChanged(nameof(SelectedGameSave)); }
        }

        private string _SaveSearchDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Windswept");

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Represents the search directory to search for game saves
        /// </summary>
        public string SaveSearchDir
        {
            get => _SaveSearchDir;
            set { _SaveSearchDir = value; OnPropertyChanged(nameof(SaveSearchDir)); }
        }

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
