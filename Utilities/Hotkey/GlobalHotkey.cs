using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Oar_Audio.Utilities.Hotkey
{
    public class GlobalHotkey
    {
        public ModifierKeys Modifier { get; set; }
        public ModifierKeys Modifier2 { get; set; }
        public Key Key { get; set; }
        public Action Callback { get; set; }
        public bool CanExecute { get; set; }

        public GlobalHotkey(ModifierKeys Modifier, Key Key, Action Callback, bool CanExecute)
        {
            this.Modifier = Modifier;
            this.Key = Key;
            this.Callback = Callback;
            this.CanExecute = CanExecute;
        }

        public GlobalHotkey(ModifierKeys Modifier, ModifierKeys Modifier2, Key Key, Action Callback, bool CanExecute)
        {
            this.Modifier = Modifier;
            this.Modifier2 = Modifier2;
            this.Key = Key;
            this.Callback = Callback;
            this.CanExecute = CanExecute;
        }
    }
}
