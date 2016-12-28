﻿using ModelGenerator.Models;
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

            // create a dictionary for claOptions and assign default values
            Dictionary<string, object> claOptions = new Dictionary<string, object>();
            claOptions.Add("-ii", false);
            claOptions.Add("-hl", false);
            claOptions.Add("-mf", "model");

            string[] boolSwitches = new string[] { "-ii", "-hl" };

            var expectValue = false;
            var lastSwitch = "";
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                if (!expectValue) { lastSwitch = arg; }

                if (arg.StartsWith("-"))
                {
                    if (expectValue)
                    {
                        // log error
                        Console.WriteLine("Error: {0} switch not specified correctly. Missing value", lastSwitch);
                        Console.ReadLine();
                        return;
                    }

                    var isBoolSwitch = boolSwitches.Any(s => s == arg);
                    if (isBoolSwitch)
                    {
                        claOptions[arg] = true;
                    }
                    else
                    {
                        expectValue = true;
                    }
                }
                else
                {
                    claOptions[lastSwitch] = arg;
                    expectValue = false;
                }
            }

            foreach (var item in claOptions)
            {
                LogCommandLineParameterToConsole(item.Key, item.Value);
            }

            var modelFolder = claOptions["-mf"].ToString();

            Console.WriteLine("Done.\n\n");

            Console.WriteLine("Reading entities\n");

            var entitiesRelativePath = System.IO.Path.Combine(modelFolder, "input", "entities");
            IList<Entity> entities = EntityReader.Read(entitiesRelativePath);

            Console.WriteLine("Done. \n");

            Console.WriteLine("Reading intents\n");

            var intentsRelativePath = System.IO.Path.Combine(modelFolder, "input", "intents");
            IList<Intent> intents = IntentReader.Read(intentsRelativePath);

            Console.WriteLine("Done. \n");

            Console.WriteLine("Writing intents\n");

            IntentProcessingOptions intentProcessingOptions = new IntentProcessingOptions();
            intentProcessingOptions.InlineIds = bool.Parse(claOptions["-ii"].ToString());
            intentProcessingOptions.IgnoreHeaderLine = bool.Parse(claOptions["-hl"].ToString());

            var intentOutputFolderRelativePath = System.IO.Path.Combine(modelFolder, "output", "intents");
            IntentLineSplitter intentLineSplitter = new IntentLineSplitter();
            EntityParser entityParser = new EntityParser();
            IntentWriter intentWriter = new IntentWriter(intentLineSplitter, entityParser);
            intentWriter.Write(intents, intentOutputFolderRelativePath, entities, intentProcessingOptions);

            Console.WriteLine("Finished processing intents\n\n");

            Console.WriteLine("Writing entities\n");

            var entitiesOutputFolderRelativePath = System.IO.Path.Combine(modelFolder, "output", "entities");
            EntityWriter.Write(entities, entitiesOutputFolderRelativePath);

            Console.WriteLine("Finished processing entities\n\n");

            Console.WriteLine("Models generation completed");

            Console.WriteLine("Finished. Press ENTER to exit");
            Console.ReadLine();
        }

        private static void LogCommandLineParameterToConsole(string switchName, object value)
        {
            Console.WriteLine("Switch {0} with value of {1}.", switchName, value.ToString());
        }
    }
}
