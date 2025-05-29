using UnityEngine;
using HarmonyLib;
using Google.Protobuf.Collections;
using Newtonsoft.Json;

namespace UnbeatableSongHack.Translation
{
    public class ProgramLoader
    {

        public static bool disableCustomTranslation = false;

        public static string GetLocalTranslationDirectory()
        {
            // Path of the game exe
            string dataDir = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'));
            // Get the directory of the custom translations
            string translationDir = dataDir + "/Translation";

            if (!Directory.Exists(translationDir))
            {
                // Create the directory if it doesn't exist
                Directory.CreateDirectory(translationDir);
            }

            return translationDir;
        }

        public static void LoadLocalTranslations()
        {

            ProgramIndex.programs.Clear();
            ProgramIndex.lines.Clear();

            // Get the directory of the custom translations
            string translationDir = GetLocalTranslationDirectory();

            // Get all files in the directory
            // *.yarnproject.json and lines.json
            string[] files = Directory.GetFiles(translationDir, "*.yarnproject.json", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                string programText = File.ReadAllText(file);

                Yarn.Program program = Yarn.Program.Parser.ParseJson(programText);

                string name = Path.GetFileNameWithoutExtension(file);

                if (name.LastIndexOf('.') != -1)
                {
                    name = name.Substring(0, name.LastIndexOf('.'));
                }


                Core.GetLogger().Msg("Loaded program: " + name);

                ProgramIndex.programs.Add(name, program);
            }

            files = Directory.GetFiles(translationDir, "lines.json", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                string jsonText = File.ReadAllText(file);

                Dictionary<string, string> lines = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonText);
                foreach (KeyValuePair<string, string> line in lines)
                {
                    ProgramIndex.lines.Add(line.Key, line.Value);
                }
            }

            Core.GetLogger().Msg("Loaded lines: " + ProgramIndex.lines.Count);

        }


       
    }


}
