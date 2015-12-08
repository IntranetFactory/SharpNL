using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;
using Netron.Lithium;
using SharpNL.Parser;
using SharpNL.Utility;
using SharpNL.ML.Model;
using SharpNL.ML.MaxEntropy;
using SharpNL.ML.MaxEntropy.IO;
using System.IO;
using SharpNL.POSTag;
using SharpNL.Chunker;
using SharpNL.Parser.Lang.en;
using SharpNL.Analyzer;
using SharpNL;
using SharpNL.SentenceDetector;

namespace ParseTree
{
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    public class ParseTreeForm : System.Windows.Forms.Form
    {
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.TextBox txtInput;
        private Netron.Lithium.LithiumControl lithiumControl;
        private System.Windows.Forms.Button btnParse;

        private string mModelPath;

        private IParser mParser;
        private Parse mParse;
        private AggregateAnalyzer mAggregateAnalyzer;

        public ParseTreeForm()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            lithiumControl.Width = this.ClientRectangle.Width;
            lithiumControl.Height = this.ClientRectangle.Height - lithiumControl.Top;

            mModelPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            mModelPath = new System.Uri(mModelPath).LocalPath + @"\models-1.5\";
            //mModelPath = @"C:\Projects\DotNet\OpenNLP\OpenNLP\Models\";

            mAggregateAnalyzer = new AggregateAnalyzer
            {
                mModelPath + "en-chunker.bin",
                mModelPath + "en-ner-date.bin",
                mModelPath + "en-ner-location.bin",
                mModelPath + "en-ner-money.bin",
                mModelPath + "en-ner-organization.bin",
                mModelPath + "en-ner-percentage.bin",
                mModelPath + "en-ner-person.bin",
                mModelPath + "en-ner-time.bin",
                mModelPath + "en-parser-chunking.bin",
                mModelPath + "en-pos-maxent.bin",
                mModelPath + "en-sent.bin",
                mModelPath + "en-token.bin"
            };
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lithiumControl = new Netron.Lithium.LithiumControl();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.btnParse = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lithiumControl
            // 
            this.lithiumControl.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.lithiumControl.AutoScroll = true;
            this.lithiumControl.AutoScrollMinSize = new System.Drawing.Size(-2147483547, -2147483547);
            this.lithiumControl.BackColor = System.Drawing.SystemColors.Window;
            this.lithiumControl.BranchHeight = 70;
            this.lithiumControl.ConnectionType = Netron.Lithium.ConnectionType.Default;
            this.lithiumControl.LayoutDirection = Netron.Lithium.TreeDirection.Vertical;
            this.lithiumControl.LayoutEnabled = true;
            this.lithiumControl.Location = new System.Drawing.Point(0, 48);
            this.lithiumControl.Name = "lithiumControl";
            this.lithiumControl.Size = new System.Drawing.Size(200, 352);
            this.lithiumControl.TabIndex = 0;
            this.lithiumControl.Text = "lithiumControl";
            this.lithiumControl.WordSpacing = 20;
            // 
            // txtInput
            // 
            this.txtInput.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.txtInput.Location = new System.Drawing.Point(8, 16);
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(568, 20);
            this.txtInput.TabIndex = 1;
            this.txtInput.Text = "A rare black squirrel has become a regular visitor to a suburban garden.";
            this.txtInput.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtInput_KeyPress);
            // 
            // btnParse
            // 
            this.btnParse.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.btnParse.Location = new System.Drawing.Point(584, 16);
            this.btnParse.Name = "btnParse";
            this.btnParse.TabIndex = 2;
            this.btnParse.Text = "Parse";
            this.btnParse.Click += new System.EventHandler(this.btnParse_Click);
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(664, 478);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.btnParse,
																		  this.txtInput,
																		  this.lithiumControl});
            this.Name = "Form1";
            this.Text = "OpenNLP Parser Demo";
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        //[STAThread]
        //static void Main() 
        //{
        //    Application.Run(new ParseTreeForm());
        //}



        private void btnParse_Click(object sender, System.EventArgs e)
        {
            ShowParse();
        }

        private void AddChildNodes(ShapeBase currentShape, Parse[] childParses)
        {
            foreach (Parse childParse in childParses)
            {
                // if this is not a token node (token node = one of the words of the sentence)
                if (childParse.Type != AbstractBottomUpParser.TOK_NODE)
                {
                    ShapeBase childShape = currentShape.AddChild(childParse.Type);
                    if (childParse.IsPosTag)
                    {
                        childShape.ShapeColor = Color.DarkGoldenrod;
                    }
                    else
                    {
                        childShape.ShapeColor = Color.SteelBlue;
                    }
                    AddChildNodes(childShape, childParse.Children);
                    childShape.Expand();
                }
                else
                {
                    Span parseSpan = childParse.Span;
                    string token = childParse.Text.Substring(parseSpan.Start, (parseSpan.End) - (parseSpan.Start));
                    ShapeBase childShape = currentShape.AddChild(token);
                    childShape.ShapeColor = Color.Ivory;
                }
            }
        }

        private void ShowParse()
        {
            if (txtInput.Text.Length == 0)
            {
                return;
            }

            //prepare the UI
            txtInput.Enabled = false;
            btnParse.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            lithiumControl.NewDiagram();

            //do the parsing
            if (mParser == null)
            {
            }

            IDocument doc = new Document("en", txtInput.Text);
            mAggregateAnalyzer.Analyze(doc);

            if (doc.Sentences.Count > 0)
            {
                //display the parse result
                ShapeBase root = this.lithiumControl.Root;
                root.Text = "R";
                root.Visible = true;

                foreach (ISentence sentence in doc.Sentences)
                {
                    mParse = sentence.Parse;

                    if (mParse.Type == AbstractBottomUpParser.TOP_NODE)
                    {
                        mParse = mParse.Children[0];
                    }

                    ShapeBase sentenceNode = root.AddChild(mParse.Type);
                    AddChildNodes(sentenceNode, mParse.Children);
                }

                root.Expand();
                this.lithiumControl.DrawTree();
                //restore the UI
                this.Cursor = Cursors.Default;
                txtInput.Enabled = true;
                btnParse.Enabled = true;
            }
            else
            {
                // report error.
            }
        }

        private void txtInput_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                ShowParse();
            }
        }

    }
}
