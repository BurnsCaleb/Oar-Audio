using System.ComponentModel;

namespace Oar_Audio.Model
{
    class Source
    {
        public string DisplayName { get; set; }
        public string ProcessName { get; set; }
        public string IconLocation {  get; set; }
        public int Location { get; set; }
        public bool IsHidden { get; set; }


        public Source(string DisplayName, string ProcessName, string IconLocation, int Location, bool IsHidden)
        {
            this.DisplayName = DisplayName;
            this.ProcessName = ProcessName;
            this.IconLocation = IconLocation;
            this.Location = Location;
            this.IsHidden = IsHidden;
        }

        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>
            {
                { "DisplayName", DisplayName },
                { "ProcessName", ProcessName },
                { "IconLocation", IconLocation },
                { "Location", Location },
                { "IsHidden", IsHidden }
            };
        }
    }
}
