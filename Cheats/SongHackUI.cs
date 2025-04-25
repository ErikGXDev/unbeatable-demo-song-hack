using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections.Generic;
using System.Text;
using UnbeatableSongHack.CustomMaps;
using UnityEngine;

using static UnbeatableSongHack.Cheats.TreeExplorer;


namespace UnbeatableSongHack.Cheats
{
    public class SongHackUI
    {


        public static string lastEventPath = "";

        public TreeNode rootNode = new TreeNode("Root");
        public Dictionary<string, bool> expandedStates = new Dictionary<string, bool>();

        public Rect windowRect = new Rect(20, 20, 400, 600);
        public Vector2 scrollPosition = new Vector2(0, 0);

        public string textInput = "";


        public List<FMOD.Studio.EventDescription> eventDescriptions = new List<EventDescription>();
        public List<string> strings = new List<string>();

        // Get the events from the bank
        // These can be used to play single audios in game through the tree menu
        FMOD.Studio.EventDescription[] GetBankEvents(Bank bank)
        {
            Core.GetLogger().Msg("\n> Getting Bank Events");
            bank.getEventList(out FMOD.Studio.EventDescription[] eventDescs);
            foreach (FMOD.Studio.EventDescription eventDesc in eventDescs)
            {
                eventDesc.getPath(out string eventPath);
                Core.GetLogger().Msg(eventPath);

                eventDescriptions.Add(eventDesc);
            }
            return eventDescs;
        }

        // Read all string events from the bank
        void GetBankStrings(Bank bank)
        {
            Core.GetLogger().Msg("\n> Getting Bank Strings");
            bank.getStringCount(out int stringCount);
            Core.GetLogger().Msg("String Count: " + stringCount);

            for (int i = 0; i < stringCount; i++)
            {
                bank.getStringInfo(i, out FMOD.GUID Id, out string stringPath);
                Core.GetLogger().Msg("String ID: " + Id);
                Core.GetLogger().Msg("String Path: " + stringPath);
                Core.GetLogger().Msg("---");

                // Strings zur Liste hinzufügen
                if (!string.IsNullOrEmpty(stringPath))
                {
                    strings.Add(stringPath);
                }
            }
        }

        // Build the event tree from the event descriptions and strings
        void FetchAllEvents()
        {
            // Listen leeren
            eventDescriptions.Clear();
            strings.Clear();
            rootNode = new TreeNode("Root");

            RuntimeManager.StudioSystem.getBankList(out Bank[] banks);

            foreach (Bank bank in banks)
            {
                // Get the name of the bank
                bank.getPath(out string bankPath);
                // Log the name of the bank
                Core.GetLogger().Msg("--- Bank Path: " + bankPath);

                // It's Master.strings! Read the strings from it!
                if (bankPath.EndsWith("strings"))
                {
                    GetBankStrings(bank);
                }

                try
                {
                    GetBankEvents(bank);
                }
                catch (System.Exception e)
                {
                    Core.GetLogger().Msg("Error fetching events: " + e.Message);
                }
            }

            BuildEventTree(out rootNode, eventDescriptions, strings);
        }


