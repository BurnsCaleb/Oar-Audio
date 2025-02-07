using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using NAudio.CoreAudioApi;
using Oar_Audio.Model;
using Oar_Audio.Utilities.Hotkey;
using Binding = System.Windows.Data.Binding;
using TextBox = System.Windows.Controls.TextBox;

namespace Oar_Audio
{
    public partial class MainWindow : Window
    {
        #region Initializing Variables
        //NAudio Data
        private static MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
        private static MMDevice defaultDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
        private static AudioSessionManager? sessionManager;
        private static SessionCollection? defaultSession;

        // Lists
        private List<AudioSessionControl> sessions = new List<AudioSessionControl>();
        private static List<Source> sources = new List<Source>();
        private static List<Source> activeSources = new List<Source>();
        private static List<Volume> volumes = new List<Volume>();
        private static readonly List<Guid> guids = new List<Guid>() { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
        private List<Source> tempShow = new List<Source>();

        private static bool onlyShowActiveSources = true;

        // xaml Data
        private StackPanel? stackPanel;
        private TextBlock? sourceTextBlock;
        private ContextMenu? contextMenu;

        // System Tray
        private NotifyIcon? notifyIcon;
        private ToolStripMenuItem? toggleKeybinds;
        #endregion

        public MainWindow()
        {
            InitializeData();
            InitializeSystemTray();
            InitializeComponent();
            PopulateGui();
            InitializeKeybinds();

            // When a new audio session is created
            // Add to sessions list and categorize
            if (sessionManager != null)
            {
                sessionManager.OnSessionCreated += async (sender, session) =>
                {
                    await Task.Delay(1000);
                    AudioSessionControl newSession = new AudioSessionControl(session);
                    Debug.WriteLine("New Audio Session Detected: " + newSession.GetProcessID);
                    Debug.WriteLine("New Audio Session Detected: " + newSession);
                    sessions.Add(newSession);
                    CreateNewSource(newSession);
                };
            }

        }


        /// <summary>
        /// To be called at the start of the program. Sets up
        /// necessary data to be used elsewhere in the program
        /// </summary>
        public void InitializeData()
        {
            // Read previous audio sources
            sources = Utilities.JsonUtil.GetSources();

            // Populate volumes list
            volumes = Utilities.JsonUtil.GetVolumes();

            // NAudio Data
            sessionManager = defaultDevice.AudioSessionManager;
            defaultSession = sessionManager.Sessions;

            // Add each default session to sessions list
            for (int i = 0; i < defaultSession.Count; i++)
            {
                Debug.WriteLine($"{defaultSession[i].GetProcessID}");
                sessions.Add(defaultSession[i]);
            }

            LinkSessionsAndSources();
        }


        /// <summary>
        /// Sets up system tray and context menu
        /// </summary>
        private void InitializeSystemTray()
        {
            notifyIcon = new NotifyIcon()
            {
                Icon = new System.Drawing.Icon(@"C:\\Program Files\\Oar Audio\\images\\oar_img.ico"),
                Visible = true,
                Text = "Oar Audio"
            };
            notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.Add("Open", null, (sender, args) => ShowWindow());
            // Toggle Keybinds
            toggleKeybinds = new ToolStripMenuItem("Toggle Keybinds")
            {
                CheckOnClick = true,
                Checked = true
            };
            toggleKeybinds.CheckedChanged += ToggleKeybinds_CheckedChanged;
            notifyIcon.ContextMenuStrip.Items.Add(toggleKeybinds);
            notifyIcon.ContextMenuStrip.Items.Add("Exit", null, (sender, args) => ExitApplication());

            notifyIcon.DoubleClick += (sender, args) => ShowWindow();
        }


        /// <summary>
        /// Creates Audio Source items and adds them to the GUI.
        /// </summary>
        public void PopulateGui()
        {
            masterPanel.Dispatcher.Invoke(() =>
            {

                // Clear StackPanels
                masterPanel.Children.Clear();
                mediaPanel.Children.Clear();
                gamesPanel.Children.Clear();
                auxPanel.Children.Clear();
            });


            // Create AudioSource Widgets/TextBlocks
            if (activeSources != null)
            {
                // Show active sources or all sources
                List<Source> appSources = new List<Source>();
                if (onlyShowActiveSources)
                {
                    appSources = activeSources; 
                } 
                else
                {
                    appSources = sources;
                }
                foreach (Source source in appSources)
                {
                    if (source.IsHidden == false)
                    {
                        // Get Correct StackPanel for TextBlock
                        switch (source.Location)
                        {
                            default:
                                stackPanel = masterPanel;
                                break;
                            case 1:
                                stackPanel = mediaPanel;
                                break;
                            case 2:
                                stackPanel = gamesPanel;
                                break;
                            case 3:
                                stackPanel = auxPanel;
                                break;
                        }

                        masterPanel.Dispatcher.Invoke(() =>
                        {
                            // Create TextBlock
                            sourceTextBlock = new TextBlock
                            {
                                Text = source.DisplayName,
                            };


                            Border border = new Border();
                            border.Style = this.FindResource("TextBlockBorder") as Style;
                            border.ContextMenu = CreateContextMenu(source, border, stackPanel, sourceTextBlock);

                            // Add TextBlock to Border, then to StackPanel
                            border.Child = sourceTextBlock;
                            stackPanel.Children.Add(border);
                                       
                        });
                    }
                }
            }


            #region Set up Data Binding
            masterPanel.Dispatcher.Invoke(() =>
            {
                Binding masterVolumeBinding = new Binding();
                masterVolumeBinding.Source = volumes[0];
                masterVolumeBinding.Path = new PropertyPath("DisplayText");
                masterVolumeLevel.SetBinding(TextBlock.TextProperty, masterVolumeBinding);


                Binding mediaVolumeBinding = new Binding();
                mediaVolumeBinding.Source = volumes[1];
                mediaVolumeBinding.Path = new PropertyPath("DisplayText");
                mediaVolumeLevel.SetBinding(TextBlock.TextProperty, mediaVolumeBinding);


                Binding gamesVolumeBinding = new Binding();
                gamesVolumeBinding.Source = volumes[2];
                gamesVolumeBinding.Path = new PropertyPath("DisplayText");
                gamesVolumeLevel.SetBinding(TextBlock.TextProperty, gamesVolumeBinding);


                Binding auxVolumeBinding = new Binding();
                auxVolumeBinding.Source = volumes[3];
                auxVolumeBinding.Path = new PropertyPath("DisplayText");
                auxVolumeLevel.SetBinding(TextBlock.TextProperty, auxVolumeBinding);
            });
            #endregion
        }

        
        /// <summary>
        /// Sets Up a set configuration of keybinds for
        /// controlling volume.
        /// </summary>
        public void InitializeKeybinds()
        {
            HotkeysManager.SetupSystemHook();

            // Master Volume Keys
            // Control+Up, Control+Down, Control+Left
            HotkeysManager.AddHotkey(ModifierKeys.Control, Key.Up, () => { VolumeAction(0, 1); }, true);
            HotkeysManager.AddHotkey(ModifierKeys.Control, Key.Left, () => { VolumeAction(0, 0); }, true);
            HotkeysManager.AddHotkey(ModifierKeys.Control, Key.Down, () => { VolumeAction(0, -1); }, true);

            // Media Volume Keys
            // Alt+Up, Alt+Down, Alt+Left
            HotkeysManager.AddHotkey(ModifierKeys.Alt, Key.Up, () => { VolumeAction(1, 1); }, true);
            HotkeysManager.AddHotkey(ModifierKeys.Alt, Key.Left, () => { VolumeAction(1, 0); }, true);
            HotkeysManager.AddHotkey(ModifierKeys.Alt, Key.Down, () => { VolumeAction(1, -1); }, true);

            // Games Volume Keys
            // Shift+Up, Shift+Down, Shift+Left
            HotkeysManager.AddHotkey(ModifierKeys.Shift, Key.Up, () => { VolumeAction(2, 1); }, true);
            HotkeysManager.AddHotkey(ModifierKeys.Shift, Key.Left, () => { VolumeAction(2, 0); }, true);
            HotkeysManager.AddHotkey(ModifierKeys.Shift, Key.Down, () => { VolumeAction(2, -1); }, true);

            // Aux Volume Keys
            // Control+Up, Control+Down, Control+Left
            HotkeysManager.AddHotkey(ModifierKeys.Control, ModifierKeys.Shift, Key.Up, () => { VolumeAction(3, 1); }, true);
            HotkeysManager.AddHotkey(ModifierKeys.Control, ModifierKeys.Shift, Key.Left, () => { VolumeAction(3, 0); }, true);
            HotkeysManager.AddHotkey(ModifierKeys.Control, ModifierKeys.Shift, Key.Down, () => { VolumeAction(3, -1); }, true);          
        }


        /// <summary>
        /// Turns on or off keybind system hook based on context menu value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToggleKeybinds_CheckedChanged(object? sender, EventArgs e)
        {
            if (toggleKeybinds != null)
            {
                if (toggleKeybinds.Checked)
                {
                    HotkeysManager.SetupSystemHook();
                }
                else
                {
                    HotkeysManager.ShutdownSystemHook();
                }
            }
        }


        /// <summary>
        /// Helper method for PopulateGui(). Creates a new context menu for each TextBlock.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="border"></param>
        /// <param name="stackPanel"></param>
        /// <returns>contextMenu</returns>
        private ContextMenu CreateContextMenu(Source source, Border border, StackPanel stackPanel, TextBlock currentTextBlock)
        {
            contextMenu = new ContextMenu();


            MenuItem changeDisplayNameMenuItem = new MenuItem
            {
                Header = "Change Display Name"
            };
            changeDisplayNameMenuItem.Click += (s, e) =>
            {
                // Create TextBox for New DisplayName
                TextBox displayNameTextBox = new TextBox();

                // Update Display Name and Change back to TextBlock
                displayNameTextBox.LostFocus += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(displayNameTextBox.Text))
                    {
                        source.DisplayName = displayNameTextBox.Text;
                        currentTextBlock.Text = source.DisplayName;
                        border.Child = currentTextBlock;
                    }
                };

                // Update Display Name and Change back to TextBlock
                displayNameTextBox.KeyDown += (sender, args) =>
                {
                    if (args.Key == Key.Enter && stackPanel != null)
                    {
                        source.DisplayName = displayNameTextBox.Text;
                        currentTextBlock.Text = source.DisplayName;
                        border.Child = currentTextBlock;
                    }
                };

                // Remove TextBlock and Replace with TextBox
                if (stackPanel != null)
                {
                    border.Child = displayNameTextBox;
                    displayNameTextBox.Focus();
                }
            };


