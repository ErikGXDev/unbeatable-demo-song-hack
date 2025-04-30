using UnityEngine;
using HarmonyLib;
using Google.Protobuf.Collections;
using Newtonsoft.Json;

namespace UnbeatableSongHack.Translation
{
    public class ProgramLoader
    {

        public static string GetLocalTranslationDirectory()
        {
            // Path of the game exe
            string dataDir = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'));
            // Get the directory of the custom songs
            string translationDir = dataDir + "/Translation";
            return translationDir;
        }


        [HarmonyPatch(typeof(Yarn.Unity.YarnProject), "GetProgram")]
        public class YarnProjectPatch
        {
            public static bool Prefix(Yarn.Unity.YarnProject __instance, ref Yarn.Program __result)
            {
                if (disableCustomTranslation)
                {
                    return true;
                }

                // Get the directory of the custom songs
                string translationDir = GetLocalTranslationDirectory();
                // Get all files in the directory
                string[] files = Directory.GetFiles(translationDir, "*.yarnproject.json", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    if (file.Contains(__instance.name))
                    {

                        string programText = File.ReadAllText(file);


                        Yarn.Program program = Yarn.Program.Parser.ParseJson(programText);


                        __result = program;

                        return false;
                    }
                }
                return true;
            }
        }

        public static bool disableCustomTranslation = false;

        [HarmonyPatch(typeof(Yarn.Unity.Localization), "GetLocalizedString")]
        public class LocalizationPatch
        {
            public static bool Prefix(Yarn.Unity.Localization __instance, string key, ref string __result)
            {

                if (disableCustomTranslation)
                {
                    return true;
                }

                // Get the directory of the custom songs
                string translationDir = GetLocalTranslationDirectory();
                // Get all files in the directory
                string[] files = Directory.GetFiles(translationDir, "lines.json", SearchOption.AllDirectories);
                foreach (string file in files)
                {

                    string jsonText = File.ReadAllText(file);
                    Dictionary<string, string> lines = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonText);

                    if (lines.ContainsKey(key))
                    {

                        Core.GetLogger().Msg("Found translation for key: " + key);

                        __result = lines[key];
                        return false;
                    }

                }

                return true;

            }
        }
    }


}
