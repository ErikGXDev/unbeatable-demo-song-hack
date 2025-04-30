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


            YarnProject[] projects = (YarnProject[])Resources.FindObjectsOfTypeAll(typeof(YarnProject));

            

            Core.GetLogger().Msg(projects.Length + " projects found.");



            var lineRec = new Dictionary<string, string>();

            bool isCustomDisabled = ProgramLoader.disableCustomTranslation;
            if (!isCustomDisabled)
            {
                ProgramLoader.disableCustomTranslation = true;

            }

            foreach (YarnProject project in projects)
            {

                string program = project.GetProgram().ToString();


                string fileName = project.name + ".yarnproject.json";


                string filePath = outPath + fileName;



                File.WriteAllText(filePath, program);

                Yarn.Unity.Localization baseLoc = project.baseLocalization;

                

                foreach (string id in baseLoc.GetLineIDs())
                {
                    if (!lineRec.TryAdd(id, baseLoc.GetLocalizedString(id)))
                    {
                        Core.GetLogger().Msg("Duplicate line found: " + id + " in " + fileName);
                    }
                    
                }

               

            }

            if (!isCustomDisabled)
            {
                ProgramLoader.disableCustomTranslation = false;
            }

            var outLines = JsonConvert.SerializeObject(lineRec, Formatting.Indented);

            var outPathLines = outPath + "lines.json";


            File.WriteAllText(outPathLines, outLines);


            Core.GetLogger().Msg("Dumped translations to: " + outPath);
        }

    }
}
