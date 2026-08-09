using FMOD.Studio;
using FMODUnity;
using HarmonyLib;
using Minigames;
using Newtonsoft.Json;
using Overworld;
using Rhythm;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace UnbeatableSongHack.CustomMaps
{

    public class LocalBeatballPlayer
    {
        public static string loadCustomAudio = "";
        public static string loadCustomBeatmap = "";
        public static string loadCustomWeather = "";

        public static string targetBeatball = "Beatball (Optional) (Easy)";

        public static void AddBeatballSceneHandler()
        {
            SceneManager.sceneLoaded += InitializeBeatballScene;
        }

        public static void PlayBeatballFromFile(string beatmapPath = "C:\\Users\\Anwender\\Downloads\\beatball.txt")
        {
            string basePath = Path.GetDirectoryName(beatmapPath);

            string contents = File.ReadAllText(beatmapPath);
            Beatmap beatmap = LocalLoader.ReadBeatmap(contents);

            var audioFilename = beatmap.general.audioFilename;

            var tagData = LocalLoader.GetPropertyValue(contents, "Tags");
            
            Core.GetLogger().Msg("Tag data: " + tagData);

            if (!String.IsNullOrEmpty(tagData))
            {
                BeatballTags beatballTags = ReadBeatmapTags(tagData);
                
                Core.GetLogger().Msg("Tag data 2: " + beatballTags.Weather);

                string weather = beatballTags.Weather;

                loadCustomWeather = weather;

                Core.GetLogger().Msg("Setting weather to: " + weather);


            }

            string audioPath = LocalLoader.FindAudioPath(basePath, audioFilename);

            loadCustomAudio = audioPath.Replace("\\", "/");
            loadCustomBeatmap = contents;

            SceneManager.LoadScene("C2_BattingCage_Minigame");

        }

        public class BeatballTags
        {
            public string Weather;
        }

        public static BeatballTags ReadBeatmapTags(string tagString)
        {
            BeatballTags beatballTags = JsonConvert.DeserializeObject<BeatballTags>(tagString);

            return beatballTags;
        }

        public static void InitializeBeatballScene(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != "C2_BattingCage_Minigame")
            {
                return;
            }
            
           
            if (String.IsNullOrEmpty(loadCustomAudio) || String.IsNullOrEmpty(loadCustomBeatmap))
            {
                Core.GetLogger().Msg("No custom beatmap or audio loaded, skipping initialization.");
                return;
            }
            
            DayNightSystem dayNightSystem = GameObject.FindFirstObjectByType<DayNightSystem>();
                
            foreach (var weather in dayNightSystem.weather)
            {
                Core.GetLogger().Msg("Available weather: " + weather.name);
            }



            BeatballController[] beatballControllers = GameObject.FindObjectsOfType<BeatballController>(true);

            foreach (BeatballController controller in beatballControllers)
            {

                Core.GetLogger().Msg("Found BeatballController: " + controller.gameObject.name);

                if (controller.gameObject.name == targetBeatball)
                {
                    Core.GetLogger().Msg("Target Beatball, initializing with custom beatmap.");

                    controller.beatmapAsset = new TextAsset(loadCustomBeatmap);
                    controller.showTutorial = false;
                    controller.gameObject.SetActive(true);
                    
                    controller.OnSongStart.AddListener(AddCustomWeather);
                    
                    //PatchInstanceWithAudio(controller);

                    continue;
                } else
                {
                    controller.enabled = false;
                    foreach (Transform child in controller.gameObject.GetComponentsInChildren<Transform>(true))
                    {
                        child.gameObject.SetActive(false);
                    }
                    controller.OnSongStart.AddListener(DisableBeatballObjects);
                }
            }

            

        }

        public static void AddCustomWeather()
        {
            if (!String.IsNullOrEmpty(loadCustomWeather))
            {
                DayNightSystem dayNightSystem = GameObject.FindFirstObjectByType<DayNightSystem>();
                
                dayNightSystem.SetWeatherOverride(loadCustomWeather);

                Core.GetLogger().Msg("Weather override set to: " + loadCustomWeather);

                loadCustomWeather = "";
            }
        }

        public static void DisableBeatballObjects()
        {
            BeatballController[] beatballControllers = GameObject.FindObjectsOfType<BeatballController>(true);
            foreach (BeatballController controller in beatballControllers)
            {
                if (controller.gameObject.name == targetBeatball)
                {
                    continue;
                }
                Core.GetLogger().Msg("Disabling BeatballController: " + controller.gameObject.name);
                
                if (controller.gameObject.activeSelf)
                {
                    controller.gameObject.SetActive(false);
                }
                

            }
        }
        
        public static void PatchInstanceWithAudio(BeatballController __instance)
        {
            Traverse controllerTraverse = Traverse.Create(__instance);

            var trackerInstance = controllerTraverse.Field("tracker").Field("instance").GetValue<EventInstance>();
            trackerInstance.release();

            trackerInstance = RuntimeManager.CreateInstance("event:/beatmap");
            RhythmTracker.PrepareInstance(trackerInstance, PlaySource.FromFile, LocalBeatballPlayer.loadCustomAudio);
                    
                    
            controllerTraverse.Field("tracker").Field("instance").SetValue(trackerInstance);

            trackerInstance.start();

            Core.GetLogger().Msg("Custom audio loaded successfully.");
        }

    }

    public class LocalBeatballPlayerPatch
    {
        [HarmonyPatch(typeof(BeatballController), "Initialize")]
        [HarmonyPostfix]
        public static void BeatballControllerPatch(BeatballController __instance)
        {

            try
            {

                if (!(String.IsNullOrEmpty(LocalBeatballPlayer.loadCustomAudio))) {

                    Core.GetLogger().Msg("Loading custom audio: " + LocalBeatballPlayer.loadCustomAudio);

                    Traverse controllerTraverse = Traverse.Create(__instance);

                    var trackerInstance = controllerTraverse.Field("tracker").Field("instance").GetValue<EventInstance>();
                    trackerInstance.release();

                    trackerInstance = RuntimeManager.CreateInstance("event:/beatmap");
                    RhythmTracker.PrepareInstance(trackerInstance, PlaySource.FromFile, LocalBeatballPlayer.loadCustomAudio);
                    
                    
                    controllerTraverse.Field("tracker").Field("instance").SetValue(trackerInstance);

                    trackerInstance.start();

                    Core.GetLogger().Msg("Custom audio loaded successfully.");
                }
                else
                {
                    Core.GetLogger().Msg("No custom audio to load.");
                }
            }
            catch (Exception e)
            {
                Core.GetLogger().Msg("Failed to patch BeatballController: " + e); 
            }
        }
        
        
    }
    
    
}
