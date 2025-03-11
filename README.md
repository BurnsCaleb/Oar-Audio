<h1>Oar Audio</h1>
<h3>Output Audio Routing Manager</h3>

<br>

<h4><b><em>The Problem</em></b></h4>
<p>
  Have you ever wanted to change the volume of a specific application while leaving the other audio applications the same volume? 
  Assume your playing a game and listening to music, maybe the music is too loud and you can't hear the game or vice versa. 
  Going back to the desktop to open the volume mixer and manually adjust the applications volume can be tedius and rather annoying.
</p>

<p>
  I built this application with the goal of allowing users to group applications that output audio to easily be able to adjust the volume of a group using hotkeys.
</p>

<br>

<h4><b><em>How It Works</em></b></h4>
<p>
  The application gathers active audio sessions and monitors for newly created sessions. These sessions are processed and stored in a configuration file to keep them grouped based on what the user has specified.
  It then groups these sessions and loops through the entire group updating the volumes when prompted.
</p>

<br>

<h4><b><em>The GUI</em></b></h4>
<p>
  Only active audio sessions outputting audio are shown in the GUI. But, there is an option to show every single audio session stored in the configuration file. 
  There is also an option to change keybinds to increase, decrease, and mute the group volumes. By right-clicking a session, you get four options. You can change 
  the display name and reset it to it's default value. You have an option to move the session to one of the four groups. And you have the option to hide the audio 
  session from displaying in the GUI. Lastly, the GUI displays the volume level of each group at the bottom. 
</p>
