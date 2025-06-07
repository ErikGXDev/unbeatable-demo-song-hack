using System;
using System.Collections.Generic;
using System.Text;
using Arcade.UI.SongSelect;
using UnityEngine;


namespace UnbeatableSongHack.CustomMaps
{
    public class Dump
    {

        public static string GetBeatmapDumpPath()
        {
            return Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/')) + "/beatmaps_dumped/";
        }

        public static void DumpArcadeBeatmaps()
        {

            string outPath = GetBeatmapDumpPath();

            ArcadeSongDatabase arcadeDatabase = ArcadeSongDatabase.Instance;

            foreach (var (title, beatmap) in arcadeDatabase.SongDatabase)
            {
                string newTitle = title.Replace("/", " - ");

                string fileName = newTitle + ".osu";

                string songName = beatmap.Song.name;

                string songFolder = outPath + songName + "/";

                // Check if the directory exists
                if (!Directory.Exists(songFolder))
                {
                    // Create the directory
                    Directory.CreateDirectory(songFolder);
                }

                string filePath = songFolder + fileName;

                // Write the beatmap to the file
                try
                {
                    System.IO.File.WriteAllText(filePath, beatmap.BeatmapInfo.textAsset.text);
                    Core.GetLogger().Msg("Dumped beatmap: " + fileName);
                }
                catch (Exception e)
                {
                    Core.GetLogger().Msg("Failed to dump beatmap: " + fileName + " to " + songFolder + "\n" + e.Message);
                }

            }
        }
    }
}
