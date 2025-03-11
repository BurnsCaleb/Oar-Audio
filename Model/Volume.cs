using System.ComponentModel;
using System.Text.Json.Serialization;
using Oar_Audio.Utilities.Hotkey;

namespace Oar_Audio.Model
{
    public class Volume:INotifyPropertyChanged
    {
        public string Name { get; set; }
        private int _level { get; set; }

        public int Level
        {
            get { return _level; }
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

        private List<string> _volumeUp;
        private List<string> _volumeDown;
        private List<string> _volumeMute;

        public List<string> VolumeUp 
        { 
            get { return _volumeUp; }
            set 
            {
                _volumeUp = value;
                OnPropertyChanged(nameof(VolumeUp));
            }
        }

        public List<string> VolumeDown 
        { 
            get { return _volumeDown; } 
            set 
            { 
                _volumeDown = value;
                OnPropertyChanged(nameof(VolumeDown));
            }
        }

        public List<string> VolumeMute 
        { 
            get { return _volumeMute; }
            set
            {
                _volumeMute = value;
                OnPropertyChanged(nameof(VolumeMute));
            }
        
        }
        public bool _isMute { get; set; }
        public bool IsMute
        {
            get { return _isMute; }
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

        public Volume(string Name, int Level, List<string> VolumeUp, List<string> VolumeDown, List<string> VolumeMute, bool IsMute)
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
                { "VolumeUp", VolumeUp },
                { "VolumeDown", VolumeDown },
                { "VolumeMute", VolumeMute },
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
