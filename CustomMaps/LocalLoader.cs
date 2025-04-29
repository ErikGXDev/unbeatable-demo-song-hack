using Rhythm;
using static Arcade.UI.SongSelect.ArcadeSongDatabase;
using UnityEngine;
using Arcade.UI.SongSelect;

namespace UnbeatableSongHack.CustomMaps
{
    class LocalLoader
    {
        public static bool LoadBeatmapFromFile(string file, out BeatmapItem beatmapItem)
        {

            if (!File.Exists(file))
            {
                Core.GetLogger().Msg("Beatmap not found: " + file);
                beatmapItem = null;
                return false;
            }

            // Read the beatmap file
            string contents = File.ReadAllText(file);

            Beatmap beatmap = ScriptableObject.CreateInstance<Beatmap>();
            BeatmapParserEngine beatmapParserEngine = new BeatmapParserEngine();
            beatmapParserEngine.ReadBeatmap(contents, ref beatmap);


            Core.GetLogger().Msg("Parsed beatmap: " + beatmap.metadata.title);

            beatmapItem = new BeatmapItem();

            beatmapItem.Beatmap = beatmap;
            if (beatmapItem.Beatmap.metadata.tagData.Level.Equals(0)) { beatmapItem.Beatmap.metadata.tagData.Level = 99; }
            //beatmapItem.Beatmap.metadata.tagData.Level = 99;
            //beatmapItem.Beatmap.metadata.tagData.SongLength = 99;


            string difficulty = "Star";

            string[] defaultDifficulties = BeatmapIndex.defaultIndex.Difficulties;
            // Difficulties are not what they seem, welcome to devhell
            Dictionary<string, string> difficultyIndex = new Dictionary<string, string>
            {
                {"Beginner", "Beginner"},
                {"Normal", "Easy"},
                {"Hard", "Normal"},
                {"Expert", "Hard"},
                {"UNBEATABLE", "UNBEATABLE"},
                {"Unbeatable", "UNBEATABLE"}
            };
            string[] difficultyList = difficultyIndex.Keys.ToArray();

            // Check if the difficulty is in the default list
            // If not, set it to one that can be found in the game
            if (difficultyIndex.TryGetValue(beatmap.metadata.version, out string d))
            {
                difficulty = d;
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
                        Core.GetLogger().Msg("Audio file not found! Using EMPTY DIARY as fallback... :(");
                        audioPath = "EMPTY DIARY";
                    }

                }
            }

            var mapDataName = Encoder.EncodeSongName(basePath, audioPath);

            beatmapItem.Path = mapDataName + "/" + difficulty;

            beatmapItem.Song = new BeatmapIndex.Song(mapDataName);
            beatmapItem.Song.stageScene = "TrainStationRhythm";


            TextAsset textAsset = new TextAsset(contents);

            beatmapItem.BeatmapInfo = new BeatmapInfo(textAsset, difficulty);

            beatmapItem.Highscore = ArcadeSongDatabase.Instance.HighScores.GetScoreItem(mapDataName + "/" + difficulty);

            return true;
        }
    }
}
