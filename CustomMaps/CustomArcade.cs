using Arcade.UI;
using Arcade.UI.SongSelect;
using HarmonyLib;
using Rhythm;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;
using static Arcade.UI.SongSelect.ArcadeSongDatabase;
using static MelonLoader.MelonLogger;
using static Rhythm.BeatmapIndex;

namespace UnbeatableSongHack.CustomMaps
{
    public class CustomArcade
    {

        public static Category customCategory = new Category("LOCAL", "Local songs", 7);

        public static void AddBeatmapItemToArcadeList(ArcadeSongDatabase instance, BeatmapItem item)
        {
            var LoggerInstance = Core.GetLogger();

            item.Unlocked = true;

            BeatmapIndex beatmapIndex = BeatmapIndex.defaultIndex;

            // Manipulate the beatmap index
            Traverse beatmapIndexTraverse = Traverse.Create(beatmapIndex);


            //TryAddCustomCategory();


            var songNames = beatmapIndexTraverse.Field("_songNames").GetValue<List<string>>();

            var songName = item.Song.name;

            // Check if the song name is already in the list
            // If not, add it to the list and the dictionary
            if (!songNames.Contains(songName))
            {
                songNames.Add(songName);
                beatmapIndexTraverse.Field("_songNames").SetValue(songNames);

                var songs = beatmapIndexTraverse.Field("_songs").GetValue<Dictionary<string, Song>>();
                songs.TryAdd(songName, item.Song);


                var categorySongs = beatmapIndexTraverse.Field("_categorySongs").GetValue<Dictionary<Category, List<Song>>>();
                categorySongs[customCategory].Add(item.Song);
                beatmapIndexTraverse.Field("_categorySongs").SetValue(categorySongs);
            }
            else
            {
                // If the song already exists, we need to get it from the dictionary to avoid creating a new one
                // (I think), would probably work without this but it doesn't hurt
                item.Song = beatmapIndexTraverse.Field("_songs").GetValue<Dictionary<string, Song>>()[songName];
            }


            // Manipulate the song object
            Traverse songTraverse = Traverse.Create(item.Song);

            // Set song to custom category
            songTraverse.Field("_category").SetValue(customCategory);

            // Add the beatmap to the song
            var songBeatmaps = songTraverse.Field("_beatmaps").GetValue<Dictionary<string, BeatmapInfo>>();

            if (songBeatmaps == null)
            {
                // If the beatmaps dictionary is null, create a new one
                songBeatmaps = new Dictionary<string, BeatmapInfo>();
            }

            // Add the beatmap to the song difficulty registry
            var beatmapDifficulty = item.BeatmapInfo.difficulty;

            songBeatmaps.TryAdd(beatmapDifficulty, item.BeatmapInfo);
            songTraverse.Field("_beatmaps").SetValue(songBeatmaps);

            var songDifficulties = songTraverse.Field("_difficulties").GetValue<List<string>>();

            if (songDifficulties == null)
            {
                // If the difficulties list is null, create a new one
                songDifficulties = new List<string>();
            }

            songDifficulties.Add(item.BeatmapInfo.difficulty);
            songTraverse.Field("_difficulties").SetValue(songDifficulties);

            //Core.GetLogger().Msg("Stage: " + item.Song.stageScene);

            //item.Song.Beatmaps.TryAdd(item.BeatmapInfo.difficulty, item.BeatmapInfo);
            //item.Song.Difficulties.Add(item.BeatmapInfo.difficulty);


            // Finally add the song to the full song database
            Traverse traverse = Traverse.Create(instance);
            Dictionary<string, BeatmapItem> songList = traverse.Field("_songDatabase").GetValue<Dictionary<string, BeatmapItem>>();

            var key = item.Path;

            songList.TryAdd(key, item);
            traverse.Field("_songDatabase").SetValue(songList);

            instance.RefreshSongList();

            LoggerInstance.Msg("Adding song to list: " + item.Path);


        }

        public static void TryAddCustomCategory()
        {
            BeatmapIndex beatmapIndex = BeatmapIndex.defaultIndex;

            Traverse beatmapIndexTraverse = Traverse.Create(beatmapIndex);

            var beatmapIndexCategories = beatmapIndexTraverse.Field("categories").GetValue<List<Category>>();

            // Check if the custom category already exists
            if (!beatmapIndexCategories.Contains(customCategory))
            {
                // If not, add it to the list

                beatmapIndexCategories.Add(customCategory);
                beatmapIndexTraverse.Field("categories").SetValue(beatmapIndexCategories);

                Core.GetLogger().Msg("Added category " + customCategory.Name);


                /*var categoriesByName = beatmapIndexTraverse.Field("CategoriesByName").GetValue<Dictionary<string, Category>>();
                Core.GetLogger().Msg("DEBUG");
                categoriesByName.TryAdd(customCategory.Name, customCategory);
                Core.GetLogger().Msg("DEBUG");
                beatmapIndexTraverse.Field("CategoriesByName").SetValue(categoriesByName);*/


                var categorySongs = beatmapIndexTraverse.Field("_categorySongs").GetValue<Dictionary<Category, List<Song>>>();
                categorySongs.TryAdd(customCategory, new List<Song>());
                beatmapIndexTraverse.Field("_categorySongs").SetValue(categorySongs);


            }



            /*
            Traverse songDatabaseTraverse = Traverse.Create(ArcadeSongDatabase.Instance);

            var selectableCategories = songDatabaseTraverse.Field("SelectableCategories").GetValue<List<Category>>();
            if (!selectableCategories.Contains(customCategory))
            {
                selectableCategories.Add(customCategory);
                songDatabaseTraverse.Field("SelectableCategories").SetValue(selectableCategories);
            }*/

        }


        // Patch to make the game load custom beatmaps on arcade db load
        [HarmonyPatch(typeof(ArcadeSongDatabase), "LoadDatabase")]
        public class LoadDatabasePatch
        {
            public static void Postfix(ArcadeSongDatabase __instance)
            {
                Core.GetLogger().Msg("Loading DB...");

                TryAddCustomCategory();

                var packages = LocalDatabase.GetLocalBeatmapItems();
                foreach (var package in packages)
                {
                    Core.GetLogger().Msg(package.Path);
                    CustomArcade.AddBeatmapItemToArcadeList(ArcadeSongDatabase.Instance, package);
                }


            }
        }

        [HarmonyPatch(typeof(BeatmapIndex), "GetVisibleCategories")]
        public class GetVisibleCategoriesPatch
        {
            public static void Postfix(ref List<Category> __result)
            {
                // Add the custom category to the list of visible categories
                if (!__result.Contains(customCategory))
                {
                    __result.Add(customCategory);
                }
            }
        }


    }
}
