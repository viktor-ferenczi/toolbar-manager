using ToolbarManager.Settings;
using ToolbarManager.Settings.Elements;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ToolbarManager.Settings.Tools;
using VRage.Input;


namespace ToolbarManager
{
    public class Config : INotifyPropertyChanged
    {
        public static readonly Config Default = new Config();
        public static readonly Config Current = ConfigStorage.Load();

        public static void Save()
        {
            ConfigStorage.Save(Current);
        }
        
        #region Options
        
        private bool useCharacterProfilesForBuildCockpit = true;
        
        private bool keepBlockSearchText;
        private string latestBlockSearchText = "";
        
        private bool keepProfileSearchText;
        private string latestProfileSearchText = "";
        
        private Binding blockSearchKey = new Binding(MyKeys.OemPipe);
        
        #endregion
        
        #region User interface
        
        public readonly string Title = "Toolbar Manager";

        [Separator("Functionality")]
        [Checkbox(label: "Build cockpit is character", description: "Building from cockpit (Ctrl-G) uses the character profiles")]
        public bool UseCharacterProfilesForBuildCockpit
        {
            get => useCharacterProfilesForBuildCockpit;
            set => SetField(ref useCharacterProfilesForBuildCockpit, value);
        }

        [Separator("Recent searches")]
        [Checkbox(description: "Keep the search text between subsequent uses of the block search")]
        public bool KeepBlockSearchText
        {
            get => keepBlockSearchText;
            set => SetField(ref keepBlockSearchText, value);
        }

        // [Textbox(description: "Latest block search text")]
        public string LatestBlockSearchText
        {
            get => latestBlockSearchText;
            set => SetField(ref latestBlockSearchText, value);
        }

        [Checkbox(description: "Keep the search text between subsequent uses of the Profile dialog")]
        public bool KeepProfileSearchText
        {
            get => keepProfileSearchText;
            set => SetField(ref keepProfileSearchText, value);
        }

        // [Textbox(description: "Latest profile search text")]
        public string LatestProfileSearchText
        {
            get => latestProfileSearchText;
            set => SetField(ref latestProfileSearchText, value);
        }

        [Separator("Hotkeys")]
        [Keybind(description: "Key to open the G menu and activate block search")]
        public Binding BlockSearchKey
        {
            get => blockSearchKey;
            set => SetField(ref blockSearchKey, value);
        }

        #endregion
        
        #region Property change notification bilerplate

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        
        #endregion
    }
}