﻿using BepInEx;
//using MelonLoader;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using HarmonyLib;
using Rhythm;
using System.Runtime.InteropServices;
using UnbeatableSongHack.Cheats;
using BepInEx.Logging;
using UnbeatableSongHack.Translation;
using UnbeatableSongHack.CustomMaps;

// "Unbeatable Demo Song Hack" Mod by Erik G - 2025

//[assembly: MelonInfo(typeof(UnbeatableSongHack.Core), "UnbeatableSongHack", "1.1.0", "Erik G", null)]
//[assembly: MelonGame("D-CELL GAMES", "UNBEATABLE [DEMO]")]

namespace UnbeatableSongHack
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Core : BaseUnityPlugin //MelonMod
    {
        private const string modGUID = "erikg.unbeatable.songhack";
        private const string modName = "UnbeatableSongHack";
        private const string modVersion = "1.1.0";

        Harmony harmony = new Harmony(modGUID);

        public void Awake()
        {

            BepLogger = base.Logger;
            LoggerInstance = new LoggerInstanceCompat();


            LoggerInstance.Msg("Initialized Song Hack!");

            // Harmony
            Type[] patchClasses =
            {
                typeof(CustomArcadePatch),
                typeof(GodModePatch),
                typeof(LocalPlayerPatch),
                typeof(ProgramLoaderPatch),
                typeof(FixMouse),
            };

            foreach (Type patchClass in patchClasses)
            {
                harmony.PatchAll(patchClass);
            }


            ProgramLoader.LoadLocalTranslations();
        }

        public void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                // Stop all events
                RuntimeManager.GetBus("bus:/").stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                //FetchAllEvents();
            }
        }


        public SongHackUI songHackUI = new SongHackUI();

        public void OnGUI()
        {
            songHackUI.DrawUI();
        }

        // Let other classes access the logger
        internal static ManualLogSource BepLogger;

        public class LoggerInstanceCompat()
        {
            public void Msg(string message)
            {
                BepLogger.LogInfo(message);
            }
        }

        public static LoggerInstanceCompat LoggerInstance;

        public static LoggerInstanceCompat GetLogger()
        {
            return LoggerInstance;
        }

        /* -- Melon Loader
        public static MelonLogger.Instance GetLogger()
        {
            MelonBase core = Core.FindMelon("UnbeatableSongHack", "Erik G");
            return core.LoggerInstance;
        }*/


        /*
        [HarmonyPatch(typeof(RhythmTracker), "HandleCreateProgrammerSound", new Type[] { typeof(EventInstance), typeof(IntPtr) })]
        public static class RhythmTrackerPatch
        {

            public static bool Prefix(EventInstance instance, IntPtr parameterPtr)
            {
                var LoggerInstance = Core.GetLogger();

                //LoggerInstance.Msg("HandleCreateProgrammerSound called");

                instance.getUserData(out var h_userdata);
                PlayInfo h_playInfo = GCHandle.FromIntPtr(h_userdata).Target as PlayInfo;

                if (h_playInfo.source == PlaySource.FromTable)
                {
                    string key = Marshal.PtrToStringUni(h_playInfo.key);

                    // Log the key of the sound being played
                    // Useful since we can't find the sound name otherwise
                    // Was used to match the names to the unnamed soundtrack files
                    LoggerInstance.Msg("Now playing: " + key);

                    //SOUND_INFO h_info;
                    //RESULT h_soundInfo = RuntimeManager.StudioSystem.getSoundInfo(key, out h_info);

                    //LoggerInstance.Msg("SoundInfo: " + (h_info.name));




                }

                //instance.getVolume(out float volume);
                //LoggerInstance.Msg("Volume: " + volume.ToString());

                return true;
            }
        }*/




        // Doesn't really work, but it sometimes logs when something is played
        /*
        [HarmonyPatch(typeof(FMODUnity.RuntimeManager), "PlayOneShot", new Type[] { typeof(string), typeof(Vector3) })]
        public class RuntimeManagerPatch
        {
            public static bool Prefix(string path)
            {
                var LoggerInstance = Core.GetLogger();

                LoggerInstance.Msg("[Patch] Playing: " + path);

                return true;
            }
        }*/

        /*
        // I honestly don't know what this does, but it seems to show me more difficulties.
        [HarmonyPatch(typeof(BeatmapIndex), "DifficultyIsSelectable", new Type[] { typeof(string)})]
        public class BeatmapIndexDiffPatch
        {
            public static bool Prefix(string difficulty, ref bool __result)
            {
                __result = true;
                return false;
            }
        }*/


        // Function to add arbitrary beatmap songs into the arcade list
        // Might be useful for some custom beatmaps, although some info is fetched from
        // TextAssets that we cannot create/access through mods.
        // It may be a mess.



    }







}