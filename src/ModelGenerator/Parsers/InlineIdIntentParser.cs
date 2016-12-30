using ModelGenerator.Models;
using ModelGenerator.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModelGenerator.Parsers
{
    class InlineIdIntentParser
    {
        private IIntentLineParser intentLineParser;
        private IIntentLineSplitter intentLineSplitter;

        public InlineIdIntentParser(IIntentLineSplitter intentLineSplitter, IIntentLineParser intentLineParser)
        {
            this.intentLineSplitter = intentLineSplitter;
            this.intentLineParser = intentLineParser;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="intentFileInfos">intentFileInfos that will be processed</param>
        /// <param name="entities">List of already existing entities</param>
        /// <returns></returns>
        public IList<IntentsOutputObject> Parse(IList<IntentFileInfo> intentFileInfos, IList<Entity> entities, bool ignoreHeaderLines)
        {
            // we produce list of IntentsOutputObject as that will be used to write to disk
            List<IntentsOutputObject> result = new List<IntentsOutputObject>();

            Dictionary<string, IntentsOutputObject> currentData = new Dictionary<string, IntentsOutputObject>();

            for (int fileInfoIndex = 0; fileInfoIndex < intentFileInfos.Count; fileInfoIndex++)
            {
                // intentFileInfo has info about intent definitions 
                // as content to be processed intentFileInfo has list of lines read from file
                var intentFileInfo = intentFileInfos[fileInfoIndex];

                int defaultLineIndex = ignoreHeaderLines ? 1 : 0;

                for (int lineIndex = defaultLineIndex; lineIndex < intentFileInfo.Lines.Count; lineIndex++)
                {
                    var line = intentFileInfo.Lines[lineIndex];

                    var lineParts = this.intentLineSplitter.Split(line);
                    if (lineParts.Length < 2) { continue; }

                    var intentId = lineParts[0].Trim();
                    var intentText = lineParts[1];

                    if (string.IsNullOrEmpty(intentId)) { continue; }
                    if (string.IsNullOrEmpty(intentText)) { continue; }

                    var intentsOutputObject = new IntentsOutputObject();
                    var intentResponse = new IntentResponse();

                    if (currentData.ContainsKey(intentId))
                    {
                        intentsOutputObject = currentData[intentId];
                        intentResponse = intentsOutputObject.Responses[0];
                    }
                    else
                    {
                        intentsOutputObject.Id = Guid.NewGuid().ToString();
                        intentsOutputObject.Name = intentId;

                        intentsOutputObject.UserSays = new List<UserSay>();

                        intentResponse = new IntentResponse();
                        intentsOutputObject.Responses.Add(intentResponse);
                        currentData.Add(intentId, intentsOutputObject);
                    }

                    UserSay userSay = new UserSay();
                    userSay.Id = Guid.NewGuid().ToString();
                    userSay.Data = new List<UserSayData>();

                    var intentLineParserResult = this.intentLineParser.Parse(intentText);
                    if (intentLineParserResult.HasError)
                    {
                        Console.WriteLine("Ignoring line {0} of file {1}: {2}", lineIndex + 1, intentFileInfo.IntentName, line);
                        Console.WriteLine("Error {0}", intentLineParserResult.ErrorMessage);
                        continue;
                    }
                    else
                    {
                        // if any entities were found, see if they already exist in entities list
                        // if not add new entities to already existing entities list
                        for (int entityIndex = 0; entityIndex < intentLineParserResult.Entities.Count; entityIndex++)
                        {
                            Entity entity = intentLineParserResult.Entities[entityIndex];

                            // if recognized entity doesn't exist add it to list of entities
                            var existingEntity = entities.FirstOrDefault(ent => ent.EntityName == entity.EntityName);
                            if (existingEntity == null)
                            {
                                Console.WriteLine("Entity {0} not present in list of entities", entity.EntityName);
                                entities.Add(entity);
                                Console.WriteLine("Entity {0} added to list of entities", entity.EntityName);
                            }
                            else
                            {
                                existingEntity.Synonyms = existingEntity.Synonyms.Concat(entity.Synonyms).ToArray();
                            }

                            if (!intentResponse.Parameters.Any(p => p.Name == entity.EntityName))
                            {
                                IntentParameter parameter = new IntentParameter(entity.EntityName);
                                intentResponse.Parameters.Add(parameter);
                            }
                        }

                        // add UserSayData objects to userSay output object
                        userSay.Data.AddRange(intentLineParserResult.UserSayDatas);
                    }

                    intentsOutputObject.UserSays.Add(userSay);
                }
            }

            result = currentData.Values.ToList();

            return result;
        }
    }
}
