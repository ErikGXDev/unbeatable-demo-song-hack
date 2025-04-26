using Newtonsoft.Json;
using Rhythm;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Arcade.UI.SongSelect.ArcadeSongDatabase;
using static Dreamteck.Splines.SplineSampleModifier;


namespace UnbeatableSongHack.CustomMaps
{

    public class PackageBeatmap
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("artist")]
        public string Artist { get; set; }

        [JsonProperty("creator")]
        public string Creator { get; set; }

        [JsonProperty("difficulty")]
        public string Difficulty { get; set; }

        [JsonProperty("audioFileName")]
        public string AudioFileName { get; set; }
    }

    public class Package
    {
        [JsonProperty("filePath")]
        public string FilePath { get; set; }

        [JsonProperty("time")]
        public string Time { get; set; }

        [JsonProperty("beatmaps")]
        public Dictionary<string, PackageBeatmap> Beatmaps { get; set; }

        public Package() { }

        public Package(string filePath, string time)
        {
            FilePath = filePath;
            Time = time;
            Beatmaps = new Dictionary<string, PackageBeatmap>();
        }

        public override string ToString()
        {
            var serialized = JsonConvert.SerializeObject(this, Formatting.Indented);
            return serialized;
        }
    }

    public class Packages
    {
        [JsonProperty("packages")]
        public Package[] PackageList { get; set; }
        public Packages() { }
        public Packages(Package[] packageList)
        {
            PackageList = packageList;
        }
    }

    public class PackagesRoot
    {
        [JsonProperty("packages")]
        public List<Package> PackageList { get; set; }
        public PackagesRoot() { }
        public PackagesRoot(List<Package> packageList)
        {
            PackageList = packageList;
        }
    }

    public static class DatabaseLoader
    {
        public static List<Package> GetPackages()
        {
            string packageURL = DatabaseConfig.GetServerPackageListURL();

            Core.GetLogger().Msg("Getting packages from: " + packageURL);

            using (var webClient = new System.Net.WebClient())
            {
                string json = webClient.DownloadString(packageURL);

                // Deserialize the JSON into an array of Package objects
                var packages = JsonConvert.DeserializeObject<PackagesRoot>(json);
                return packages.PackageList;
            }

        }

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

                // Read the beatmap file
                string contents = File.ReadAllText(file);

                Beatmap beatmap = ScriptableObject.CreateInstance<Beatmap>();
                BeatmapParserEngine beatmapParserEngine = new BeatmapParserEngine();
                beatmapParserEngine.ReadBeatmap(contents, ref beatmap);


                Core.GetLogger().Msg("Parsed beatmap: " + beatmap.metadata.title);

                BeatmapItem beatmapItem = new BeatmapItem();

                beatmapItem.Beatmap = beatmap;
                beatmapItem.Beatmap.metadata.tagData.Level = 99;
                beatmapItem.Beatmap.metadata.tagData.SongLength = 99;


                string difficulty = beatmap.metadata.version;

                string[] defaultDifficulties = BeatmapIndex.defaultIndex.Difficulties;

                // Check if the difficulty is in the default list
                // If not, set it to one that can be found in the game
                if (!defaultDifficulties.Contains(difficulty))
                {
                    difficulty = "UNBEATABLE";
                }

                // Find audio file
                var basePath = Path.GetDirectoryName(file);
                var audioFilename = beatmap.general.audioFilename;
                var audioPath = Path.Combine(basePath, audioFilename);

                // Check for audio file specified in metadata
                if (!File.Exists(audioPath))
                {
                    // Check for audio file in the same directory as the beatmap
                    audioFilename = Path.GetFileName(audioPath);
                    audioPath = Path.Combine(basePath, audioFilename);
                    if (!File.Exists(audioPath))
                    {

                        // Check for potential audio.mp3 file
                        audioPath = Path.Combine(basePath, "audio.mp3");
                        if (!File.Exists(audioPath))
                        {
                            Core.GetLogger().Msg("Audio file not found! :(");
                            audioPath = "EMPTY DIARY";
                        }

                    }
                }



                var mapDataName = Encoder.EncodeSongName(file, audioPath);

                beatmapItem.Path = mapDataName + "/" + difficulty;

                beatmapItem.Song = new BeatmapIndex.Song(mapDataName);
                beatmapItem.Song.stageScene = "TrainStationRhythm";


                TextAsset textAsset = new TextAsset(contents);

                beatmapItem.BeatmapInfo = new BeatmapInfo(textAsset, difficulty);

                beatmapItem.Highscore = new HighScoreItem(beatmapItem.Path, 0, 0f, 0, cleared: false, new Dictionary<string, int>(), Modifiers.None);

                beatmapItems.Add(beatmapItem);


            }

            return beatmapItems;

        }
    }
}
