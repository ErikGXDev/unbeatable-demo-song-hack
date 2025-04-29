using System;
using System.Collections.Generic;
using System.Text;
using Yarn.Unity;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

namespace UnbeatableSongHack.Translation
{
    public class Dump
    {

        public static void DumpTranslations()
        {

            Core.GetLogger().Msg("Dumping translations...");

            string outPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/')) + "/dumped/";

            // Check if the directory exists
            if (!Directory.Exists(outPath))
            {
                // Create the directory
                Directory.CreateDirectory(outPath);
            }

            Core.GetLogger().Msg("Dumping translations...");


            YarnProject[] projects = UnityEngine.Object.FindObjectsByType<YarnProject>(FindObjectsSortMode.InstanceID);

            Core.GetLogger().Msg(projects.Length + " projects found.");

            Core.GetLogger().Msg("Dumping translations...");


            foreach (YarnProject project in projects)
            {
                string program = project.GetProgram().ToString();

                Core.GetLogger().Msg("1Dumping translations...");

                string fileName = project.name + ".yarnproject.json";

                Core.GetLogger().Msg("1Dumping translations...");

                string filePath = outPath + fileName;

                Core.GetLogger().Msg("1Dumping translations...");


                File.WriteAllText(filePath, program);

                Core.GetLogger().Msg("1Dumping translations...");


            }

            Core.GetLogger().Msg("Dumping translations...");


            var baseLoc = projects[0].baseLocalization;

            Core.GetLogger().Msg("Dumping translations...");


            var lineRec = new Dictionary<string, string>();

            Core.GetLogger().Msg("Dumping translations...");


            foreach (string id in baseLoc.GetLineIDs())
            {
                lineRec.Add(id, baseLoc.GetLocalizedString(id));
            }

            Core.GetLogger().Msg("Dumping translations...");


            var outLines = JsonConvert.SerializeObject(lineRec, Formatting.Indented);

            Core.GetLogger().Msg("Dumping translations...");


            var outPathLines = outPath + "lines.json";

            Core.GetLogger().Msg("Dumping translations...");


            File.WriteAllText(outPathLines, outLines);


            Core.GetLogger().Msg("Dumped translations to: " + outPath);
        }

    }
}
