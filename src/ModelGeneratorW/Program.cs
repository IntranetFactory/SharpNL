using ModelGeneratorW.Models;
using ModelGeneratorW.Parsers;
using ModelGeneratorW.Readers;
using ModelGeneratorW.Tools;
using ModelGeneratorW.Writers;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Reflection;
using System.IO.Compression;

namespace ModelGeneratorW
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

            IList<Entity> entities = new List<Entity>();
            IList<IntentFileInfo> intents = new List<IntentFileInfo>();

            Console.WriteLine("Reading entities\n");
            var entitiesRelativePath = Path.Combine(modelFolder, "input", "entities");
            try
            {
                entities = EntityReader.Read(entitiesRelativePath);
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine("Folder {0} not found. Skipping ...", entitiesRelativePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            // Console.WriteLine("Done. \n");
            Console.WriteLine("Reading intents\n");
            var intentsRelativePath = Path.Combine(modelFolder, "input", "intents");
            try
            {
                intents = IntentReader.Read(intentsRelativePath);
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine("Folder {0} not found. Skipping ...", intentsRelativePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            try
            {
                Console.WriteLine("Writing intents\n");

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

                Console.WriteLine("Finished processing intents\n\n");
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            // Console.WriteLine("Done. \n");

            // create name of the folder that will contain everything to be compressed
            var folderToBeCompressed = string.Format("{0}W", modelFolder);

            // create relative paht for the entities folder under folder to be compressed
            var entitiesFolderToBeCompressed = Path.Combine(folderToBeCompressed, "entities");

            // prepare the AppOutputObject
            var appOutputObject = new AppOutputObject();
            appOutputObject.Version = 20160513;
            appOutputObject.ZipCommand = string.Join(" ",
                "zip",
                string.Format("{0}.zip", folderToBeCompressed),
                string.Format("{0}/app.json", folderToBeCompressed),
                string.Format("{0}/entities/*.json", folderToBeCompressed),
                string.Format("{0}/actions.json", folderToBeCompressed),
                string.Format("{0}/stories.json", folderToBeCompressed),
                string.Format("{0}/expressions.json", folderToBeCompressed));

            var appDataOutputObject = new AppDataOutputObject();
            appDataOutputObject.Name = modelFolder;
            appDataOutputObject.Description = string.Empty;
            appDataOutputObject.Lang = "en";

            appOutputObject.Data = appDataOutputObject;

            // prepare actions output object
            var actionsOutputObject = new ActionsOutputObject();
            actionsOutputObject.Data = new List<string>();

            // prepare stories output object 
            var storiesOutputObject = new StoriesOutputObject();
            storiesOutputObject.Data = new List<string>();

            // prepare expressions output object
            var expressionsOutputObject = new ExpressionsOutputObject();

            for (int i = 0; i < intents.Count; i++)
            {
                var intent = intents[i];
                for (int j = 0; j < intent.Lines.Count; j++)
                {
                    var intentLine = intent.Lines[j];
                    intentLine = intentLine.Remove(0, 1);
                    intentLine = intentLine.Remove(intentLine.Length - 1, 1);

                    var containedEntityNames = new List<string>();
                    var expressionOutputObject = new ExpressionOutputObject();

                    var containedEntities = new List<ExpressionEntityEntryOutputObject>();
                    containedEntities.Add(new ExpressionEntityEntryOutputObject("intent", string.Format("\"{0}\"", intent.IntentName)));

                    for (int k = 0; k < entities.Count; k++)
                    {
                        var entity = entities[k];
                        if (intentLine.Contains(entity.EntityName))
                        {
                            containedEntityNames.Add(entity.EntityName);
                        }
                    }

                    for (int k = 0; k < containedEntityNames.Count; k++)
                    {
                        // if intent line contains an entity 
                        var entityName = containedEntityNames[k];

                        // find value 
                        var entityOpenTag = string.Format("<{0}>", entityName);
                        var entityCloseTag = string.Format("</{0}>", entityName);
                        var openTagStartIndex = intentLine.IndexOf(entityOpenTag, StringComparison.InvariantCultureIgnoreCase);
                        while (openTagStartIndex > 0)
                        {
                            var containedEntity = new ExpressionEntityEntryOutputObject(entityName);
                            var openTagEndIndex = openTagStartIndex + entityOpenTag.Length;
                            var endTagStartIndex = intentLine.IndexOf(entityCloseTag, StringComparison.InvariantCultureIgnoreCase);
                            containedEntity.Value = intentLine.Substring(openTagEndIndex, endTagStartIndex - openTagEndIndex);

                            // replace entity open tag and entity end tag with empty strings
                            intentLine = intentLine.Remove(endTagStartIndex, entityCloseTag.Length);
                            intentLine = intentLine.Remove(openTagStartIndex, entityOpenTag.Length);

                            var valueStartIndex = intentLine.IndexOf(containedEntity.Value);

                            containedEntity.Start = valueStartIndex;
                            containedEntity.End = valueStartIndex + containedEntity.Value.Length;
                            containedEntities.Add(containedEntity);
                            containedEntity.Value = string.Format("\"{0}\"", containedEntity.Value);

                            openTagStartIndex = intentLine.IndexOf(entityOpenTag, StringComparison.InvariantCultureIgnoreCase);
                        }
                    }

                    expressionOutputObject.Text = intentLine;
                    // else if it doesn't contain an entity intent entity is already added
                    expressionOutputObject.Entities.AddRange(containedEntities);
                    expressionsOutputObject.Data.Add(expressionOutputObject);
                }
            }

            // prepare entities objects
            var entitiesOutputObjects = new List<EntitiesOutputObject>();

            for (int i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                var entitiesOutputObject = new EntitiesOutputObject(entity.EntityName);

                for (int j = 0; j < entity.Synonyms.Length; j++)
                {
                    var synonym = entity.Synonyms[j];
                    var synonymParts = synonym.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    if (synonymParts.Length > 0)
                    {
                        var synonymKey = synonymParts[0];
                        var entityOutputObject = new EntityOutputObject();
                        var alreadyExisting = entitiesOutputObject.Data.Values.FirstOrDefault(obj => obj.Value == synonymKey);
                        if (alreadyExisting != null)
                        {
                            entityOutputObject = alreadyExisting;
                        }
                        else
                        {
                            entitiesOutputObject.Data.Values.Add(entityOutputObject);
                        }

                        entityOutputObject.Value = synonymKey;
                        for (int k = 1; k < synonymParts.Length; k++)
                        {
                            var expression = synonymParts[k];
                            if (!entityOutputObject.Expressions.Contains(expression))
                            {
                                entityOutputObject.Expressions.Add(expression);
                            }
                        }

                    }
                }

                entitiesOutputObjects.Add(entitiesOutputObject);
            }

            // prepare intent output object
            var intentsOutputObject = new IntentsOutputObjectW();

            for (int i = 0; i < intents.Count; i++)
            {
                var intent = intents[i];
                var intentOutputObject = new IntentOutputObjectW(intent.IntentName);
                intentsOutputObject.Data.Values.Add(intentOutputObject);
            }

            // write to disk
            try
            {
                // json serialization options
                var jsonSerializationOptions = new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented,
                    ContractResolver = new CustomContractResolver(),
                    DefaultValueHandling = DefaultValueHandling.Ignore
                };

                // create output folder
                DirectoryInfo outputFolder = new DirectoryInfo(folderToBeCompressed);
                if (!outputFolder.Exists) { outputFolder.Create(); }

                // write actions.json
                FileInfo actionsJsonFileInfo = new FileInfo(Path.Combine(folderToBeCompressed, "actions.json"));
                var actionsOutputObjectJsonString = JsonConvert.SerializeObject(actionsOutputObject, jsonSerializationOptions);
                WriteFileToDisk(actionsOutputObjectJsonString, actionsJsonFileInfo);

                // write app.json
                FileInfo appJsonFileInfo = new FileInfo(Path.Combine(folderToBeCompressed, "app.json"));
                var appOutputObjectJsonString = JsonConvert.SerializeObject(appOutputObject, jsonSerializationOptions);
                WriteFileToDisk(appOutputObjectJsonString, appJsonFileInfo);

                // write expressions.json
                FileInfo expressionsJsonFileInfo = new FileInfo(Path.Combine(folderToBeCompressed, "expressions.json"));
                var expressionsOutputObjectJsonString = JsonConvert.SerializeObject(expressionsOutputObject, jsonSerializationOptions);
                WriteFileToDisk(expressionsOutputObjectJsonString, expressionsJsonFileInfo);

                // write stories.json 
                FileInfo storiesJsonFileInfo = new FileInfo(Path.Combine(folderToBeCompressed, "stories.json"));
                var storiesOutputObjectJsonString = JsonConvert.SerializeObject(storiesOutputObject, jsonSerializationOptions);
                WriteFileToDisk(storiesOutputObjectJsonString, storiesJsonFileInfo);

                // write entities folder
                DirectoryInfo entitiesOutputFolder = new DirectoryInfo(entitiesFolderToBeCompressed);
                if (!entitiesOutputFolder.Exists) { entitiesOutputFolder.Create(); }

                // write entities
                for (int i = 0; i < entitiesOutputObjects.Count; i++)
                {
                    var entitiesOutputObject = entitiesOutputObjects[i];
                    FileInfo entityJsonFileInfo = new FileInfo(Path.Combine(entitiesFolderToBeCompressed, string.Format("{0}.json", entitiesOutputObject.Data.Name)));
                    var entityOutputObjectJsonString = JsonConvert.SerializeObject(entitiesOutputObject, jsonSerializationOptions);
                    WriteFileToDisk(entityOutputObjectJsonString, entityJsonFileInfo);
                }

                // write intent file
                FileInfo intentJsonFileInfo = new FileInfo(Path.Combine(entitiesFolderToBeCompressed, "intent.json"));
                var intentOutputObjectJsonString = JsonConvert.SerializeObject(intentsOutputObject, jsonSerializationOptions);
                WriteFileToDisk(intentOutputObjectJsonString, intentJsonFileInfo);

                // compress
                var zipOutputFileName = modelFolder + "W.zip";
                FileInfo zipOutput = new FileInfo(zipOutputFileName);
                if (zipOutput.Exists) { zipOutput.Delete(); }

                ZipFile.CreateFromDirectory(
                    folderToBeCompressed, 
                    zipOutputFileName,
                    CompressionLevel.Optimal, 
                    true
                );

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


            Console.WriteLine("Finished. Press ENTER to exit");
            Console.ReadLine();
        }

        private static void WriteFileToDisk(string dataToWrite, FileInfo fileToWriteInto)
        {
            var jsonFileWritter = fileToWriteInto.CreateText();

            jsonFileWritter.WriteLine(dataToWrite);

            jsonFileWritter.Flush();
            jsonFileWritter.Close();
        }

        private static void LogCommandLineParameterToConsole(string switchName, object value)
        {
            Console.WriteLine("Switch {0} with value of {1}.", switchName, value.ToString());
        }
    }

    public class CustomContractResolver : CamelCasePropertyNamesContractResolver
    {
        public CustomContractResolver() : base()
        {
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            if (member.Name == "ZipCommand")
            {
                var jsonProperty = base.CreateProperty(member, memberSerialization);
                jsonProperty.PropertyName = "zip-command";
                return jsonProperty;
            }

            return base.CreateProperty(member, memberSerialization);
        }
    }
}
