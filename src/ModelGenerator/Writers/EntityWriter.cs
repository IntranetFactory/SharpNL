using ModelGenerator.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;

namespace ModelGenerator.Writers
{
    static class EntityWriter
    {
        public static void Write(IList<Entity> entities, string folderRelativePath)
        {
            var currentFolder = Directory.GetCurrentDirectory();

            var entitiesAbsolutePath = Path.Combine(currentFolder, folderRelativePath);

            System.Console.WriteLine("Writing entities to {0}", entitiesAbsolutePath);

            var directoryInfo = new DirectoryInfo(entitiesAbsolutePath);
            if (!directoryInfo.Exists) {
                System.Console.Write("Folder {0} doesn't exist. Creating ... ", entitiesAbsolutePath);
                directoryInfo.Create();
                System.Console.WriteLine("done");
            }

            int count = 1;
            int total = entities.Count;
            foreach (var entity in entities)
            {
                System.Console.WriteLine("Processing entity {0} of {1}", count, total);

                var fileName = string.Format("{0}.json", entity.EntityName);
                var fileAbsolutePath = Path.Combine(entitiesAbsolutePath, fileName);
                var fileOutputStream = File.CreateText(fileAbsolutePath);

                System.Console.Write("Writing file {0} ... ", fileName);

                EntitiesOutputObject entitiesOutput = new EntitiesOutputObject();
                entitiesOutput.Id = entity.Id;
                entitiesOutput.Name = entity.EntityName;
                entitiesOutput.Entries = new List<EntityOutputObject>(entity.Synonyms.Length);

                foreach (var synonym in entity.Synonyms)
                {
                    var synonymParts = synonym.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    if (synonymParts.Length > 0)
                    {
                        EntityOutputObject outputObject = new EntityOutputObject();
                        outputObject.Value = synonymParts[0];
                        if (synonymParts.Length > 1)
                        {
                            outputObject.Synonyms = new string[synonymParts.Length - 1];
                            for (int j = 0; j < outputObject.Synonyms.Length; j++)
                            {
                                outputObject.Synonyms[j] = synonymParts[j + 1];
                            }
                        }
                        entitiesOutput.Entries.Add(outputObject);
                    }
                }

                string outputString = JsonConvert.SerializeObject(entitiesOutput, Formatting.Indented, new JsonSerializerSettings() {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                });
                fileOutputStream.Write(outputString);

                fileOutputStream.Flush();
                fileOutputStream.Dispose();
                count += 1;

                System.Console.WriteLine("done", entitiesAbsolutePath);
            }
        }
    }
}
