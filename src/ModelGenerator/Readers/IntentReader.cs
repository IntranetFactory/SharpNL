using ModelGenerator.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ModelGenerator.Readers
{
    static class IntentReader
    {
        public static IList<Intent> Read(string folderRelativePath)
        {
            var currentFolder = Directory.GetCurrentDirectory();

            var intentsAbsolutePath = Path.Combine(currentFolder, folderRelativePath);

            Console.WriteLine(string.Format("Reading intent definition files from {0}", intentsAbsolutePath));

            DirectoryInfo directoryInfo = new DirectoryInfo(intentsAbsolutePath);
            var fileInfos = directoryInfo.EnumerateFiles();
            var fileCount = fileInfos.Count();

            List<Intent> result = new List<Intent>(fileCount);

            if (fileCount > 0)
            {
                System.Console.WriteLine("Found {0} files that contain intent definitions", fileCount);
            }
            else
            {
                System.Console.WriteLine("Folder {0} is empty", intentsAbsolutePath);
            }

            foreach (var fileInfo in fileInfos)
            {
                System.Console.Write("Processing file {0}", fileInfo.Name);
                
                var fileInputStream = fileInfo.OpenRead();
                byte[] fileBytes = new byte[fileInfo.Length];

                // WARNING! Casting file length to int limits the file size to 4GB
                fileInputStream.Read(fileBytes, 0, (int)fileInfo.Length);
                fileInputStream.Dispose();

                var contents = Encoding.UTF8.GetString(fileBytes);
                contents = contents.Replace("\"", "");

                Intent entity = new Intent();
                entity.Id = Guid.NewGuid().ToString();
                entity.IntentName = Path.GetFileNameWithoutExtension(fileInfo.Name);
                entity.Text = contents;

                result.Add(entity);

                System.Console.WriteLine("... done");
            }

            return result;
        }
    }
}
