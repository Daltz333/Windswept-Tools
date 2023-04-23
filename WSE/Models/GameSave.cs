using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Tmds.DBus;

namespace WSE.Models
{
    public class GameSave : INotifyPropertyChanged
    {
        private double _GameTimeSeconds;

        /// <summary>
        /// Represents the total game time in milliseconds
        /// </summary>
        public double GameTimeSeconds
        {
            get => _GameTimeSeconds;
            set { _GameTimeSeconds = value; OnPropertyChanged(nameof(_GameTimeSeconds)); }
        }

        private int _GameCoins;
        
        /// <summary>
        /// Represents the total coins collected in the current run
        /// </summary>
        public int GameCoins
        {
            get => _GameCoins;
            set { _GameCoins = value; OnPropertyChanged(nameof(GameCoins)); }
        }

        private HardmodeState _HardmodePlayers;

        /// <summary>
        /// Represents whether or not the current save is in hard mode
        /// </summary>
        public HardmodeState HardmodePlayers
        {
            get => _HardmodePlayers;
            set { _HardmodePlayers = value; OnPropertyChanged(nameof(HardmodePlayers)); }
        }

        private List<string> _hardmodeStates = new()
        {
            "None",
            "Turtle",
            "Duck",
        };

        /// <summary>
        /// Represents as a string list the possible hardmode states
        /// Only used for the GUI
        /// </summary>
        public List<string> HardmodeStates
        {
            get => _hardmodeStates;
            set { _hardmodeStates = value; OnPropertyChanged(nameof(HardmodeStates)); }
        }

        private List<Stage>? _Stages;

        public List<Stage> Stages
        {
#pragma warning disable CS8603 // Possible null reference return.
            get => _Stages;
#pragma warning restore CS8603 // Possible null reference return.
            set { _Stages = value; OnPropertyChanged(nameof(Stages)); }
        }

        private Stage? _SelectedStage;

        public Stage? SelectedStage
        {
            get => _SelectedStage;
            set { _SelectedStage = value; OnPropertyChanged(nameof(SelectedStage)); }
        }

        private string? _SaveName;

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Name of the save
        /// 
        /// EX: Save 1
        /// </summary>
        public string SaveName
        {
            get => _SaveName ?? "null";
            set { _SaveName = value; OnPropertyChanged(nameof(SaveName)); }
        }

        public UnparsedGameSave? UnparsedGameSave;

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class Stage : INotifyPropertyChanged
    {
        private int _StageIndex;

        /// <summary>
        /// Represents the index of the stage, as to be saved
        /// </summary>
        public int StageIndex
        {
            get => _StageIndex;
            set { _StageIndex = value; OnPropertyChanged(nameof(StageIndex)); }
        }

        private StageCollectableState _CometState;

        /// <summary>
        /// Represents whether all the comets were collected for this stage
        /// </summary>
        public StageCollectableState CometState
        {
            get => _CometState;
            set { _CometState = value; OnPropertyChanged(nameof(CometState)); }
        }

        private int _ShardsCollected;

        /// <summary>
        /// Represents the number of shards collected on this stage
        /// 
        /// Value of 0 - 4
        /// Stored in JSON as EX: 2-2-2-0-0 (collect, collect, collect, none, none)
        /// </summary>
        public int ShardsCollected
        {
            get => _ShardsCollected;
            set { _ShardsCollected = value; OnPropertyChanged(nameof(ShardsCollected)); }
        }

        private bool _IsPacifist;

        /// <summary>
        /// Represents whether or not this stage was completed without killing enemies
        /// </summary>
        public bool IsPacifist
        {
            get => _IsPacifist;
            set { _IsPacifist = value; OnPropertyChanged(nameof(IsPacifist)); }
        }

        private double _StageTimeSeconds;

        /// <summary>
        /// Represents the stage time in milliseconds
        /// </summary>
        public double StageTimeSeconds
        {
            get => _StageTimeSeconds;
            set { _StageTimeSeconds = value; OnPropertyChanged(nameof(StageTimeSeconds)); }
        }

        private StageCollectableState _CloudState;

        /// <summary>
        /// Represents whether all the clouds were collected for the current stage
        /// </summary>
        public StageCollectableState CloudState
        {
            get => _CloudState;
            set { _CloudState = value; OnPropertyChanged(nameof(CloudState)); }
        }

        private int _NumMoons;

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Represents whether all the moons were collected for the current stage
        /// </summary>
        public int NumMoons
        {
            get => _NumMoons;
            set { _NumMoons = value; OnPropertyChanged(nameof(NumMoons)); }
        }


        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    /// <summary>
    /// Represents the state of a stage collectable
    /// 
    /// This currently represents Clouds, Comets, and Moons
    /// </summary>
    public enum StageCollectableState
    {
        None = 0,
        Partial = 1,
        All = 2,
    }
    
    public enum HardmodeState
    {
        None = 0,
        Duck = 1,
        Turtle = 2,
    }
}
