
namespace BaseSim2021
{
    partial class ValueExplorer
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
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.button1 = new System.Windows.Forms.Button();
            this.Appliquer = new System.Windows.Forms.Button();
            this.TitreLabel = new System.Windows.Forms.Label();
            this.DescLabel = new System.Windows.Forms.Label();
            this.gloryEst = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(13, 436);
            this.numericUpDown1.Margin = new System.Windows.Forms.Padding(4);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(481, 22);
            this.numericUpDown1.TabIndex = 0;
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(501, 528);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(267, 44);
            this.button1.TabIndex = 1;
            this.button1.Text = "Quitter";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // Appliquer
            // 
            this.Appliquer.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Appliquer.Location = new System.Drawing.Point(501, 463);
            this.Appliquer.Margin = new System.Windows.Forms.Padding(4);
            this.Appliquer.Name = "Appliquer";
            this.Appliquer.Size = new System.Drawing.Size(267, 44);
            this.Appliquer.TabIndex = 3;
            this.Appliquer.Text = "Appliquer";
            this.Appliquer.UseVisualStyleBackColor = true;
            // 
            // TitreLabel
            // 
            this.TitreLabel.AutoSize = true;
            this.TitreLabel.BackColor = System.Drawing.SystemColors.Control;
            this.TitreLabel.Location = new System.Drawing.Point(303, 9);
            this.TitreLabel.Name = "TitreLabel";
            this.TitreLabel.Size = new System.Drawing.Size(40, 17);
            this.TitreLabel.TabIndex = 4;
            this.TitreLabel.Text = "        ";
            // 
            // DescLabel
            // 
            this.DescLabel.AutoSize = true;
            this.DescLabel.BackColor = System.Drawing.SystemColors.Control;
            this.DescLabel.Location = new System.Drawing.Point(303, 50);
            this.DescLabel.Name = "DescLabel";
            this.DescLabel.Size = new System.Drawing.Size(52, 17);
            this.DescLabel.TabIndex = 5;
            this.DescLabel.Text = "           ";
            // 
            // gloryEst
            // 
            this.gloryEst.AutoSize = true;
            this.gloryEst.Location = new System.Drawing.Point(12, 490);
            this.gloryEst.Name = "gloryEst";
            this.gloryEst.Size = new System.Drawing.Size(56, 17);
            this.gloryEst.TabIndex = 6;
            this.gloryEst.Text = "            ";
            // 
            // ValueExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(781, 585);
            this.Controls.Add(this.gloryEst);
            this.Controls.Add(this.DescLabel);
            this.Controls.Add(this.TitreLabel);
            this.Controls.Add(this.Appliquer);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.numericUpDown1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ValueExplorer";
            this.Text = "ValueExplorer";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ValueExplorer_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button Appliquer;
        private System.Windows.Forms.Label TitreLabel;
        private System.Windows.Forms.Label DescLabel;
        private System.Windows.Forms.Label gloryEst;
    }
}