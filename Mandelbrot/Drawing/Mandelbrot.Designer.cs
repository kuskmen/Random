namespace Mandelbrot
{
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    
    partial class Mandelbrot
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            var resources = new ComponentResourceManager(typeof(Mandelbrot));
            this.RenderButton = new Button();
            this.mandelbrotPb = new PictureBox();
            this.iterationsLb = new Label();
            this.iterationsTb = new TextBox();
            this.argumentsGb = new GroupBox();
            this.timerGb = new GroupBox();
            this.stopwatchLabel = new Label();
            this.parallelismTb = new TextBox();
            this.parallelismLb = new Label();
            ((ISupportInitialize)(this.mandelbrotPb)).BeginInit();
            this.argumentsGb.SuspendLayout();
            this.timerGb.SuspendLayout();
            this.SuspendLayout();
            // 
            // RenderButton
            // 
            this.RenderButton.BackColor = SystemColors.Control;
            this.RenderButton.Location = new Point(196, 30);
            this.RenderButton.Name = "RenderButton";
            this.RenderButton.Size = new Size(55, 22);
            this.RenderButton.TabIndex = 0;
            this.RenderButton.Text = "Render";
            this.RenderButton.UseVisualStyleBackColor = false;
            this.RenderButton.Click += new System.EventHandler(this.RenderMandelbrot);
            // 
            // mandelbrotPb
            // 
            this.mandelbrotPb.Dock = DockStyle.Fill;
            this.mandelbrotPb.Location = new Point(0, 0);
            this.mandelbrotPb.Name = "mandelbrotPb";
            this.mandelbrotPb.Size = new Size(989, 652);
            this.mandelbrotPb.TabIndex = 20;
            this.mandelbrotPb.TabStop = false;
            this.mandelbrotPb.MouseClick += this.Mandelbrot_MouseClick;
            // 
            // iterationsLb
            // 
            this.iterationsLb.AutoSize = true;
            this.iterationsLb.Location = new Point(10, 16);
            this.iterationsLb.Name = "iterationsLb";
            this.iterationsLb.Size = new Size(50, 13);
            this.iterationsLb.TabIndex = 4;
            this.iterationsLb.Text = "Iterations";
            // 
            // iterationsTb
            // 
            this.iterationsTb.Location = new Point(15, 32);
            this.iterationsTb.Name = "iterationsTb";
            this.iterationsTb.Size = new Size(45, 20);
            this.iterationsTb.TabIndex = 5;
            this.iterationsTb.Text = "1500";
            // 
            // argumentsGb
            // 
            this.argumentsGb.BackColor = SystemColors.Control;
            this.argumentsGb.Controls.Add(this.parallelismLb);
            this.argumentsGb.Controls.Add(this.parallelismTb);
            this.argumentsGb.Controls.Add(this.RenderButton);
            this.argumentsGb.Controls.Add(this.iterationsLb);
            this.argumentsGb.Controls.Add(this.iterationsTb);
            this.argumentsGb.Location = new Point(12, 12);
            this.argumentsGb.Name = "argumentsGb";
            this.argumentsGb.Size = new Size(258, 60);
            this.argumentsGb.TabIndex = 19;
            this.argumentsGb.TabStop = false;
            this.argumentsGb.Text = "Arguments";
            // 
            // timerGb
            // 
            this.timerGb.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Right)));
            this.timerGb.Controls.Add(this.stopwatchLabel);
            this.timerGb.Location = new Point(828, 596);
            this.timerGb.Name = "timerGb";
            this.timerGb.Size = new Size(149, 44);
            this.timerGb.TabIndex = 20;
            this.timerGb.TabStop = false;
            this.timerGb.Text = "Timer";
            // 
            // stopwatchLabel
            // 
            this.stopwatchLabel.AutoSize = true;
            this.stopwatchLabel.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.stopwatchLabel.Location = new Point(9, 16);
            this.stopwatchLabel.Name = "stopwatchLabel";
            this.stopwatchLabel.Size = new Size(0, 20);
            this.stopwatchLabel.TabIndex = 0;
            // 
            // parallelismTb
            // 
            this.parallelismTb.Location = new Point(69, 32);
            this.parallelismTb.Name = "parallelismTb";
            this.parallelismTb.Size = new Size(53, 20);
            this.parallelismTb.TabIndex = 6;
            this.parallelismTb.Text = "8";
            // 
            // parallelismLb
            // 
            this.parallelismLb.AutoSize = true;
            this.parallelismLb.Location = new Point(66, 16);
            this.parallelismLb.Name = "parallelismLb";
            this.parallelismLb.Size = new Size(56, 13);
            this.parallelismLb.TabIndex = 7;
            this.parallelismLb.Text = "Parallelism";
            // 
            // Mandelbrot
            // 
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(989, 652);
            this.Controls.Add(this.timerGb);
            this.Controls.Add(this.argumentsGb);
            this.Controls.Add(this.mandelbrotPb);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Name = "Mandelbrot";
            this.Text = "Mandelbrot";
            this.WindowState = FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            ((ISupportInitialize)(this.mandelbrotPb)).EndInit();
            this.argumentsGb.ResumeLayout(false);
            this.argumentsGb.PerformLayout();
            this.timerGb.ResumeLayout(false);
            this.timerGb.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Button RenderButton;
        private Label iterationsLb;
        private TextBox iterationsTb;
        private GroupBox argumentsGb;
        private GroupBox timerGb;
        private Label stopwatchLabel;
        private PictureBox mandelbrotPb;
        private Label parallelismLb;
        private TextBox parallelismTb;
    }
}

