using ModelGenerator.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace ModelGenerator.Writers
{
    static class IntentWriter
    {
        public static void Write(IList<Intent> entities, string folderRelativePath)
        {
            var currentFolder = Directory.GetCurrentDirectory();

            var entitiesAbsolutePath = Path.Combine(currentFolder, folderRelativePath);

            System.Console.WriteLine("Writing intents to {0}", entitiesAbsolutePath);

            var directoryInfo = new DirectoryInfo(entitiesAbsolutePath);
            if (!directoryInfo.Exists)
            {
                System.Console.Write("Folder {0} doesn't exist. Creating ... ", entitiesAbsolutePath);
                directoryInfo.Create();
                System.Console.WriteLine("done");
            }

            int count = 1;
            int total = entities.Count;
            foreach (var entity in entities)
            {
                System.Console.WriteLine("Processing intent {0} of {1}", count, total);

                var fileName = string.Format("{0}.json", entity.IntentName);
                var fileAbsolutePath = Path.Combine(entitiesAbsolutePath, fileName);
                var fileOutputStream = File.CreateText(fileAbsolutePath);

                System.Console.Write("Writing file {0} ... ", fileName);

                var lines = entity.Text.Split(new String[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                IntentsOutputObject intentsOutput = new IntentsOutputObject();
                intentsOutput.Id = entity.Id;
                intentsOutput.Name = entity.IntentName;
                intentsOutput.UserSays = new List<UserSay>(lines.Length);

                IntentResponse intentResponse = new IntentResponse();
                intentsOutput.Responses.Add(intentResponse);

                var xmlDoc = new XmlDocument();

                foreach (var line in lines)
                {
                    UserSay outputObject = new UserSay();
                    outputObject.Id = Guid.NewGuid().ToString();
                    outputObject.Data = new List<UserSayData>();

                    List<string> fragments = new List<string>();
                    var lineTokens = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                    bool isBeginning = true;

                    foreach (var token in lineTokens)
                    {
                        try
                        {
                            xmlDoc.LoadXml(token);
                            var xmlNode = xmlDoc.DocumentElement;
                            var name = xmlNode.Name;
                            var value = xmlNode.FirstChild.Value;

                            if (fragments.Count > 0)
                            {
                                UserSayData userSayDataInner = new UserSayData();
                                if (isBeginning) {
                                    userSayDataInner.Text = String.Format("{0} ", String.Join(" ", fragments));
                                } else {
                                    userSayDataInner.Text = String.Format(" {0} ", String.Join(" ", fragments));
                                }
                                fragments.Clear();
                                outputObject.Data.Add(userSayDataInner);
                            }

                            UserSayData userSayData = new UserSayData();
                            userSayData.Text = value;
                            userSayData.Alias = name;
                            userSayData.Meta = string.Format("@{0}", name);
                            userSayData.UserDefined = true;
                            outputObject.Data.Add(userSayData);

                            if(!intentResponse.Parameters.Any(p => p.Name == name))
                            {
                                IntentParameter parameter = new IntentParameter(name);
                                intentResponse.Parameters.Add(parameter);
                            }
                        }
                        catch (XmlException)
                        {
                            fragments.Add(token);
                        }
                    }

                    if (fragments.Count > 0)
                    {
                        UserSayData userSayDataInner = new UserSayData();
                        userSayDataInner.Text = String.Format(" {0}", String.Join(" ", fragments));
                        fragments.Clear();
                        outputObject.Data.Add(userSayDataInner);
                    }

                    intentsOutput.UserSays.Add(outputObject);
                }

                string outputString = JsonConvert.SerializeObject(intentsOutput, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                });
                fileOutputStream.Write(outputString);

                fileOutputStream.Flush();
                fileOutputStream.Dispose();

                System.Console.WriteLine("done", entitiesAbsolutePath);
            }
        }
    }
}
