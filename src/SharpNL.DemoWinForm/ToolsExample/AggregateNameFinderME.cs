using SharpNL.ML.MaxEntropy;
using SharpNL.ML.MaxEntropy.IO;
using SharpNL.ML.Model;
using SharpNL.NameFind;
using SharpNL.Parser;
using SharpNL.Tokenize;
using SharpNL.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNL.DemoWinForm.ToolsExample
{
    /// <summary>
    /// This is an aggregate maximum entropy name finder
    /// It has to be an aggregate because NameFinderME can only load one .bin model (ie en-ner-date.bin) and there are seven models
    /// en-ner-[date,location,money,organization,percentage,person,time]
    /// </summary>
    class AggregateNameFinderME
    {
        private Dictionary<string, NameFinderME> mFinders;
        private string mModelPath;

        public const string Start = "start";
        public const string Continue = "cont";
        public const string Other = "other";

        public AggregateNameFinderME(string modelPath)
        {
            mModelPath = modelPath;
            mFinders = new Dictionary<string, NameFinderME>();
        }

        private Span[] TokenizeToSpans(string input)
        {
            CharacterEnum charType = CharacterEnum.Whitespace;
            CharacterEnum state = charType;

            List<Span> tokens = new List<Span>();
            int inputLength = input.Length;
            int start = -1;
            char previousChar = (char)(0);
            for (int characterIndex = 0; characterIndex < inputLength; characterIndex++)
            {
                char c = input[characterIndex];
                if (System.Char.IsWhiteSpace(c))
                {
                    charType = CharacterEnum.Whitespace;
                }
                else if (System.Char.IsLetter(c))
                {
                    charType = CharacterEnum.Alphabetic;
                }
                else if (System.Char.IsDigit(c))
                {
                    charType = CharacterEnum.Numeric;
                }
                else
                {
                    charType = CharacterEnum.Other;
                }
                if (state == CharacterEnum.Whitespace)
                {
                    if (charType != CharacterEnum.Whitespace)
                    {
                        start = characterIndex;
                    }
                }
                else
                {
                    if (charType != state || (charType == CharacterEnum.Other && c != previousChar))
                    {
                        tokens.Add(new Span(start, characterIndex));
                        start = characterIndex;
                    }
                }
                state = charType;
                previousChar = c;
            }
            if (charType != CharacterEnum.Whitespace)
            {
                tokens.Add(new Span(start, inputLength));
            }
            return tokens.ToArray();
        }

        private string[] SpansToStrings(Span[] spans, string input)
        {
            string[] tokens = new string[spans.Length];
            for (int currentSpan = 0, spanCount = spans.Length; currentSpan < spanCount; currentSpan++)
            {
                tokens[currentSpan] = input.Substring(spans[currentSpan].Start, (spans[currentSpan].End) - (spans[currentSpan].Start));
            }
            return tokens;
        }

        private void AddNames(string tag, List<Span> names, Parse[] tokens, Parse lineParse)
        {
            for (int currentName = 0, nameCount = names.Count; currentName < nameCount; currentName++)
            {
                Span nameTokenSpan = names[currentName];
                Parse startToken = tokens[nameTokenSpan.Start];
                Parse endToken = tokens[nameTokenSpan.End];
                Parse commonParent = startToken.GetCommonParent(endToken);

                if (commonParent != null)
                {
                    Span nameSpan = new Span(startToken.Span.Start, endToken.Span.End);
                    if (nameSpan.Equals(commonParent.Span))
                    {

                        commonParent.Insert(new Parse(commonParent.Text, nameSpan, tag, 1.0, 0));
                    }
                    else
                    {
                        Parse[] kids = commonParent.Children;
                        bool crossingKids = false;
                        for (int currentKid = 0, kidCount = kids.Length; currentKid < kidCount; currentKid++)
                        {
                            if (nameSpan.Crosses(kids[currentKid].Span))
                            {
                                crossingKids = true;
                            }
                        }
                        if (!crossingKids)
                        {
                            commonParent.Insert(new Parse(commonParent.Text, nameSpan, tag, 1.0, 0));
                        }
                        else
                        {
                            if (commonParent.Type == "NP")
                            {
                                Parse[] grandKids = kids[0].Children;
                                if (grandKids.Length > 1 && nameSpan.Contains(grandKids[grandKids.Length - 1].Span))
                                {
                                    commonParent.Insert(new Parse(commonParent.Text, commonParent.Span, tag, 1.0, 0));
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds sgml style name tags to the specified input string and outputs this information.
        /// </summary>
        /// <param name="models">
        /// The model names for the name finders to be used.
        /// </param>
        /// <param name="input">
        /// The input.
        /// </param>
        private string ProcessText(string[] models, string line)
        {
            string output = line.ToString();

            Span[][] finderTags = new Span[models.Length][];

            Span[] spans = TokenizeToSpans(line);
            string[] tokens = SpansToStrings(spans, line);
            for (int currentFinder = 0, finderCount = models.Length; currentFinder < finderCount; currentFinder++)
            {
                NameFinderME finder = mFinders[models[currentFinder]];
                finderTags[currentFinder] = finder.Find(tokens);
            }

            for (int currentFinder = 0, finderCount = models.Length; currentFinder < finderCount; currentFinder++)
            {
                Span[] currentTags = finderTags[currentFinder];
                for (int currentSpan = 0, spanCount = currentTags.Length; currentSpan < spanCount; currentSpan++)
                {
                    Span span = currentTags[currentSpan];
                    string coveredText = span.GetCoveredText(tokens);

                    string replaceText = string.Format("<{0}>{1}</{2}>", models[currentFinder], coveredText, models[currentFinder]);

                    output = output.Replace(coveredText, replaceText);
                }
            }
            output = output + "\r\n";
            return output;
        }

        public string GetNames(string[] models, string data)
        {
            CreateModels(models);
            return ProcessText(models, data);
        }

        private void CreateModels(string[] models)
        {
            for (int currentModel = 0; currentModel < models.Length; currentModel++)
            {
                if (!mFinders.ContainsKey(models[currentModel]))
                {
                    string modelName = mModelPath + "en-ner-" + models[currentModel] + ".bin";
                    try
                    {
                        FileStream fileStream = new FileStream(modelName, FileMode.Open);
                        TokenNameFinderModel nameFinderModel = new TokenNameFinderModel(fileStream);
                        NameFinderME finder = new NameFinderME(nameFinderModel);
                        mFinders.Add(models[currentModel], finder);
                        fileStream.Dispose();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            }
        }
    }
}
