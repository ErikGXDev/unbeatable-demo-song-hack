using Arcade.UI.SongSelect;
using HarmonyLib;
using Rhythm;
using System;
using System.Collections.Generic;
using System.Text;
using static Arcade.UI.SongSelect.ArcadeSongDatabase;

namespace UnbeatableSongHack.CustomMaps
{
    public class CustomArcade
    {


        public static void AddBeatmapItemToArcadeList(ArcadeSongDatabase instance, BeatmapItem item)
        {
            var LoggerInstance = Core.GetLogger();

            item.Unlocked = true;

            BeatmapIndex beatmapIndex = BeatmapIndex.defaultIndex;

            // Manipulate the beatmap index
            Traverse beatmapIndexTraverse = Traverse.Create(beatmapIndex);

            // Add difficulty to the list of beatmapIndex difficulties
            // Most likely not needed
            /*
            var difficulties = beatmapIndexTraverse.Field("_difficulties").GetValue<HashSet<string>>();
            if (!difficulties.Contains(item.BeatmapInfo.difficulty))
            {
                difficulties.Add(item.BeatmapInfo.difficulty);

                beatmapIndexTraverse.Field("_difficulties").SetValue(difficulties);
            }*/

            //var selectableDifficulties = beatmapIndexTraverse.Field("_selectableDifficulties").GetValue<HashSet<string>>();


            var songNames = beatmapIndexTraverse.Field("_songNames").GetValue<List<string>>();

            var songName = item.Song.name;

            // Check if the song name is already in the list
            // If not, add it to the list and the dictionary
            if (!songNames.Contains(songName))
            {
                songNames.Add(songName);
                beatmapIndexTraverse.Field("_songNames").SetValue(songNames);

                var songs = beatmapIndexTraverse.Field("_songs").GetValue<Dictionary<string, BeatmapIndex.Song>>();
                songs.TryAdd(songName, item.Song);
            }
            else
            {
                // If the song already exists, we need to get it from the dictionary to avoid creating a new one
                // (I think), would probably work without this but it doesn't hurt
                item.Song = beatmapIndexTraverse.Field("_songs").GetValue<Dictionary<string, BeatmapIndex.Song>>()[songName];
            }


            // Manipulate the song object
            Traverse songTraverse = Traverse.Create(item.Song);

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
    }
}
