using ModelGeneratorW.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ModelGeneratorW.Readers
{
    static class EntityReader
    {
        public static IList<Entity> Read(string folderRelativePath)
        {
            var currentFolder = Directory.GetCurrentDirectory();

            var entitiesAbsolutePath = Path.Combine(currentFolder, folderRelativePath);

            Console.WriteLine(string.Format("Reading entity definition files from {0}", entitiesAbsolutePath));

            DirectoryInfo directoryInfo = new DirectoryInfo(entitiesAbsolutePath);
            var fileInfos = directoryInfo.EnumerateFiles();
            var fileCount = fileInfos.Count();

            List<Entity> result = new List<Entity>(fileCount);

            if (fileCount > 0)
            {
                Console.WriteLine("Found {0} files that contain entity definitions", fileCount);
            }
            else
            {
                Console.WriteLine("Folder {0} is empty", entitiesAbsolutePath);
            }

            foreach (var fileInfo in fileInfos)
            {
                Console.Write("Processing file {0}", fileInfo.Name);

                var fileInputStream = fileInfo.OpenRead();
                List<string> synonyms = new List<string>();

                StreamReader streamReader = new StreamReader(fileInputStream);

                while (!streamReader.EndOfStream)
                {
                    var synonym = streamReader.ReadLine();
                    synonym = synonym.Replace("\"", "");
                    synonyms.Add(synonym);
                }

                streamReader.Dispose();
                fileInputStream.Dispose();

                Entity entity = new Entity();
                entity.Id = Guid.NewGuid().ToString();
                entity.EntityName = Path.GetFileNameWithoutExtension(fileInfo.Name);
                entity.Synonyms = synonyms.ToArray();

                result.Add(entity);

                Console.WriteLine("... done.");
            }

            return result;
        }
    }
}
