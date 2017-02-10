using ModelGeneratorW.Models;
using ModelGeneratorW.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ModelGeneratorW.Writers
{
    class IntentWriter
    {
        public static void Write(IList<IntentsOutputObject> intentsOutputObjects, string folderRelativePath)
        {
            var currentFolder = Directory.GetCurrentDirectory();

            var entitiesAbsolutePath = Path.Combine(currentFolder, folderRelativePath);

            Console.WriteLine("Writing intents to {0}", entitiesAbsolutePath);

            var directoryInfo = new DirectoryInfo(entitiesAbsolutePath);
            if (!directoryInfo.Exists)
            {
                Console.Write("Folder {0} doesn't exist. Creating ... ", entitiesAbsolutePath);
                directoryInfo.Create();
                Console.WriteLine("done");
            }
            else
            {
                foreach (var fileInfo in directoryInfo.EnumerateFiles())
                {
                    fileInfo.Delete();
                }
            }


            var sorted = intentsOutputObjects.ToList();
            sorted.Sort(new IntentOutputObjectComparer());

            for (int outputIndex = 0; outputIndex < sorted.Count; outputIndex++)
            {
                var intentsOutputObject = sorted[outputIndex];

                // Console.WriteLine("Processing intent {0} of {1}", outputIndex + 1, intentsOutputObjects.Count);

                var fileName = string.Format("{0}.json", intentsOutputObject.Name);
                var fileAbsolutePath = Path.Combine(entitiesAbsolutePath, fileName);
                var fileOutputStream = File.CreateText(fileAbsolutePath);

                Console.WriteLine("Writing file {0} ... ", fileName);

                string outputString = JsonConvert.SerializeObject(intentsOutputObject, new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented,
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                });
                outputString = outputString.Replace("\r", "");
                fileOutputStream.Write(outputString);

                fileOutputStream.Flush();
                fileOutputStream.Dispose();

                //Console.WriteLine("Done");
            }
        }
    }
}
