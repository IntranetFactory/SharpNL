using ModelGenerator.Models;
using ModelGenerator.Readers;
using ModelGenerator.Writers;
using System.Collections.Generic;

namespace ModelGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Generating models\n\n");

            System.Console.WriteLine("Processing intents\n");

            var intentsRelativePath = System.IO.Path.Combine("model", "input", "intents");
            IList<Intent> intents = IntentReader.Read(intentsRelativePath);

            System.Console.WriteLine("\n");

            var intentOutputFolderRelativePath = System.IO.Path.Combine("model", "output", "intents");
            IntentWriter.Write(intents, intentOutputFolderRelativePath);

            System.Console.WriteLine("Finished processing intents\n\n");

            System.Console.WriteLine("Processing entities\n");
            
            var entitiesRelativePath = System.IO.Path.Combine("model", "input", "entities");
            IList<Entity> entities = EntityReader.Read(entitiesRelativePath);

            System.Console.WriteLine("\n");

            var entitiesOutputFolderRelativePath = System.IO.Path.Combine("model", "output", "entities");
            EntityWriter.Write(entities, entitiesOutputFolderRelativePath);

            System.Console.WriteLine("Finished processing entities\n\n");

            System.Console.WriteLine("Models generation completed");

            System.Console.WriteLine("Press ENTER to exit");
            System.Console.ReadLine();
        }
    }
}