            MenuItem resetDisplayNameMenuItem = new MenuItem
            {
                Header = "Reset Display Name"
            };
            resetDisplayNameMenuItem.Click += (sender, e) =>
            {
                source.DisplayName = source.ProcessName;
                currentTextBlock.Text = source.DisplayName;
            };




            MenuItem moveAudioSourceMenuItem = new MenuItem
            {
                Header = "Move Audio Source",
            };

            MenuItem masterLocation = new MenuItem
            {
                Header = "Master"
            };
            masterLocation.Click += (sender, e) =>
            {
                source.Location = 0;
                stackPanel.Children.Remove(border);
                stackPanel = masterPanel;
                stackPanel.Children.Add(border);

                LinkSessionsAndSources();

            };

            MenuItem mediaLocation = new MenuItem
            {
                Header = "Media"
            };
            mediaLocation.Click += (sender, e) =>
            {
                source.Location = 1;
                stackPanel.Children.Remove(border);
                stackPanel = mediaPanel;
                stackPanel.Children.Add(border);

                LinkSessionsAndSources();
            };

            MenuItem gamesLocation = new MenuItem
            {
                Header = "Games"
            };
            gamesLocation.Click += (sender, e) =>
            {
                source.Location = 2;
                stackPanel.Children.Remove(border);
                stackPanel = gamesPanel;
                stackPanel.Children.Add(border);

                LinkSessionsAndSources();
            };

