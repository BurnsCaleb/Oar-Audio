using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using NAudio.Wave.SampleProviders;
using Oar_Audio.Model;

namespace Oar_Audio.View
{
    /// <summary>
    /// Interaction logic for KeybindModifier.xaml
    /// </summary>
    public partial class KeybindModifier : Window
    {
        private View.MainWindow _mainWindow;
        public List<Volume> volumes { get; set; }

        public KeybindModifier(View.MainWindow mainWindow, ref List<Volume>volumes)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            this.volumes = volumes;

            // Setup Data Binding
            DataContext = this;
        }

        private void ApplyButtonClick(object sender, RoutedEventArgs e)
        {
            string groupInput = groupSelector.Text;
            int group;
            if (string.IsNullOrEmpty(groupInput) || groupInput.Equals("Master Volume Control"))
            {
                group = 0;
            } else if (groupInput.Equals("Media Volume Control"))
            {
                group = 1;
            } else if (groupInput.Equals("Games Volume Control"))
            {
                group = 2;
            } else if (groupInput.Equals("Aux Volume Control"))
            {
                group = 3;
            } else
            {
                group = 4;
            }

            string controlInput = controlSelector.Text;
            int control;
            if (string.IsNullOrEmpty(controlInput) || controlInput.Equals("Change Increase Volume"))
            {
                control = 1;
            }
            else if (controlInput.Equals("Change Decrease Volume"))
            {
                control = -1;
            }
            else if (controlInput.Equals("Toggle Volume Mute"))
            {
                control = 0;
            }
            else
            {
                control = 4;
            }


            string modifier1Input = modifier1Selector.Text;
            if (string.IsNullOrEmpty(modifier1Input) || modifier1Input.Equals("None"))
            {
                modifier1Input = null;
            }

            string modifier2Input = modifier2Selector.Text;
            if (string.IsNullOrEmpty(modifier2Input) || modifier2Input.Equals("None"))
            {
                modifier2Input = null;
            }

            string key = keySelector.Text;
            if (string.IsNullOrEmpty(key))
            {
                key = "Up";
            }

            // Get List to populate Volume class that holds keybinds
            List<string> hotkey = new List<string>();
            if (modifier1Input == null && modifier2Input == null)
            {
                hotkey.Add(key);
            }
            else if (modifier1Input != null && modifier2Input == null)
            {
                hotkey.Add(modifier1Input);
                hotkey.Add(key);
            }
            else if (modifier1Input == null && modifier2Input != null)
            {
                hotkey.Add(modifier2Input);
                hotkey.Add(key);
            } else if (modifier1Input != null & modifier2Input != null)
            {
                hotkey.Add(modifier1Input);
                hotkey.Add(modifier2Input);
                hotkey.Add(key);
            }

            // Update Class
            switch (control){
                case 1:
                    this.volumes[group].VolumeUp = hotkey;
                    break;
                case 2:
                    this.volumes[group].VolumeDown = hotkey;
                    break;
                case 3:
                    this.volumes[group].VolumeMute = hotkey;
                    break;
            }

            // Reset Keybinds
            _mainWindow.ResetKeybinds();

            keybindModifier.Close();
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            keybindModifier.Close();
        }
    }
}
