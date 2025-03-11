using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using Newtonsoft.Json;

namespace Oar_Audio.Model
{
    public class Source
    {
        [JsonProperty("DisplayName")]
        public string DisplayName { get; set; }

        [JsonProperty("ProcessName")]
        public string ProcessName { get; set; }

        [JsonProperty("IconLocation")]
        public string IconLocation {  get; set; }

        [JsonProperty("Location")]
        public int Location { get; set; }

        [JsonProperty("IsHidden")]
        public bool IsHidden { get; set; }

        [JsonProperty("AudioSessionIdentifier")]
        public string AudioSessionIdentifier { get; set; }

        [JsonIgnore]
        public AudioSessionControl? AudioSession { get; set; }

        public Source() { }

        public Source(string DisplayName, string ProcessName, string IconLocation, int Location, bool IsHidden, string AudioSessionIdentifier)
        {
            this.DisplayName = DisplayName;
            this.ProcessName = ProcessName;
            this.IconLocation = IconLocation;
            this.Location = Location;
            this.IsHidden = IsHidden;
            this.AudioSessionIdentifier = AudioSessionIdentifier;
            this.AudioSession = null;
        }

        public Source(string DisplayName, string ProcessName, string IconLocation, int Location, bool IsHidden, string AudioSessionIdentifier, AudioSessionControl AudioSession)
        {
            this.DisplayName = DisplayName;
            this.ProcessName = ProcessName;
            this.IconLocation = IconLocation;
            this.Location = Location;
            this.IsHidden = IsHidden;
            this.AudioSessionIdentifier = AudioSessionIdentifier;
            this.AudioSession = AudioSession;
        }

        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>
            {
                { "DisplayName", DisplayName },
                { "ProcessName", ProcessName },
                { "IconLocation", IconLocation },
                { "Location", Location },
                { "IsHidden", IsHidden },
                { "AudioSessionIdentifier", AudioSessionIdentifier }
            };
        }
    }
}
