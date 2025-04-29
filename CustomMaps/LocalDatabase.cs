using UnityEngine;
using static Arcade.UI.SongSelect.ArcadeSongDatabase;


namespace UnbeatableSongHack.CustomMaps
{


    public static class LocalDatabase
    {


        public static string GetLocalBeatmapDirectory()
        {
            // Path of the game exe
            string dataDir = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'));
            // Get the directory of the custom songs
            string songDir = dataDir + "/CustomSongs";
            return songDir;
        }

        public static string GetOsuBeatmapDirectory()
        {
            // Path of the game exe
            string test = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'));
            Core.GetLogger().Msg("slice 1: " + test);
            string dataDir = test.Substring(0, test.LastIndexOf('/'));
            Core.GetLogger().Msg("slice 2: " + dataDir);
            // Get the directory of the custom songs
            string songDir = dataDir + "/UNBEATABLE [white label]/CustomBeatmapsV3-Data/SERVER_PACKAGES";
            return songDir;
        }

        public static List<BeatmapItem> GetLocalBeatmapItems()
        {

            // Get the directory of the custom songs
            string songDir = GetLocalBeatmapDirectory();

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

        public static List<BeatmapItem> GetBeatmapItems(string songDir)
        {

            // Get the directory of the custom songs
            //string songDir = GetLocalBeatmapDirectory();

            Core.GetLogger().Msg("Getting osu beatmaps from: " + songDir);

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