            MenuItem auxLocation = new MenuItem
            {
                Header = "Aux"
            };
            auxLocation.Click += (sender, e) =>
            {
                source.Location = 3;
                stackPanel.Children.Remove(border);
                stackPanel = auxPanel;
                stackPanel.Children.Add(border);

                LinkSessionsAndSources();
            };


            MenuItem hideAudioSourceMenuItem = new MenuItem
            {
                Header = "Hide Audio Source"
            };
            hideAudioSourceMenuItem.Click += (sender, e) =>
            {
                source.IsHidden = true;
                stackPanel.Children.Remove(border);
            };



            moveAudioSourceMenuItem.Items.Add(masterLocation);
            moveAudioSourceMenuItem.Items.Add(mediaLocation);
            moveAudioSourceMenuItem.Items.Add(gamesLocation);
            moveAudioSourceMenuItem.Items.Add(auxLocation);

            contextMenu.Items.Add(changeDisplayNameMenuItem);
            contextMenu.Items.Add(resetDisplayNameMenuItem);
            contextMenu.Items.Add(moveAudioSourceMenuItem);
            contextMenu.Items.Add(hideAudioSourceMenuItem);

            return contextMenu;
        }


        /// <summary>
        /// Searches for a matching session and source name.
        /// Sets source data as needed to match session.
        /// Sets grouping parameter for volume controls.
        /// </summary>
        private void LinkSessionsAndSources()
        {
            // Get AudioSession name
            // Find in sources_conf.json file
            // Set grouping parameter if needed
            foreach (AudioSessionControl session in sessions)
            {
                bool matching = false;
                string sessionDisplayName = session.DisplayName;
                string appName = "Unknown";
                string processName = "Unknown";


                // Get Audio Source name by accessing Process Name
                if (session.GetProcessID != 0)
                {
                    try
                    {
                        Process process = Process.GetProcessById((int)session.GetProcessID);
                        processName = process.ProcessName;
                        appName = process.ProcessName;
                        sessionDisplayName = string.IsNullOrEmpty(sessionDisplayName) ? appName : sessionDisplayName;
                    }
                    catch
                    {
                        appName = "Unknown (Process Ended)";
                    }
                }
                else
                {
                    appName = "System Sounds";
                    processName = sessionDisplayName;
                }

                // Match Process name with Source Name
                // Set Grouping Parameter if needed
                if (sources != null)
                {
                    foreach (Source source in sources)
                    {
                        if (!string.IsNullOrEmpty(source.ProcessName))
                        {
                            if (source.ProcessName.Equals(sessionDisplayName))
                            {
                                matching = true;

                                if (source.Location != 0)
                                {
                                    session.SetGroupingParam(guids[source.Location - 1], Guid.Empty);
                                }

                                // Get Path to Icon
                                source.IconLocation = session.IconPath;

                                // Add active sessions to sessions list
                                FilterAudioSessions(session, source);

                                break;
                            }
                        }
                    }
                }
                // Create new source to add to sources_conf.json
                if (!matching)
                {
                    if (sources == null)
                    {
                        sources = new List<Source>();
                    }
                    Source newSource = new Source(sessionDisplayName, processName, session.IconPath, 0, false);
                    sources.Add(newSource);
                    // Add active sessions to sessions list
                    FilterAudioSessions(session, newSource);
                    Utilities.JsonUtil.WriteToSources(sources);
                }      

                
            }
            ChangeAllVolumes();
        }


