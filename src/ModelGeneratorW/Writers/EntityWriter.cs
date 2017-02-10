using ModelGeneratorW.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;

namespace ModelGeneratorW.Writers
{
    static class EntityWriter
    {
        public static void Write(IList<Entity> entities, string folderRelativePath)
        {
            var currentFolder = Directory.GetCurrentDirectory();

            var entitiesAbsolutePath = Path.Combine(currentFolder, folderRelativePath);

            Console.WriteLine("Writing entities to {0}", entitiesAbsolutePath);

            var directoryInfo = new DirectoryInfo(entitiesAbsolutePath);
            if (!directoryInfo.Exists) {
                Console.Write("Folder {0} doesn't exist. Creating ... ", entitiesAbsolutePath);
                directoryInfo.Create();
                Console.WriteLine("done");
            }

            int count = 1;
            int total = entities.Count;
            //foreach (var entity in entities)
            //{
            //    Console.WriteLine("Processing entity {0} of {1}", count, total);

            //    var fileName = string.Format("{0}.json", entity.EntityName);
            //    var fileAbsolutePath = Path.Combine(entitiesAbsolutePath, fileName);
            //    var fileOutputStream = File.CreateText(fileAbsolutePath);

            //    Console.Write("Writing file {0} ... ", fileName);

            //    EntitiesOutputObject entitiesOutput = new EntitiesOutputObject();
            //    entitiesOutput.Id = entity.Id;
            //    entitiesOutput.Name = entity.EntityName;
            //    entitiesOutput.Entries = new List<EntityOutputObject>(entity.Synonyms.Length);

            //    foreach (var synonym in entity.Synonyms)
            //    {
            //        var synonymParts = synonym.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            //        if (synonymParts.Length > 0)
            //        {
            //            EntityOutputObject outputObject = new EntityOutputObject();
            //            outputObject.Value = synonymParts[0];
            //            if (synonymParts.Length > 1)
            //            {
            //                outputObject.Synonyms = new string[synonymParts.Length - 1];
            //                for (int j = 0; j < outputObject.Synonyms.Length; j++)
            //                {
            //                    outputObject.Synonyms[j] = synonymParts[j + 1];
            //                }
            //            }
            //            entitiesOutput.Entries.Add(outputObject);
            //        }
            //    }

            //    string outputString = JsonConvert.SerializeObject(entitiesOutput, Formatting.Indented, new JsonSerializerSettings() {
            //        ContractResolver = new CamelCasePropertyNamesContractResolver(),
            //        NullValueHandling = NullValueHandling.Ignore
            //    });
            //    outputString = outputString.Replace("\r", "");
            //    fileOutputStream.Write(outputString);

            //    fileOutputStream.Flush();
            //    fileOutputStream.Dispose();
            //    count += 1;

            //    Console.WriteLine("done", entitiesAbsolutePath);
            //}
        }
    }
}
