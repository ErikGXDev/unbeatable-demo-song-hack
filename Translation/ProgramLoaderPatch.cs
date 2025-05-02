using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using static UnbeatableSongHack.Translation.ProgramLoader;

namespace UnbeatableSongHack.Translation
{
    public static class ProgramLoaderPatch
    {
        [HarmonyPatch(typeof(Yarn.Unity.YarnProject), "GetProgram")]
        [HarmonyPrefix]
        public static bool GetCustomProgram(Yarn.Unity.YarnProject __instance, ref Yarn.Program __result)
        {
            if (disableCustomTranslation)
            {
                return true;
            }

            if (ProgramIndex.programs.ContainsKey(__instance.name))
            {
                __result = ProgramIndex.programs[__instance.name];
                return false;
            }

            return true;
        }
        

        [HarmonyPatch(typeof(Yarn.Unity.Localization), "GetLocalizedString")]
        [HarmonyPrefix]
        public static bool GetCustomLocalizedString(Yarn.Unity.Localization __instance, string key, ref string __result)
        {

            if (disableCustomTranslation)
            {
                return true;
            }

            if (ProgramIndex.lines.ContainsKey(key))
            {
                __result = ProgramIndex.lines[key];
                return false;
            }


            return true;

        }
        
    }
}
