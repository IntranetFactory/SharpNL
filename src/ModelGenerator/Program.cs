using ModelGenerator.Models;
using ModelGenerator.Readers;
using ModelGenerator.Tools;
using ModelGenerator.Writers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModelGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Model generator started \n\n");
            Console.WriteLine("Processing command line arguments ... \n");

            bool InlineIds = args.Any(arg => arg == "-ii");
            bool IgnoreHeaderLine = args.Any(arg => arg == "-hl");

            LogCommandLineParameterToConsole("-ii", "InlineIds", InlineIds);
            LogCommandLineParameterToConsole("-hl", "IgnoreHeaderLine", IgnoreHeaderLine);

            Console.WriteLine("Done.\n\n");

            Console.WriteLine("Reading entities\n");

            var entitiesRelativePath = System.IO.Path.Combine("model", "input", "entities");
            IList<Entity> entities = EntityReader.Read(entitiesRelativePath);

            Console.WriteLine("Done. \n");

            Console.WriteLine("Reading intents\n");

            var intentsRelativePath = System.IO.Path.Combine("model", "input", "intents");
            IList<Intent> intents = IntentReader.Read(intentsRelativePath);

            Console.WriteLine("Done. \n");

            IntentProcessingOptions intentProcessingOptions = new IntentProcessingOptions();
            intentProcessingOptions.InlineIds = InlineIds;
            intentProcessingOptions.IgnoreHeaderLine = IgnoreHeaderLine;

            var intentOutputFolderRelativePath = System.IO.Path.Combine("model", "output", "intents");
            IntentLineSplitter intentLineSplitter = new IntentLineSplitter();
            EntityParser entityParser = new EntityParser();
            IntentWriter intentWriter = new IntentWriter(intentLineSplitter, entityParser);
            intentWriter.Write(intents, intentOutputFolderRelativePath, entities, intentProcessingOptions);

            Console.WriteLine("Finished processing intents\n\n");


            var entitiesOutputFolderRelativePath = System.IO.Path.Combine("model", "output", "entities");
            EntityWriter.Write(entities, entitiesOutputFolderRelativePath);

            Console.WriteLine("Finished processing entities\n\n");

            Console.WriteLine("Models generation completed");

            Console.WriteLine("Finished. Press ENTER to exit");
            Console.ReadLine();
        }

        private static void LogCommandLineParameterToConsole(string switchName, string name, bool value)
        {
            if (!value)
            {
                Console.WriteLine("Switch {0} not present. Parameter {1} set to {2}.", switchName, name, value);
            }
            else
            {
                Console.WriteLine("Switch {0} present. Parameter {1} set to {2}.", switchName, name, value);
            }
        }
    }
}
