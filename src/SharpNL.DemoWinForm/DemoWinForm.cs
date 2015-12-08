using ParseTree;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ToolsExample;

namespace SharpNL.DemoWinForm
{
    public partial class DemoWinForm : Form
    {
        public DemoWinForm()
        {
            InitializeComponent();
        }

        private void btnParseTreeDemo_Click(object sender, EventArgs e)
        {
            ParseTreeForm ptf = new ParseTreeForm();
            ptf.ShowDialog(this);
        }

        private void btnToolsExample_Click(object sender, EventArgs e)
        {
            ToolsExampleForm tef = new ToolsExampleForm();
            tef.ShowDialog(this);
        }
    }
}
