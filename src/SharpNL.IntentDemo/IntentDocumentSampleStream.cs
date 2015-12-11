using SharpNL.DocumentCategorizer;
using SharpNL.Tokenize;
using SharpNL.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNL.IntentDemo
{
    class IntentDocumentSampleStream : IObjectStream<DocumentSample>
    {
        private string _category;
        private IObjectStream<string> _stream;

        public IntentDocumentSampleStream(string category, IObjectStream<string> stream)
        {
            _category = category;
            _stream = stream;
        }

        public DocumentSample Read()
        {
            String sampleString = _stream.Read();
            if (sampleString != null)
            {
                String[] tokens = WhitespaceTokenizer.Instance.Tokenize(sampleString);

                List<String> list = new List<string>(tokens.Length);
                bool skip = false;
                foreach (string token in tokens)
                {
                    if (token.StartsWith("<"))
                    {
                        skip = !skip;
                    }
                    else if (!skip)
                    {
                        list.Add(token);
                    }
                }

                tokens = new String[list.Count];
                list.CopyTo(tokens);
                
                DocumentSample sample;
                if (tokens.Length > 0)
                {
                    sample = new DocumentSample(_category, tokens);
                }
                else
                {
                    throw new IOException("Empty lines are not allowed.");
                }
                return sample;
            }
            else
            {
                return null;
            }
        }

        public void Reset()
        {
            _stream.Reset();
        }

        public void Dispose()
        {
            _stream.Dispose();
        }
    }
}
