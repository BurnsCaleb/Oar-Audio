using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Oar_Audio.Model;

namespace Oar_Audio
{
    /// <summary>
    /// Interaction logic for PopupWindow.xaml
    /// </summary>
    public partial class PopupWindow : Window
    {
        private DispatcherTimer timer;
        public PopupWindow(Volume volume)
        {
            InitializeComponent();

            // Position top left
            Left = SystemParameters.WorkArea.Left + 10;
            Top = SystemParameters.WorkArea.Top + 10;

            // Set timer to close window after 2 seconds
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            timer.Tick += Timer_Tick;
            timer.Start();


            // Set Header Text
            string headerText;
            switch (volume.Name)
            {
                default:
                    headerText = "";
                        break;
                case "mastervolume":
                    headerText = "Master";
                    break;
                case "mediavolume":
                    headerText = "Media";
                    break;
                case "gamesvolume":
                    headerText = "Games";
                    break;
                case "auxvolume":
                    headerText = "Aux";
                    break;
            }
            volumeGroupName.Text = headerText;

            // Setup Progress Bar
            volumeProgressBar.Value = volume.Level;
            // Setup Volume Level text
            Binding volumeLevelBind = new Binding();
            volumeLevelBind.Source = volume;
            volumeLevelBind.Path = new PropertyPath("DisplayText");
            volumeTextBlock.SetBinding(TextBlock.TextProperty, volumeLevelBind);
        }

        // On Timer end stop timer and close window
        private void Timer_Tick(object? sender, EventArgs e)
        {
            timer.Stop();
            Close();
        }
    }
}
