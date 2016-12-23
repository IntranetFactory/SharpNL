using ModelGenerator.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ModelGenerator.Readers
{
    static class EntityReader
    {
        public static IList<Entity> Read(string folderRelativePath)
        {
            var currentFolder = Directory.GetCurrentDirectory();

            var entitiesAbsolutePath = Path.Combine(currentFolder, folderRelativePath);

            DirectoryInfo directoryInfo = new DirectoryInfo(entitiesAbsolutePath);
            var fileInfos = directoryInfo.EnumerateFiles();
            var fileCount = fileInfos.Count();

            List<Entity> result = new List<Entity>(fileCount);

            foreach (var fileInfo in fileInfos)
            {
                var fileInputStream = fileInfo.OpenRead();
                byte[] fileBytes = new byte[fileInfo.Length];

                // WARNING! Casting file length to int limits the file size to 4GB
                fileInputStream.Read(fileBytes, 0, (int)fileInfo.Length);
                fileInputStream.Dispose();

                var contents = Encoding.UTF8.GetString(fileBytes);
                contents = contents.Replace("\"", "");

                Entity entity = new Entity();
                entity.Id = Guid.NewGuid().ToString();
                entity.EntityName = Path.GetFileNameWithoutExtension(fileInfo.Name);
                entity.Synonyms = contents.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                result.Add(entity);
            }

            return result;
        }
    }
}
