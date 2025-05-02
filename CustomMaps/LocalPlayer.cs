using Arcade.UI.SongSelect;
using Arcade.UI;
using Rhythm;
using static Arcade.UI.SongSelect.ArcadeSongDatabase;
using static Rhythm.BeatmapIndex;
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

            public BeatmapItem beatmapItem;

            public CustomProgression(BeatmapItem beatmapItem)
            {
                this.beatmapItem = beatmapItem;

                this.stageScene = "TrainStationRhythm";
            }

            public string GetBeatmapPath()
            {
                // Placeholder
                return beatmapItem.Path;
            }
            public string GetSongName()
            {
                return beatmapItem.Song.name;
            }

            public string GetDifficulty()
            {
                // Placeholder
                return "UNBEATABLE";
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


            if (!LocalLoader.LoadBeatmapFromFile(filePath, out BeatmapItem beatmapItem))
            {
                Core.GetLogger().Msg("Beatmap not found: " + filePath);
                return;
            }

            CustomProgression customProgression = new CustomProgression(beatmapItem);

            JeffBezosController.rhythmProgression = customProgression;

            LevelManager.LoadLevel(customProgression.stageScene);

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


                beatmapItem.Beatmap = new Beatmap();

                // This was a test and is not needed

                // This whole part would be needed for AddSongList()

                //beatmapItem.Beatmap.metadata.title = "Custom " + name;
                //beatmapItem.Beatmap.metadata.titleUnicode = "Custom " + name;
                //beatmapItem.Beatmap.metadata.artist = "Not You";
                //beatmapItem.Beatmap.metadata.artistUnicode = "Not You";
                //beatmapItem.Beatmap.metadata.tagData.Level = 10;
                //AddSongToArcadeList(songList,beatmapItem);

                Core.GetLogger().Msg("Playing Key: " + key);


                arcadeBGMManger.PlaySongPreview(beatmapItem);


            }

        }


    }
}
