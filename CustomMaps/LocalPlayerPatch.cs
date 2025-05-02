using FMOD.Studio;
using HarmonyLib;
using Rhythm;
using System;
using System.Collections.Generic;
using System.Text;
using static UnbeatableSongHack.CustomMaps.LocalPlayer;

namespace UnbeatableSongHack.CustomMaps
{
    public static class LocalPlayerPatch
    {

        // Detect if the beatmap is a custom one
        // Since ParseBeatmap attempts to load a TextAsset (which we cant create)
        // we have to patch the function and make it load from text contents instead.
        [HarmonyPatch(typeof(BeatmapParser), "ParseBeatmap", new Type[] { })]
        [HarmonyPrefix]
        public static bool ParseBeatmapPatch(ref BeatmapParser __instance)
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
        


        // Make the game play local files
        [HarmonyPatch(typeof(RhythmTracker), "PrepareInstance", new Type[] { typeof(EventInstance), typeof(PlaySource), typeof(string) })]
        [HarmonyPrefix]
        public static bool PrepareInstancePatch(EventInstance instance, ref PlaySource source, ref string key)
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


        // Patch to load audio from file
        // Since the game really wants to load from its own audio table,
        // we need to patch this to load from a file instead
        // That way we can make the game load any sound file (and in this case,
        // the one from our custom level)
        [HarmonyPatch(typeof(RhythmTracker), "PreloadFromTable")]
        [HarmonyPrefix]
        public static bool PreloadFromTablePatch(string key, ref RhythmTracker __instance)
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
}