        public void DrawUI()
        {
            windowRect = GUI.Window(0, windowRect, DoWindow, "Song Hack");

            if (windowRect.width == 0)
            {
                if (GUI.Button(new Rect(10, 10, 32, 32), "O"))
                {
                    windowRect = new Rect(20, 20, 400, 600);
                }
            }

            // Draw MOD UI
            void DoWindow(int windowId)
            {
                GUIStyle compactButtonStyle = new GUIStyle(GUI.skin.button);
                compactButtonStyle.alignment = TextAnchor.MiddleLeft;
                compactButtonStyle.fontSize = 14;

                GUIStyle compactEnabledButtonStyle = new GUIStyle(compactButtonStyle);
                compactEnabledButtonStyle.normal.textColor = Color.green;

                GUIStyle folderStyle = new GUIStyle(GUI.skin.button);
                folderStyle.alignment = TextAnchor.MiddleLeft;
                folderStyle.fontSize = 14;
                folderStyle.normal.textColor = Color.yellow;

                GUIStyle leafStyle = new GUIStyle(GUI.skin.button);
                leafStyle.alignment = TextAnchor.MiddleLeft;
                leafStyle.fontSize = 14;
                leafStyle.normal.textColor = Color.white;

                GUIStyle stringStyle = new GUIStyle(GUI.skin.button);
                stringStyle.alignment = TextAnchor.MiddleLeft;
                stringStyle.fontSize = 14;
                stringStyle.normal.textColor = Color.cyan;

                GUIStyle playableStringStyle = new GUIStyle(GUI.skin.button);
                playableStringStyle.alignment = TextAnchor.MiddleLeft;
                playableStringStyle.fontSize = 14;
                playableStringStyle.normal.textColor = Color.green;

                GUI.DragWindow(new Rect(0, 0, 10000, 20));

                // Create button to stop all events
                if (GUI.Button(new Rect(10, 25, 110, 30), "Stop All Events"))
                {
                    RuntimeManager.GetBus("bus:/").stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

                }

                // Create button to get all events
                if (GUI.Button(new Rect(120, 25, 110, 30), "Get All Events"))
                {
                    FetchAllEvents();
                    Core.GetLogger().Msg($"Fetched {eventDescriptions.Count} events and {strings.Count} strings.");
                }

                // Play an event from the text input
                if (GUI.Button(new Rect(230, 25, 110, 30), "Sound from Text"))
                {
                    if (!string.IsNullOrEmpty(textInput))
                    {
                        RuntimeManager.PlayOneShot(textInput);
                        lastEventPath = textInput;
                        Core.GetLogger().Msg("Playing: " + textInput);
                    }
                }

                if (GUI.Button(new Rect(230, 55, 110, 30), "Sound from Key"))
                {
                    if (!string.IsNullOrEmpty(textInput))
                    {
                        CustomMaps.Player.PlayFromKey(textInput);
                        lastEventPath = "Key: " + textInput;
                        Core.GetLogger().Msg("Playing Key: " + textInput);
                    }
                }

                if (GUI.Button(new Rect(130, 55, 100, 30), "Play File Level"))
                {
                    if (!string.IsNullOrEmpty(textInput))
                    {
                        CustomMaps.Player.LoadBeatmapFromFile(textInput);
                    }
                    else
                    {
                        CustomMaps.Player.LoadBeatmapFromFile();
                    }
                }

                if (GUI.Button(new Rect(130, 85, 100, 30), "Get PKGs"))
                {
                    try
                    {
                        var packages = DatabaseLoader.GetPackages();
                        Core.GetLogger().Msg(packages.ToString());
                    }
                    catch (Exception e)
                    {
                        Core.GetLogger().Msg("Error fetching packages: " + e.Message);
                        Core.GetLogger().Msg(e.StackTrace);
                    }
                }

                if (GUI.Button(new Rect(230, 85, 100, 30), "Get Loc PKGs"))
                {
                    try
                    {
                        var packages = DatabaseLoader.GetLocalBeatmapItems();
                        foreach (var package in packages)
                        {
                            Core.GetLogger().Msg(package.Path);
                            CustomArcade.AddBeatmapItemToArcadeList(Arcade.UI.SongSelect.ArcadeSongDatabase.Instance, package);
                        }
                    }
                    catch (Exception e)
                    {
                        Core.GetLogger().Msg("Error fetching packages: " + e.Message);
                        Core.GetLogger().Msg(e.StackTrace);
                    }
                }


                if (GUI.Button(new Rect(340, 25, 50, 30), "X"))
                {
                    windowRect = new Rect(-100, -100, 0, 0);
                }


                GUI.Label(new Rect(10, 180, 360, 30), "Last Event: " + lastEventPath);
                textInput = GUI.TextField(new Rect(10, 210, 360, 25), textInput);

                GUIStyle godModeButton;
                if (GodMode.enabled)
                {
                    godModeButton = compactEnabledButtonStyle;
                }
                else
                {
                    godModeButton = compactButtonStyle;
                }

                // God Mode toggle
                if (GUI.Button(new Rect(10, 120, 100, 25), "Toggle God", godModeButton))
                {
                    bool isGodMode = GodMode.ToggleGodMode();
                    Core.GetLogger().Msg("God Mode: " + isGodMode);
                }

                GUIStyle autoPlayButton;
                if (AutoPlay.enabled)
                {
                    autoPlayButton = compactEnabledButtonStyle;
                }
                else
                {
                    autoPlayButton = compactButtonStyle;
                }

                // Autoplay toggle
                if (GUI.Button(new Rect(110, 120, 100, 25), "Toggle Auto", autoPlayButton))
                {
                    bool isAutoPlay = AutoPlay.ToggleAutoPlay();
                    FileStorage.profile.SaveBeatmapOptions();
                    Core.GetLogger().Msg("Auto Play: " + isAutoPlay);
                }


                int totalHeight = CalculateTreeHeight(rootNode);

                scrollPosition = GUI.BeginScrollView(
                    new Rect(0, 250, 400, 370),
                    scrollPosition,
                    new Rect(0, 0, 380, totalHeight)
                );

                int yPos = 0;
                DrawTreeNode(rootNode, 0, ref yPos, folderStyle, leafStyle, stringStyle, playableStringStyle);

                GUI.EndScrollView();
            }
        }
        }
    
}
