using Arcade.UI.SongSelect;
using Arcade.UI;
using Rhythm;
using System;
using System.Collections.Generic;
using System.Text;
using static Arcade.UI.SongSelect.ArcadeSongDatabase;
using static Rhythm.BeatmapIndex;
using UnityEngine;
using HarmonyLib;
using FMOD.Studio;

namespace UnbeatableSongHack.CustomMaps
{
    public class LocalPlayer
    {


        // "Progessions" are some kind of class 
        // that determine how the game behaves in stages
        // They are used to load the beatmap and the audio
        public class CustomProgression : IProgression
        {
            public string stageScene = "TrainStationRhythm";

            public BeatmapItem beatmapItem;

            public CustomProgression(BeatmapItem beatmapItem)
            {
                this.beatmapItem = beatmapItem;

                this.stageScene = "TrainStationRhythm";
            }

            public string GetBeatmapPath()
            {
                // Placeholder
                return beatmapItem.Path;
            }
            public string GetSongName()
            {
                return beatmapItem.Song.name;
            }

            public string GetDifficulty()
            {
                // Placeholder
                return "UNBEATABLE";
            }

            public void Finish(string sceneIndex)
            {
                LevelManager.LoadLevel("ScoreScreenArcadeMode");
            }

            public void Retry()
            {
                LevelManager.LoadLevel(stageScene);
            }

            public void Back()
            {
                LevelManager.LoadLevel(JeffBezosController.arcadeMenuScene);
            }
        }

        public static void PlayBeatmapFromFile(string filePath = "C:\\Users\\Anwender\\Downloads\\testmap.txt")
        {


            if(!LocalLoader.LoadBeatmapFromFile(filePath, out BeatmapItem beatmapItem))
            {
                Core.GetLogger().Msg("Beatmap not found: " + filePath);
                return;
            }

            CustomProgression customProgression = new CustomProgression(beatmapItem);

            JeffBezosController.rhythmProgression = customProgression;

            LevelManager.LoadLevel(customProgression.stageScene);

        }

        // Detect if the beatmap is a custom one
        // Since ParseBeatmap attempts to load a TextAsset (which we cant create)
        // we have to patch the function and make it load from text contents instead.
        [HarmonyPatch(typeof(BeatmapParser), "ParseBeatmap", new Type[] { })]
        public class BeatmapParserPatch
        {
            public static bool Prefix(ref BeatmapParser __instance)
            {
                if (JeffBezosController.rhythmProgression is CustomProgression progression)
                {
                    Core.GetLogger().Msg("Loading custom beatmap...");


                    __instance.beatmapIndex = BeatmapIndex.defaultIndex;



                    // Set the beatmap to the one by the parser
                    __instance.beatmap = progression.beatmapItem.Beatmap;
                    __instance.audioKey = progression.GetSongName();
                    __instance.beatmapPath = progression.GetBeatmapPath();

                    return false;
                }
                return true;
            }
        }


        // Make the game play local files
        [HarmonyPatch(typeof(RhythmTracker), "PrepareInstance", new Type[] { typeof(EventInstance), typeof(PlaySource), typeof(string) })]
        public class RhythmTrackerPreparePatch
        {
            public static bool Prefix(EventInstance instance, ref PlaySource source, ref string key)
            {
                if (key.StartsWith("__CUSTOM") && key.Contains("."))
                {

                    key = Encoder.DecodeAudioName(key);
                    

                    if (File.Exists(key))
                    {
                        Core.GetLogger().Msg("Loading custom audio: " + key);
                        source = PlaySource.FromFile;
                    }
                    else
                    {
                        Core.GetLogger().Msg("Custom audio not found: " + key);
                    }
                }
                return true;
            }
        }


        // Patch to load audio from file
        // Since the game really wants to load from its own audio table,
        // we need to patch this to load from a file instead
        // That way we can make the game load any sound file (and in this case,
        // the one from our custom level)
        [HarmonyPatch(typeof(RhythmTracker), "PreloadFromTable")]
        public class RhythmTrackerLoadPatch
        {
            public static bool Prefix(string key, ref RhythmTracker __instance)
            {
                if (key.Contains("."))
                {
                    if (File.Exists(key))
                    {
                        __instance.PreloadFromFile(key);
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }

                return true;
            }
        }



        public static void PlayFromKey(string name)
        {
            var arcadeBGMManger = ArcadeBGMManager.Instance;
            var songList = ArcadeSongDatabase.Instance;
            var beatmapIndex = BeatmapIndex.defaultIndex;


            if (arcadeBGMManger != null && songList != null)
            {


                Core.GetLogger().Msg("Adding key: " + name);

                var beatmapItem = new BeatmapItem();

                beatmapItem.Path = "Custom_" + name + "/Beginner";
                var key = beatmapItem.Path;



                beatmapItem.Song = new Song(name);
                beatmapItem.Song.stageScene = "TrainStationRhythm";

                beatmapItem.Unlocked = true;



                beatmapItem.BeatmapInfo = new BeatmapInfo(null, "Beginner");


                beatmapItem.Highscore = new HighScoreItem(key, 0, 0f, 0, cleared: false, new Dictionary<string, int>(), Modifiers.None);


                beatmapItem.Beatmap = new Beatmap();

                // This was a test and is not needed

                // This whole part would be needed for AddSongList()

                //beatmapItem.Beatmap.metadata.title = "Custom " + name;
                //beatmapItem.Beatmap.metadata.titleUnicode = "Custom " + name;
                //beatmapItem.Beatmap.metadata.artist = "Not You";
                //beatmapItem.Beatmap.metadata.artistUnicode = "Not You";
                //beatmapItem.Beatmap.metadata.tagData.Level = 10;
                //AddSongToArcadeList(songList,beatmapItem);

                Core.GetLogger().Msg("Playing Key: " + key);


                arcadeBGMManger.PlaySongPreview(beatmapItem);


            }

        }


    }
}
