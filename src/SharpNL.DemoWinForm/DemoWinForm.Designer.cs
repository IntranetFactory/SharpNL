namespace SharpNL.DemoWinForm
{
    partial class DemoWinForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
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
            this.btnParseTreeDemo = new System.Windows.Forms.Button();
            this.btnToolsExample = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnParseTreeDemo
            // 
            this.btnParseTreeDemo.Location = new System.Drawing.Point(78, 28);
            this.btnParseTreeDemo.Name = "btnParseTreeDemo";
            this.btnParseTreeDemo.Size = new System.Drawing.Size(144, 55);
            this.btnParseTreeDemo.TabIndex = 0;
            this.btnParseTreeDemo.Text = "Run ParseTree Demo";
            this.btnParseTreeDemo.UseVisualStyleBackColor = true;
            this.btnParseTreeDemo.Click += new System.EventHandler(this.btnParseTreeDemo_Click);
            // 
            // btnToolsExample
            // 
            this.btnToolsExample.Location = new System.Drawing.Point(78, 109);
            this.btnToolsExample.Name = "btnToolsExample";
            this.btnToolsExample.Size = new System.Drawing.Size(144, 55);
            this.btnToolsExample.TabIndex = 1;
            this.btnToolsExample.Text = "Run Tools Example";
            this.btnToolsExample.UseVisualStyleBackColor = true;
            this.btnToolsExample.Click += new System.EventHandler(this.btnToolsExample_Click);
            // 
            // DemoWinForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(304, 201);
            this.Controls.Add(this.btnToolsExample);
            this.Controls.Add(this.btnParseTreeDemo);
            this.Name = "DemoWinForm";
            this.Text = "Demo Win Form";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnParseTreeDemo;
        private System.Windows.Forms.Button btnToolsExample;
    }
}