        /// <summary>
        /// Creates a new source object to be added to the GUI.
        /// Similar to LinkSessionAndSources but only acts for one session
        /// </summary>
        /// <param name="newSession"></param>
        public void CreateNewSource(AudioSessionControl newSession)
        {
            bool matching = false;
            string sessionDisplayName = newSession.DisplayName;
            string processName = "Unknown";
            string appName = "Unknown";

            if (newSession.GetProcessID != 0)
            {
                try
                {
                    Process process = Process.GetProcessById((int)newSession.GetProcessID);
                    processName = process.ProcessName;
                    appName = process.ProcessName;
                    sessionDisplayName = string.IsNullOrEmpty(sessionDisplayName) ? appName : sessionDisplayName;
                }
                catch
                {
                    appName = "Unknown (Process Ended)";
                }
            }
            else
            {
                appName = "System Sounds";
                processName = sessionDisplayName;
            }

            if (sources != null)
            {

                foreach (Source source in sources)
                {
                    if (!string.IsNullOrEmpty(source.ProcessName))
                    {
                        if (source.ProcessName.Equals(sessionDisplayName))
                        {
                            matching = true;

                            newSession.SetGroupingParam(guids[source.Location - 1], Guid.Empty);

                            // Get Icon Path
                            source.IconLocation = newSession.IconPath;

                            FilterAudioSessions(newSession, source);

                            break;
                        }
                    }
                }

                // Create new source to add to sources_conf.json
                if (!matching)
                {
                    Source newSource = new Source(sessionDisplayName, processName, newSession.IconPath, 0, false);
                    sources.Add(newSource);
                    FilterAudioSessions(newSession, newSource);
                    Utilities.JsonUtil.WriteToSources(sources);
                }

                PopulateGui();
            }

            // Get new source index and update its volume
            if (sources != null)
            {
                int sourceIndex = sources.FindIndex(x => x.ProcessName == processName);
                if (sourceIndex != -1)
                {
                    ChangeOneVolume(sources[sourceIndex].Location);
                }
            }
        }


        /// <summary>
        /// Filter Audio Sessions to only show active sessions with volume
        /// </summary>
        /// <param name="session"></param>
        private void FilterAudioSessions(AudioSessionControl session, Source source)
        {
            // Add to active source list if audio session is active and has volume
            if (session.State == NAudio.CoreAudioApi.Interfaces.AudioSessionState.AudioSessionStateActive)
            {
                Debug.WriteLine($"Active Audio Session: {0}", source.ProcessName);
                activeSources.Add(source);
            }
        }


