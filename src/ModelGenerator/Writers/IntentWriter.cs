﻿using ModelGenerator.Models;
using ModelGenerator.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ModelGenerator.Writers
{
    struct IntentProcessingOptions
    {
        public bool InlineIds;
        public bool IgnoreHeaderLine;
    }

    class IntentWriter
    {
        private IEntityParser entityParser;
        private IIntentLineSplitter intentLineSplitter;

        public IntentWriter(IIntentLineSplitter intentLineSplitter, IEntityParser entityParser)
        {
            this.intentLineSplitter = intentLineSplitter;
            this.entityParser = entityParser;
        }

        public void Write(IList<Intent> intents, string folderRelativePath, IList<Entity> entities, IntentProcessingOptions options)
        {
            var currentFolder = Directory.GetCurrentDirectory();

            var entitiesAbsolutePath = Path.Combine(currentFolder, folderRelativePath);

            Console.WriteLine("Writing intents to {0}", entitiesAbsolutePath);

            var directoryInfo = new DirectoryInfo(entitiesAbsolutePath);
            if (!directoryInfo.Exists)
            {
                Console.Write("Folder {0} doesn't exist. Creating ... ", entitiesAbsolutePath);
                directoryInfo.Create();
                Console.WriteLine("done");
            }

            int count = 1;
            int total = intents.Count;
            foreach (var intent in intents)
            {
                Console.WriteLine("Processing intent {0} of {1}", count, total);
                count += 1;
                var fileName = string.Format("{0}.json", intent.IntentName);
                var fileAbsolutePath = Path.Combine(entitiesAbsolutePath, fileName);
                var fileOutputStream = File.CreateText(fileAbsolutePath);

                Console.WriteLine("Writing file {0} ... ", fileName);

                MemoryStream memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(intent.Text));
                StreamReader streamReader = new StreamReader(memoryStream);

                List<string> lines = new List<string>();

                while (!streamReader.EndOfStream)
                {
                    lines.Add(streamReader.ReadLine());
                }

                streamReader.Dispose();
                memoryStream.Dispose();

                IntentsOutputObject intentsOutput = new IntentsOutputObject();
                intentsOutput.Id = intent.Id;
                intentsOutput.Name = intent.IntentName;
                intentsOutput.UserSays = new List<UserSay>(lines.Count);

                IntentResponse intentResponse = new IntentResponse();
                intentsOutput.Responses.Add(intentResponse);

                int startIndex = options.IgnoreHeaderLine ? 1 : 0;

                for (int i = startIndex; i < lines.Count; i += 1)
                {
                    var line = lines[i];

                    UserSay outputObject = new UserSay();
                    outputObject.Id = Guid.NewGuid().ToString();
                    outputObject.Data = new List<UserSayData>();

                    // Line is a special csv string, split 
                    var lineParts = this.intentLineSplitter.Split(line);
                    var fragmentsContainer = string.Empty;

                    if (lineParts.Length == 1)
                    {
                        fragmentsContainer = lineParts[0];
                    }
                    else if (lineParts.Length > 1)
                    {
                        if (options.InlineIds)
                        {
                            outputObject.Id = lineParts[0];
                        }
                        fragmentsContainer = lineParts[1];
                    }

                    List<string> fragments = new List<string>();
                    fragmentsContainer = fragmentsContainer.Replace("\"", "");
                    var fragmentTokens = fragmentsContainer.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                    bool isBeginning = true;
                    EntityParserResult entityParserResult = new EntityParserResult();
                    for (int j = 0; j < fragmentTokens.Length; j += 1)
                    {
                        var token = fragmentTokens[j];

                        if (token.StartsWith("<"))
                        {
                            entityParserResult = this.entityParser.Parse(token);

                            if (entityParserResult.HasError)
                            {
                                Console.WriteLine("Ignoring line {0} of file {1}: {2}", i + 1, fileName, line);
                                Console.WriteLine("Error {0}", entityParserResult.ErrorMessage);
                                break;
                            }
                            else
                            {
                                // if recognized entity doesn't exist add it to list of entities
                                bool entityExists = entities.Any(ent => ent.EntityName == entityParserResult.Name);
                                if (!entityExists)
                                {
                                    Console.WriteLine("Entity {0} not present in list of entities", entityParserResult.Name);
                                    Entity entity = new Entity();
                                    entity.EntityName = entityParserResult.Name;
                                    entity.Id = Guid.NewGuid().ToString();
                                    entity.Synonyms = new string[] { entityParserResult.Name };
                                    entities.Add(entity);
                                    Console.WriteLine("Entity {0} added to list of entities", entityParserResult.Name);
                                }

                                if (fragments.Count > 0)
                                {
                                    UserSayData userSayDataInner = new UserSayData();
                                    if (isBeginning)
                                    {
                                        userSayDataInner.Text = String.Format("{0} ", String.Join(" ", fragments));
                                        isBeginning = false;
                                    }
                                    else
                                    {
                                        userSayDataInner.Text = String.Format(" {0} ", String.Join(" ", fragments));
                                    }
                                    fragments.Clear();
                                    outputObject.Data.Add(userSayDataInner);
                                }

                                UserSayData userSayData = new UserSayData();
                                userSayData.Text = entityParserResult.Value;
                                userSayData.Alias = entityParserResult.Name;
                                userSayData.Meta = string.Format("@{0}", entityParserResult.Name);
                                userSayData.UserDefined = true;
                                outputObject.Data.Add(userSayData);

                                if (!intentResponse.Parameters.Any(p => p.Name == entityParserResult.Name))
                                {
                                    IntentParameter parameter = new IntentParameter(entityParserResult.Name);
                                    intentResponse.Parameters.Add(parameter);
                                }
                            }
                        }
                        else
                        {
                            fragments.Add(token);
                        }
                    }

                    if (!entityParserResult.HasError)
                    {
                        if (fragments.Count > 0)
                        {
                            UserSayData userSayDataInner = new UserSayData();
                            if (isBeginning)
                            {
                                userSayDataInner.Text = String.Format("{0}", String.Join(" ", fragments));
                            }
                            else
                            {
                                userSayDataInner.Text = String.Format(" {0}", String.Join(" ", fragments));
                            }
                            fragments.Clear();
                            outputObject.Data.Add(userSayDataInner);
                        }

                        intentsOutput.UserSays.Add(outputObject);
                    }
                }

                intentsOutput.UserSays.Sort(new UserSayComparer());

                string outputString = JsonConvert.SerializeObject(intentsOutput, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                });
                outputString = outputString.Replace("\r", "");
                fileOutputStream.Write(outputString);

                fileOutputStream.Flush();
                fileOutputStream.Dispose();

                Console.WriteLine("Done", entitiesAbsolutePath);
            }
        }

        private class UserSayComparer : IComparer<UserSay>
        {
            public int Compare(UserSay x, UserSay y)
            {
                var result = x.Id.Length - y.Id.Length;
                if (result != 0) { return result; }

                return x.Id.CompareTo(y.Id);
            }
        }
    }
}
