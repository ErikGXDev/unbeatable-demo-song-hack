using HarmonyLib;
using Rhythm;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnbeatableSongHack.Cheats
{
    public static class GodModePatch
    {
        [HarmonyPatch(typeof(RhythmController), "CountMiss", new Type[] { typeof(Height), typeof(AttackInfo) })]
        [HarmonyPrefix]
        public static bool DontCountMiss(Height height, AttackInfo attack, ref RhythmController __instance)
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
        

        [HarmonyPatch(typeof(RhythmController), "Miss", new Type[] { })]
        [HarmonyPrefix]
        public static bool DontMiss(ref RhythmController __instance)
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
