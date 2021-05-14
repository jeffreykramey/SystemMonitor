
namespace ConfigInit
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.saveButton = new System.Windows.Forms.Button();
            this.niceHashCheckBox = new System.Windows.Forms.CheckBox();
            this.niceHashBrowse = new System.Windows.Forms.Button();
            this.niceHashFilePathLabel = new System.Windows.Forms.Label();
            this.niceHashFilePath = new System.Windows.Forms.TextBox();
            this.csvBrowse = new System.Windows.Forms.Button();
            this.csvFilePathLabel = new System.Windows.Forms.Label();
            this.csvFilePath = new System.Windows.Forms.TextBox();
            this.title = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.logFileBrowse = new System.Windows.Forms.Button();
            this.logFilePathLabel = new System.Windows.Forms.Label();
            this.logFilePath = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(30)))), ((int)(((byte)(54)))));
            this.panel1.Controls.Add(this.saveButton);
            this.panel1.Controls.Add(this.niceHashCheckBox);
            this.panel1.Controls.Add(this.niceHashBrowse);
            this.panel1.Controls.Add(this.niceHashFilePathLabel);
            this.panel1.Controls.Add(this.niceHashFilePath);
            this.panel1.Controls.Add(this.csvBrowse);
            this.panel1.Controls.Add(this.csvFilePathLabel);
            this.panel1.Controls.Add(this.csvFilePath);
            this.panel1.Controls.Add(this.title);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.logFileBrowse);
            this.panel1.Controls.Add(this.logFilePathLabel);
            this.panel1.Controls.Add(this.logFilePath);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 450);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(400, 398);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 14;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // niceHashCheckBox
            // 
            this.niceHashCheckBox.AutoSize = true;
            this.niceHashCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.niceHashCheckBox.ForeColor = System.Drawing.Color.Silver;
            this.niceHashCheckBox.Location = new System.Drawing.Point(3, 210);
            this.niceHashCheckBox.Name = "niceHashCheckBox";
            this.niceHashCheckBox.Size = new System.Drawing.Size(193, 19);
            this.niceHashCheckBox.TabIndex = 13;
            this.niceHashCheckBox.Text = "Automate NiceHash Execution?";
            this.niceHashCheckBox.UseVisualStyleBackColor = false;
            this.niceHashCheckBox.CheckedChanged += new System.EventHandler(this.niceHashCheckBox_CheckedChanged);
            // 
            // niceHashBrowse
            // 
            this.niceHashBrowse.Location = new System.Drawing.Point(641, 241);
            this.niceHashBrowse.Name = "niceHashBrowse";
            this.niceHashBrowse.Size = new System.Drawing.Size(75, 23);
            this.niceHashBrowse.TabIndex = 10;
            this.niceHashBrowse.Text = "Browse";
            this.niceHashBrowse.UseVisualStyleBackColor = true;
            this.niceHashBrowse.Visible = false;
            this.niceHashBrowse.Click += new System.EventHandler(this.niceHashBrowse_Click);
            // 
            // niceHashFilePathLabel
            // 
            this.niceHashFilePathLabel.AutoSize = true;
            this.niceHashFilePathLabel.BackColor = System.Drawing.Color.Transparent;
            this.niceHashFilePathLabel.ForeColor = System.Drawing.Color.Silver;
            this.niceHashFilePathLabel.Location = new System.Drawing.Point(3, 244);
            this.niceHashFilePathLabel.Name = "niceHashFilePathLabel";
            this.niceHashFilePathLabel.Size = new System.Drawing.Size(189, 15);
            this.niceHashFilePathLabel.TabIndex = 9;
            this.niceHashFilePathLabel.Text = "NiceHashQuickMiner.exe File Path";
            this.niceHashFilePathLabel.Visible = false;
            this.niceHashFilePathLabel.Click += new System.EventHandler(this.niceHashFilePathLabel_Click);
            // 
            // niceHashFilePath
            // 
            this.niceHashFilePath.Location = new System.Drawing.Point(212, 241);
            this.niceHashFilePath.Name = "niceHashFilePath";
            this.niceHashFilePath.Size = new System.Drawing.Size(423, 23);
            this.niceHashFilePath.TabIndex = 8;
            this.niceHashFilePath.Visible = false;
            this.niceHashFilePath.TextChanged += new System.EventHandler(this.niceHashFilePath_TextChanged);
            // 
            // csvBrowse
            // 
            this.csvBrowse.Location = new System.Drawing.Point(641, 128);
            this.csvBrowse.Name = "csvBrowse";
            this.csvBrowse.Size = new System.Drawing.Size(75, 23);
            this.csvBrowse.TabIndex = 7;
            this.csvBrowse.Text = "Browse";
            this.csvBrowse.UseVisualStyleBackColor = true;
            this.csvBrowse.Click += new System.EventHandler(this.csvBrowse_Click);
            // 
            // csvFilePathLabel
            // 
            this.csvFilePathLabel.AutoSize = true;
            this.csvFilePathLabel.BackColor = System.Drawing.Color.Transparent;
            this.csvFilePathLabel.ForeColor = System.Drawing.Color.Silver;
            this.csvFilePathLabel.Location = new System.Drawing.Point(3, 131);
            this.csvFilePathLabel.Name = "csvFilePathLabel";
            this.csvFilePathLabel.Size = new System.Drawing.Size(210, 15);
            this.csvFilePathLabel.TabIndex = 6;
            this.csvFilePathLabel.Text = "Open Hardware Monitor .csv Directory";
            this.csvFilePathLabel.Click += new System.EventHandler(this.csvPathLabel_click);
            // 
            // csvFilePath
            // 
            this.csvFilePath.Location = new System.Drawing.Point(212, 128);
            this.csvFilePath.Name = "csvFilePath";
            this.csvFilePath.Size = new System.Drawing.Size(423, 23);
            this.csvFilePath.TabIndex = 5;
            this.csvFilePath.TextChanged += new System.EventHandler(this.csvFilePath_TextChanged);
            // 
            // title
            // 
            this.title.AutoSize = true;
            this.title.BackColor = System.Drawing.Color.Transparent;
            this.title.Dock = System.Windows.Forms.DockStyle.Top;
            this.title.Font = new System.Drawing.Font("Segoe UI", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.title.ForeColor = System.Drawing.Color.Silver;
            this.title.Location = new System.Drawing.Point(0, 0);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(529, 54);
            this.title.TabIndex = 4;
            this.title.Text = "System Logging Initial Setup";
            this.title.Click += new System.EventHandler(this.title_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(-151, -162);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "label1";
            // 
            // logFileBrowse
            // 
            this.logFileBrowse.Location = new System.Drawing.Point(641, 96);
            this.logFileBrowse.Name = "logFileBrowse";
            this.logFileBrowse.Size = new System.Drawing.Size(75, 23);
            this.logFileBrowse.TabIndex = 2;
            this.logFileBrowse.Text = "Browse";
            this.logFileBrowse.UseVisualStyleBackColor = true;
            this.logFileBrowse.Click += new System.EventHandler(this.logFileBrowse_Click);
            // 
            // logFilePathLabel
            // 
            this.logFilePathLabel.AutoSize = true;
            this.logFilePathLabel.BackColor = System.Drawing.Color.Transparent;
            this.logFilePathLabel.ForeColor = System.Drawing.Color.Silver;
            this.logFilePathLabel.Location = new System.Drawing.Point(3, 99);
            this.logFilePathLabel.Name = "logFilePathLabel";
            this.logFilePathLabel.Size = new System.Drawing.Size(140, 15);
            this.logFilePathLabel.TabIndex = 1;
            this.logFilePathLabel.Text = "Log File Output Directory";
            this.logFilePathLabel.Click += new System.EventHandler(this.logFilePathLabel_click);
            // 
            // logFilePath
            // 
            this.logFilePath.Location = new System.Drawing.Point(212, 96);
            this.logFilePath.Name = "logFilePath";
            this.logFilePath.Size = new System.Drawing.Size(423, 23);
            this.logFilePath.TabIndex = 0;
            this.logFilePath.TextChanged += new System.EventHandler(this.logFilePath_TextChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(51)))), ((int)(((byte)(73)))));
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox logFilePath;
        private System.Windows.Forms.Button csvBrowse;
        private System.Windows.Forms.Label csvFilePathLabel;
        private System.Windows.Forms.TextBox csvFilePath;
        private System.Windows.Forms.Label title;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button logFileBrowse;
        private System.Windows.Forms.Button niceHashBrowse;
        private System.Windows.Forms.Label niceHashFilePathLabel;
        private System.Windows.Forms.TextBox niceHashFilePath;
        private System.Windows.Forms.TextBox i;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox niceHashCheckBox;
        private System.Windows.Forms.Label logFilePathLabel;
        private System.Windows.Forms.Button saveButton;
    }
}