        /// <summary>
        /// Updates an instance property of Volume after a keybind is pressed.
        /// location parameter accepts values 0, 1, 2, 3. Corresponds to location of
        /// audio sources in GUI.
        /// action parameter accepts values -1, 0, 1 for volume down, mute, and volume up 
        /// respectively.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="action"></param>
        public void VolumeAction(int location, int action)
        {
            // Grab volume
            // Update and make sure it is between 0 and 100
            // Assign to object
            int level;
            int changeLevel;
            bool toggleMute = false;
            level = volumes[location].Level;

            // Get Direction of volume
            // Increments / decrements by 5
            switch (action)
            {
                case -1:
                    changeLevel = -5;
                    break;
                case 0:
                    toggleMute = true;
                    changeLevel = 0;
                    break;
                case 1:
                    changeLevel = 5;
                    break;
                default:
                    changeLevel = 0;
                    break;
            }

            // Assign and make sure value is between 0 and 100
            level += changeLevel;

            if (level > 100)
            {
                level = 100;
            }
            else if (level < 0)
            {
                level = 0;
            }

            // If toggleMute is true, grab mute value and assign
            if (toggleMute)
            {
                volumes[location].IsMute = !volumes[location].IsMute;
            }

            // Set Object property and call ChangeOneVolume method
            volumes[location].Level = level;

            ShowPopupWindow(volumes[location]);

            ChangeOneVolume(location);
        }


        /// <summary>
        /// Changes volumes for each audio session in sources list
        /// </summary>
        public void ChangeAllVolumes()
        {
            // Update Master Volume
            defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar = volumes[0].Level / 100f;
            defaultDevice.AudioEndpointVolume.Mute = volumes[0].IsMute;

            // Update Other Guids
            foreach (AudioSessionControl session in sessions)
            {
                if (session.GetGroupingParam() == guids[0])
                {
                    session.SimpleAudioVolume.Volume = volumes[1].Level / 100f;
                    session.SimpleAudioVolume.Mute = volumes[1].IsMute;
                }
                else if (session.GetGroupingParam() == guids[1])
                {
                    session.SimpleAudioVolume.Volume = volumes[2].Level / 100f;
                    session.SimpleAudioVolume.Mute = volumes[2].IsMute;
                }
                else if (session.GetGroupingParam() == guids[2])
                {
                    session.SimpleAudioVolume.Volume = volumes[3].Level / 100f;
                    session.SimpleAudioVolume.Mute = volumes[3].IsMute;
                }
            }
        }
    

        /// <summary>
        /// Changes volumes for one GUID
        /// </summary>
        /// <param name="location"></param>
        public void ChangeOneVolume(int location)
        {
            if (location == 0)
            {
                defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar = volumes[location].Level / 100f;
                defaultDevice.AudioEndpointVolume.Mute = volumes[location].IsMute;
            }
            else
            {
                foreach (AudioSessionControl session in sessions)
                {
                    if (session.GetGroupingParam() == guids[location - 1])
                    {
                        session.SimpleAudioVolume.Volume = volumes[location].Level / 100f;
                        session.SimpleAudioVolume.Mute = volumes[location].IsMute;
                    }
                }
            }
        }


        /// <summary>
        /// Sets IsHidden to true for each source object and rebuilds GUI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowAllSources(object sender, RoutedEventArgs e)
        {
            onlyShowActiveSources = !onlyShowActiveSources;
            PopulateGui();
        }


        /// <summary>
        /// Brings window to focus from system tray.
        /// </summary>
        private void ShowWindow()
        {
            this.mainWindow.Show();
            this.WindowState = WindowState.Maximized;
            this.mainWindow.Activate();
        }


        /// <summary>
        /// Creates popup window for when application is in system tray
        /// </summary>
        /// <param name="volume"></param>
        private void ShowPopupWindow(Volume volume)
        {
            if (mainWindow.Visibility != Visibility.Visible)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    var popup = new PopupWindow(volume);
                    popup.Show();
                });
            }
        }


        /// <summary>
        /// Overrides close button to minimize to system tray
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            mainWindow.Hide();
        }


        /// <summary>
        /// When window is loading, it is then hidden to system tray.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartInSystemTray(object sender, RoutedEventArgs e)
        {
            mainWindow.Hide();
        }


        /// <summary>
        /// Clean up resources and exit application
        /// </summary>
        private void ExitApplication()
        {
            // Remove Unknown applications from sources list before saving to json
            sources.RemoveAll(Source => Source.ProcessName == "Unknown");
            Utilities.JsonUtil.WriteToSources(sources);
            Utilities.JsonUtil.WriteToVolumes(volumes);
            HotkeysManager.ShutdownSystemHook();
            if (notifyIcon != null)
            {
                notifyIcon.Dispose();
            }
            
            System.Windows.Application.Current.Shutdown();
        }
    }
}