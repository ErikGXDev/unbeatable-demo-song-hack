using HarmonyLib;
using Rhythm;

namespace UnbeatableSongHack.Cheats
{
    public class GodMode
    {
        public static bool enabled = false;

        public static bool ToggleGodMode()
        {
            enabled = !enabled;
            if (enabled)
            {
                Core.GetLogger().Msg("God Mode enabled");
            }
            else
            {
                Core.GetLogger().Msg("God Mode disabled");
            }

            return enabled;
        }
    }
}
