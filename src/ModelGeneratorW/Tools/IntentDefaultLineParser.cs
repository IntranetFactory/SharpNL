using ModelGeneratorW.Models;
using System;
using System.Collections.Generic;

namespace ModelGeneratorW.Tools
{
    class IntentLineParserResult
    {
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }

        public List<UserSayData> UserSayDatas { get; set; }
        public List<Entity> Entities { get; set; }

        public IntentLineParserResult()
        {
            UserSayDatas = new List<UserSayData>();
            Entities = new List<Entity>();
        }
    }

    class IntentDefaultLineParser : IIntentLineParser
    {
        private IEntityParser entityParser;

        public IntentDefaultLineParser(IEntityParser entityParser)
        {
            this.entityParser = entityParser;
        }

        public IntentLineParserResult Parse(string line)
        {
            IntentLineParserResult result = new IntentLineParserResult();

            // line tokens are input data
            var lineTokens = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // line fragments will store output data
            var lineFragmets = new List<string>(lineTokens.Length);
            bool isBeginning = true;

            // if any of line tokens is a potential entity we will parse it and store parse result here
            EntityParserResult entityParserResult = new EntityParserResult();

            for (int tokenIndex = 0; tokenIndex < lineTokens.Length; tokenIndex++)
            {
                var token = lineTokens[tokenIndex];
                token = token.Replace("\"", "");

                if (token.StartsWith("<"))
                {
                    entityParserResult = this.entityParser.Parse(token);

                    if (entityParserResult.HasError)
                    {
                        result.HasError = true;
                        result.ErrorMessage = entityParserResult.ErrorMessage;
                        break;
                    }
                    else
                    {
                        Entity entity = new Entity();
                        entity.EntityName = entityParserResult.Name;
                        entity.Id = Guid.NewGuid().ToString();
                        var synonymStr = string.Format("{0},{1}", entityParserResult.Value, entityParserResult.Value);
                        entity.Synonyms = new string[] { synonymStr };
                        result.Entities.Add(entity);

                        if (lineFragmets.Count > 0)
                        {
                            UserSayData userSayDataInner = new UserSayData();
                            if (isBeginning)
                            {
                                userSayDataInner.Text = String.Format("{0} ", String.Join(" ", lineFragmets));
                                isBeginning = false;
                            }
                            else
                            {
                                userSayDataInner.Text = String.Format(" {0} ", String.Join(" ", lineFragmets));
                            }
                            lineFragmets.Clear();
                            result.UserSayDatas.Add(userSayDataInner);
                        }

                        UserSayData userSayData = new UserSayData();
                        userSayData.Text = entityParserResult.Value;
                        userSayData.Alias = entityParserResult.Name;
                        userSayData.Meta = string.Format("@{0}", entityParserResult.Name);
                        userSayData.UserDefined = true;
                        result.UserSayDatas.Add(userSayData);
                    }
                }
                else
                {
                    lineFragmets.Add(token);
                }
            }

            if (!entityParserResult.HasError)
            {
                if (lineFragmets.Count > 0)
                {
                    UserSayData userSayDataInner = new UserSayData();
                    if (isBeginning)
                    {
                        userSayDataInner.Text = String.Format("{0}", String.Join(" ", lineFragmets));
                    }
                    else
                    {
                        userSayDataInner.Text = String.Format(" {0}", String.Join(" ", lineFragmets));
                    }
                    lineFragmets.Clear();
                    result.UserSayDatas.Add(userSayDataInner);
                }
            }

            return result;
        }
    }
}
