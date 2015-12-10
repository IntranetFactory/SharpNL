namespace SharpNL.IntentDemo
{
    partial class IntentDemoForm
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
            this.txtInput = new System.Windows.Forms.TextBox();
            this.txtResults = new System.Windows.Forms.TextBox();
            this.btnLearn = new System.Windows.Forms.Button();
            this.btnParse = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtInput
            // 
            this.txtInput.Location = new System.Drawing.Point(12, 12);
            this.txtInput.Multiline = true;
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(576, 62);
            this.txtInput.TabIndex = 0;
            // 
            // txtResults
            // 
            this.txtResults.Location = new System.Drawing.Point(12, 120);
            this.txtResults.Multiline = true;
            this.txtResults.Name = "txtResults";
            this.txtResults.Size = new System.Drawing.Size(570, 206);
            this.txtResults.TabIndex = 1;
            // 
            // btnLearn
            // 
            this.btnLearn.Location = new System.Drawing.Point(155, 80);
            this.btnLearn.Name = "btnLearn";
            this.btnLearn.Size = new System.Drawing.Size(144, 34);
            this.btnLearn.TabIndex = 2;
            this.btnLearn.Text = "Learn";
            this.btnLearn.UseVisualStyleBackColor = true;
            this.btnLearn.Click += new System.EventHandler(this.btnLearn_Click);
            // 
            // btnParse
            // 
            this.btnParse.Enabled = false;
            this.btnParse.Location = new System.Drawing.Point(305, 80);
            this.btnParse.Name = "btnParse";
            this.btnParse.Size = new System.Drawing.Size(144, 34);
            this.btnParse.TabIndex = 3;
            this.btnParse.Text = "Parse";
            this.btnParse.UseVisualStyleBackColor = true;
            this.btnParse.Click += new System.EventHandler(this.btnParse_Click);
            // 
            // IntentDemoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(594, 338);
            this.Controls.Add(this.btnParse);
            this.Controls.Add(this.btnLearn);
            this.Controls.Add(this.txtResults);
            this.Controls.Add(this.txtInput);
            this.Name = "IntentDemoForm";
            this.Text = "Intent Demo";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.TextBox txtResults;
        private System.Windows.Forms.Button btnLearn;
        private System.Windows.Forms.Button btnParse;
    }
}

