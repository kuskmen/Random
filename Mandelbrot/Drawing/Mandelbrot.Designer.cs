using System.Windows.Forms;

namespace Drawing
{
    partial class Mandelbrot {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Mandelbrot));
            this.generatePatternButton = new System.Windows.Forms.Button();
            this.mandelbrotPb = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.iterationCountTextBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.stopwatchLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.mandelbrotPb)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // generatePatternButton
            // 
            this.generatePatternButton.BackColor = System.Drawing.SystemColors.Control;
            this.generatePatternButton.Location = new System.Drawing.Point(124, 30);
            this.generatePatternButton.Name = "generatePatternButton";
            this.generatePatternButton.Size = new System.Drawing.Size(127, 22);
            this.generatePatternButton.TabIndex = 0;
            this.generatePatternButton.Text = "Generate Pattern";
            this.generatePatternButton.UseVisualStyleBackColor = false;
            this.generatePatternButton.Click += new System.EventHandler(this.RenderMandelbrot);
            // 
            // mandelbrotPb
            // 
            this.mandelbrotPb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mandelbrotPb.Location = new System.Drawing.Point(0, 0);
            this.mandelbrotPb.Name = "mandelbrotPb";
            this.mandelbrotPb.Size = new System.Drawing.Size(989, 652);
            this.mandelbrotPb.TabIndex = 20;
            this.mandelbrotPb.TabStop = false;
            this.mandelbrotPb.MouseClick += this.Mandelbrot_MouseClick;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Iterations";
            // 
            // iterationCountTextBox
            // 
            this.iterationCountTextBox.Location = new System.Drawing.Point(15, 32);
            this.iterationCountTextBox.Name = "iterationCountTextBox";
            this.iterationCountTextBox.Size = new System.Drawing.Size(103, 20);
            this.iterationCountTextBox.TabIndex = 5;
            this.iterationCountTextBox.Text = "85";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.generatePatternButton);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.iterationCountTextBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(258, 60);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Arguments";
            this.groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.stopwatchLabel);
            this.groupBox2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.groupBox2.Location = new System.Drawing.Point(828, 596);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(149, 44);
            this.groupBox2.TabIndex = 20;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Timer";
            // 
            // stopwatchLabel
            // 
            this.stopwatchLabel.AutoSize = true;
            this.stopwatchLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stopwatchLabel.Location = new System.Drawing.Point(9, 16);
            this.stopwatchLabel.Name = "stopwatchLabel";
            this.stopwatchLabel.Size = new System.Drawing.Size(0, 20);
            this.stopwatchLabel.TabIndex = 0;
            // 
            // Mandelbrot
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(989, 652);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.mandelbrotPb);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Mandelbrot";
            this.Text = "Mandelbrot";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.mandelbrotPb)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button generatePatternButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox iterationCountTextBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label stopwatchLabel;
        private System.Windows.Forms.PictureBox mandelbrotPb;
    }
}

