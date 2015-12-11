using SharpNL.DocumentCategorizer;
using SharpNL.NameFind;
using SharpNL.Tokenize;
using SharpNL.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SharpNL.IntentDemo
{
    public partial class IntentDemoForm : Form
    {
        private bool isTrained;
        private string mIntentModelPath;

        private DocumentCategorizerME _documentCategorizer;
        private NameFinderME[] _nameFinderMEs;

        public IntentDemoForm()
        {
            isTrained = false;
            string mModelPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            mIntentModelPath = new System.Uri(mModelPath).LocalPath + @"\Intents\";
            InitializeComponent();
        }

        private void btnLearn_Click(object sender, EventArgs e)
        {
            btnParse.Enabled = false;
            string[] fileNames = new string[] { "current.txt", "five.txt", "hourly.txt" };

            List<IObjectStream<DocumentSample>> trainingSamples = new List<IObjectStream<DocumentSample>>();
            foreach (string fileName in fileNames)
            {
                String intent = fileName.Replace(".txt", "");
                FileStream fileStream = new FileStream(mIntentModelPath + fileName, FileMode.Open);
                IObjectStream<string> lineStream = new PlainTextByLineStream(fileStream);
                IObjectStream<DocumentSample> docSampleStream = new IntentDocumentSampleStream(intent, lineStream);
                trainingSamples.Add(docSampleStream);
            }

            IObjectStream<DocumentSample> combinedDocumentSampleStream = new CombinedObjectStream<DocumentSample>(trainingSamples);
            DocumentCategorizerModel doccatModel = DocumentCategorizerME.Train("en", combinedDocumentSampleStream, new TrainingParameters(), new DocumentCategorizerFactory());
            combinedDocumentSampleStream.Dispose();


            List<TokenNameFinderModel> tokenNameFinderModels = new List<TokenNameFinderModel>();

            List<IObjectStream<NameSample>> nameStreams = new List<IObjectStream<NameSample>>();
            foreach (string fileName in fileNames)
            {
                FileStream fileStream = new FileStream(mIntentModelPath + fileName, FileMode.Open);
                IObjectStream<string> lineStream = new PlainTextByLineStream(fileStream);
                IObjectStream<NameSample> nameSampleStream = new NameSampleStream(lineStream);
                nameStreams.Add(nameSampleStream);
            }

            IObjectStream<NameSample> combinedNameSampleStream = new CombinedObjectStream<NameSample>(nameStreams);
            TokenNameFinderModel tokenNameFinderModel = NameFinderME.Train("en", "city", combinedNameSampleStream, TrainingParameters.DefaultParameters(), new TokenNameFinderFactory());
            combinedNameSampleStream.Dispose();

            tokenNameFinderModels.Add(tokenNameFinderModel);

            DocumentCategorizerME categorizer = new DocumentCategorizerME(doccatModel);
            _documentCategorizer = categorizer;

            NameFinderME[] nameFinderMEs = new NameFinderME[tokenNameFinderModels.Count];
            for (int i = 0; i < nameFinderMEs.Length; i++)
            {
                nameFinderMEs[i] = new NameFinderME(tokenNameFinderModels[i]);
            }
            _nameFinderMEs = nameFinderMEs;
            // categorizer and array of nameFinderMes is final result;

            isTrained = true;
            btnParse.Enabled = true;
        }

        private void btnParse_Click(object sender, EventArgs e)
        {
            String text = txtInput.Text;
            StringBuilder result = new StringBuilder();

            double[] outcome = _documentCategorizer.Categorize(text);
            result.Append(_documentCategorizer.GetBestCategory(outcome));
            result.Append(" ");

            String[] tokens = WhitespaceTokenizer.Instance.Tokenize(text);
            foreach (NameFinderME nameFinder in _nameFinderMEs)
            {
                Span[] spans = nameFinder.Find(tokens);
                String[] names = Span.SpansToStrings(spans, tokens);
                for (int i = 0; i < spans.Length; i++)
                {
                    result.Append(string.Format("{0}={1} ", spans[i].Type, names[i]));
                }
            }

            txtResults.Text = result.ToString();
        }
    }
}
