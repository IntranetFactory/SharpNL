using ModelGenerator.Models;
using ModelGenerator.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModelGenerator.Parsers
{
    /// <summary>
    /// Simple intent parser is used when InlineIds is false
    /// </summary>
    class SimpleIntentParser
    {
        private IIntentLineParser intentLineParser;

        public SimpleIntentParser(IIntentLineParser intentLineParser)
        {
            // we use intent line parser because there is a part of line parsing which is the same for both simple mode (InlineIds = false) and InlineId mode (InlineIds = true)
            this.intentLineParser = intentLineParser;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="intentFileInfos">intentFileInfos that will be processed</param>
        /// <param name="entities">List of already existing entities</param>
        /// <returns></returns>
        public IList<IntentsOutputObject> Parse(IList<IntentFileInfo> intentFileInfos, IList<Entity> entities)
        {
            // we produce list of IntentsOutputObject as that will be used to write to disk
            List<IntentsOutputObject> result = new List<IntentsOutputObject>();

            for (int fileInfoIndex = 0; fileInfoIndex < intentFileInfos.Count; fileInfoIndex++)
            {
                // intentFileInfo has info about intent definitions 
                // as content to be processed intentFileInfo has list of lines read from file
                var intentFileInfo = intentFileInfos[fileInfoIndex];

                var intentsOutputObject = new IntentsOutputObject();
                intentsOutputObject.Id = intentFileInfo.Id;
                intentsOutputObject.Name = intentFileInfo.IntentName;

                intentsOutputObject.UserSays = new List<UserSay>();

                IntentResponse intentResponse = new IntentResponse();
                intentsOutputObject.Responses.Add(intentResponse);

                for (int lineIndex = 0; lineIndex < intentFileInfo.Lines.Count; lineIndex++)
                {
                    var line = intentFileInfo.Lines[lineIndex];

                    UserSay userSay = new UserSay();
                    userSay.Id = Guid.NewGuid().ToString();
                    userSay.Data = new List<UserSayData>();

                    // use intent line parser to parse the line
                    // line has text and entities
                    // result produces error/errorMessage, parsed entities and UserSayData objects
                    IntentLineParserResult intentLineParserResult = this.intentLineParser.Parse(line);
                    
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
                            bool entityExists = entities.Any(ent => ent.EntityName == entity.EntityName);
                            if (!entityExists)
                            {
                                Console.WriteLine("Entity {0} not present in list of entities", entity.EntityName);
                                entities.Add(entity);
                                Console.WriteLine("Entity {0} added to list of entities", entity.EntityName);
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

                result.Add(intentsOutputObject);
            }

            return result;
        }
    }

}
