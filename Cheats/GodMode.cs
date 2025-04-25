using HarmonyLib;
using Rhythm;
using System;
using System.Collections.Generic;
using System.Text;

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

        [HarmonyPatch(typeof(RhythmController), "CountMiss", new Type[] { typeof(Height), typeof(AttackInfo) })]
        public class RhythmControllerPatchCountMiss
        {
            public static bool Prefix(Height height, AttackInfo attack, ref RhythmController __instance)
            {
                if (!GodMode.enabled)
                {
                    // Let the original function run
                    return true;
                }
                else
                {
                    // Just don't count the miss
                    attack.miss = false;
                    __instance.CountHit(height, attack);
                    return false;
                }

            }
        }

        [HarmonyPatch(typeof(RhythmController), "Miss", new Type[] { })]
        public class RhythmControllerPatchMiss
        {
            public static bool Prefix(ref RhythmController __instance)
            {
                if (!GodMode.enabled)
                {
                    return true;
                }
                else
                {
                    // Just don't count the miss
                    __instance.Hit(Height.None, true);
                    return false;
                }

            }
        }
    }
}
