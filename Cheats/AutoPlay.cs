using System;
using System.Collections.Generic;
using System.Text;

namespace UnbeatableSongHack.Cheats
{
    public class AutoPlay
    {
        public static bool enabled = JeffBezosController.GetAssistMode() == 1;
        public static bool ToggleAutoPlay()
        {
            if (enabled)
            {
                Core.GetLogger().Msg("Auto Play enabled");
                JeffBezosController.SetAssistMode(0);
                enabled = false;
            }
            else
            {
                Core.GetLogger().Msg("Auto Play disabled");
                JeffBezosController.SetAssistMode(1);
                enabled = true;
            }
            return enabled;
        }
    }
}
