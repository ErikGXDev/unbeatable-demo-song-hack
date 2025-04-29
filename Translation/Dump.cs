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



            foreach (YarnProject project in projects)
            {
                string program = project.GetProgram().ToString();


                string fileName = project.name + ".yarnproject.json";


                string filePath = outPath + fileName;



                File.WriteAllText(filePath, program);


            }



            var baseLoc = projects[0].baseLocalization;



            var lineRec = new Dictionary<string, string>();



            foreach (string id in baseLoc.GetLineIDs())
            {
                lineRec.Add(id, baseLoc.GetLocalizedString(id));
            }



            var outLines = JsonConvert.SerializeObject(lineRec, Formatting.Indented);



            var outPathLines = outPath + "lines.json";


            File.WriteAllText(outPathLines, outLines);


            Core.GetLogger().Msg("Dumped translations to: " + outPath);
        }

    }
}
