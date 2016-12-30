using ModelGenerator.Models;
using ModelGenerator.Parsers;
using ModelGenerator.Readers;
using ModelGenerator.Tools;
using ModelGenerator.Writers;
using System;
using System.Collections.Generic;
using System.IO;

namespace ModelGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Model generator started \n\n");
            Console.WriteLine("Processing command line arguments ... \n");

            // create a dictionary for claOptions and assign default values
            CommandLineArgumentsParser claParser = new CommandLineArgumentsParser();
            var claParserResult = claParser.Parse(args);

            if (claParserResult.HasError)
            {
                Console.WriteLine("Error processing command line arguments: {0}", claParserResult.ErrorMessage);
                Console.ReadLine();
                return;
            }

            var settings = claParserResult.ApplicationSettings;
            LogCommandLineParameterToConsole("-ii", settings.InlineIds);
            LogCommandLineParameterToConsole("-hl", settings.IgnoreHeaderLine);
            LogCommandLineParameterToConsole("-mf", settings.ModelFolder);

            var modelFolder = settings.ModelFolder;

            // Console.WriteLine("Done.\n\n");

            try
            {
                Console.WriteLine("Reading entities\n");

                var entitiesRelativePath = System.IO.Path.Combine(modelFolder, "input", "entities");
                IList<Entity> entities = EntityReader.Read(entitiesRelativePath);

                // Console.WriteLine("Done. \n");

                Console.WriteLine("Reading intents\n");

                var intentsRelativePath = System.IO.Path.Combine(modelFolder, "input", "intents");
                IList<IntentFileInfo> intents = IntentReader.Read(intentsRelativePath);

                // Console.WriteLine("Done. \n");

                Console.WriteLine("Writing intents\n");

                var intentOutputFolderRelativePath = System.IO.Path.Combine(modelFolder, "output", "intents");

                IList<IntentsOutputObject> intentsOutputObjects = new List<IntentsOutputObject>();
                EntityParser entityParser = new EntityParser();
                IntentDefaultLineParser lineParser = new IntentDefaultLineParser(entityParser);

                if (settings.InlineIds)
                {
                    // do the inline id processing and respect ignore header line
                    IntentLineSplitter intentLineSplitter = new IntentLineSplitter();

                    InlineIdIntentParser parser = new InlineIdIntentParser(intentLineSplitter, lineParser);
                    intentsOutputObjects = parser.Parse(intents, entities, settings.IgnoreHeaderLine);
                }
                else
                {
                    // do the simple processing
                    SimpleIntentParser simpleParser = new SimpleIntentParser(lineParser);
                    intentsOutputObjects = simpleParser.Parse(intents, entities);
                }

                IntentWriter.Write(intentsOutputObjects, intentOutputFolderRelativePath);

                Console.WriteLine("Finished processing intents\n\n");

                Console.WriteLine("Writing entities\n");

                var entitiesOutputFolderRelativePath = System.IO.Path.Combine(modelFolder, "output", "entities");
                EntityWriter.Write(entities, entitiesOutputFolderRelativePath);

                Console.WriteLine("Finished processing entities\n\n");

                Console.WriteLine("Models generation completed");
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Finished. Press ENTER to exit");
            Console.ReadLine();
        }

        private static void LogCommandLineParameterToConsole(string switchName, object value)
        {
            Console.WriteLine("Switch {0} with value of {1}.", switchName, value.ToString());
        }
    }
}
