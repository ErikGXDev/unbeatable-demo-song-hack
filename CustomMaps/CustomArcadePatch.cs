using Arcade.UI.SongSelect;
using HarmonyLib;
using Rhythm;
using System;
using System.Collections.Generic;
using System.Text;
using static Rhythm.BeatmapIndex;
using static UnbeatableSongHack.CustomMaps.CustomArcade;

namespace UnbeatableSongHack.CustomMaps
{
    public static class CustomArcadePatch
    {

        // Patch the song function to return all (also hidden) songs,
        // so we can access hidden beatmaps
        [HarmonyPatch(typeof(BeatmapIndex), "GetVisibleSongs")]
        [HarmonyPrefix]
        public static bool GetVisibleSongsPatch(ref BeatmapIndex __instance, ref List<Song> __result)
        {

            Core.GetLogger().Msg("Getting all songs...");

            __result = __instance.GetAllSongs();

            return false;
        }

        // Patch to make the game load custom beatmaps on arcade db load
        [HarmonyPatch(typeof(ArcadeSongDatabase), "LoadDatabase")]
        [HarmonyPostfix]
        public static void LoadDatabasePatch(ArcadeSongDatabase __instance)
        {
            Core.GetLogger().Msg("Loading DB...");

            var packages = LocalDatabase.GetBeatmapItems(LocalDatabase.GetLocalBeatmapDirectory());
            foreach (var package in packages)
            {
                Core.GetLogger().Msg(package.Path);
                CustomArcade.AddBeatmapItemToArcadeList(ArcadeSongDatabase.Instance, package, localCategory);
            }


            var osupackages = LocalDatabase.GetBeatmapItems(LocalDatabase.GetOsuBeatmapDirectory());
            foreach (var package in osupackages)
            {
                Core.GetLogger().Msg(package.Path);
                CustomArcade.AddBeatmapItemToArcadeList(ArcadeSongDatabase.Instance, package, osuCategory);
            }

        }
        

        [HarmonyPatch(typeof(BeatmapIndex), "GetVisibleCategories")]
        [HarmonyPostfix]
        public static void GetVisibleCategoriesPatch(ref List<Category> __result)
        {
            // Actually put the categories in the game
            TryAddCustomCategory(customCategories);
            // Add the custom category to the list of visible categories
            foreach (Category category in customCategories)
            {
                if (!__result.Contains(category))
                {
                    __result.Add(category);
                }
            }

        }


        
        
    }
}
