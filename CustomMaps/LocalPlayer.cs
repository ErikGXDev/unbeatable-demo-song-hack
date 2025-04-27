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
            public string filePath = "./none.txt";
            public string audioKey = "EMPTY DIARY";

            public CustomProgression(string textFile)
            {
                this.filePath = textFile;

                // Look for an audio.mp3 in the same directory as the beatmap
                // Might as well use the audioFilename property from the beatmap itself later
                string directory = Path.GetDirectoryName(filePath);

                var audioPath = Path.Combine(directory, "audio.mp3");

                // If an audio is found, set it to the audio path
                var audioTitle = "EMPTY DIARY";
                if (File.Exists(audioPath))
                {
                    audioTitle = audioPath;
                }

                this.audioKey = audioTitle;
                this.stageScene = "TrainStationRhythm";
            }

            public string GetBeatmapPath()
            {
                // Placeholder
                return "";
            }
            public string GetSongName()
            {
                return audioKey;
            }

            public string GetDifficulty()
            {
                // Placeholder
                return "CUSTOMDIFF";
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

            string audioPath = Encoder.DecodeAudioName(beatmapItem.Path);


            CustomProgression customProgression = new CustomProgression(filePath);

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

                    var contents = File.ReadAllText(progression.filePath);

                    BeatmapParserEngine beatmapParserEngine = new BeatmapParserEngine();
                    var beatmap = ScriptableObject.CreateInstance<Beatmap>();
                    beatmapParserEngine.ReadBeatmap(contents, ref beatmap);

                    // Set the beatmap to the one by the parser
                    __instance.beatmap = beatmap;
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

                Core.GetLogger().Msg("Adding song to list: " + beatmapItem.Path);

                // This whole part would be needed for AddSongList()
                beatmapItem.Beatmap = new Beatmap();
                beatmapItem.Beatmap.metadata.title = "Custom " + name;
                beatmapItem.Beatmap.metadata.titleUnicode = "Custom " + name;
                beatmapItem.Beatmap.metadata.artist = "Not You";
                beatmapItem.Beatmap.metadata.artistUnicode = "Not You";
                beatmapItem.Beatmap.metadata.tagData.Level = 10;


                // This was a test and is not needed
                //AddSongToArcadeList(songList,beatmapItem);

                Core.GetLogger().Msg("Added Key: " + key);


                arcadeBGMManger.PlaySongPreview(beatmapItem);


            }

        }


    }
}
