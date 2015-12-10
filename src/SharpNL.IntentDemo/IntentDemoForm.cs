using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

        public IntentDemoForm()
        {
            isTrained = false;
            string mModelPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            mIntentModelPath = new System.Uri(mModelPath).LocalPath + @"\Intents\";
            InitializeComponent();
        }

        private void btnLearn_Click(object sender, EventArgs e)
        {
            string[] fileNames = new string[] { "current.txt", "five.txt", "hourly.txt" };



            isTrained = true;
            btnParse.Enabled = true;
        }

        private void btnParse_Click(object sender, EventArgs e)
        {

        }
    }
}
