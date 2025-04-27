using Rhythm;
using UnityEngine;
using static Arcade.UI.SongSelect.ArcadeSongDatabase;


namespace UnbeatableSongHack.CustomMaps
{


    public static class LocalDatabase
    {


        public static string getLocalBeatmapDirectory()
        {
            // Path of the game exe
            string dataDir = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'));
            // Get the directory of the custom songs
            string songDir = dataDir + "/CustomSongs";
            return songDir;
        }

        public static List<BeatmapItem> GetLocalBeatmapItems()
        {

            // Get the directory of the custom songs
            string songDir = getLocalBeatmapDirectory();

            Core.GetLogger().Msg("Getting local beatmaps from: " + songDir);

            List<BeatmapItem> beatmapItems = new List<BeatmapItem>();


            // Get all files in the directory
            string[] files = Directory.GetFiles(songDir, "*.osu", SearchOption.AllDirectories);
            foreach (string file in files)
            {


                if (!LocalLoader.LoadBeatmapFromFile(file, out BeatmapItem beatmapItem))
                {
                    continue;
                }

                beatmapItems.Add(beatmapItem);


            }

            return beatmapItems;

        }
    }
}
