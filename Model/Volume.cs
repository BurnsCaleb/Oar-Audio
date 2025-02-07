using System.ComponentModel;

namespace Oar_Audio.Model
{
    public class Volume:INotifyPropertyChanged
    {
        public string Name { get; set; }
        public int _level { get; set; }

        public int Level
        {
            get => _level;
            set
            {
                if (_level != value)
                {
                    _level = value;
                    OnPropertyChanged(nameof(Level));
                    OnPropertyChanged(nameof(DisplayText));
                }
            }
        }

        public string VolumeUp { get; set; } // Keybind
        public string VolumeDown { get; set; } // Keybind
        public string VolumeMute { get; set; } // Keybind
        public bool _isMute { get; set; }
        public bool IsMute
        {
            get => _isMute;
            set
            {
                if (_isMute != value)
                {
                    _isMute = value;
                    OnPropertyChanged(nameof(IsMute));
                    OnPropertyChanged(nameof(DisplayText));
                }
            }
        }

        public Volume(string Name, int Level, string VolumeUp, string VolumeDown, string VolumeMute, bool IsMute)
        {
            this.Name = Name;
            this.Level = Level;
            this.VolumeUp = VolumeUp;
            this.VolumeDown = VolumeDown;
            this.VolumeMute = VolumeMute;
            this.IsMute = IsMute;
        }

        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>
            {
                { "Name", Name },
                { "Level", Level },
                { "Volume_up", VolumeUp },
                { "Volume_down", VolumeDown },
                { "Volume_mute", VolumeMute },
                { "IsMute", IsMute}
            };
        }

        public string DisplayText => IsMute ? "Muted" : Level.ToString();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
