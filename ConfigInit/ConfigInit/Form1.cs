using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace ConfigInit
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
           // string homeDir = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\"));
            //string homeDir = Path.GetFullPath(Environment.CurrentDirectory);
            string homeDir = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), @"..\");

            logFilePath.Text = homeDir;
            csvFilePath.Text = Path.Combine(homeDir, @"OpenHardwareMonitor");
           

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void title_Click(object sender, EventArgs e)
        {

        }

        private void logFilePath_TextChanged(object sender, EventArgs e)
        {

        }

        private void logFilePathLabel_click(object sender, EventArgs e)
        {

        }
        private void logFileBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                logFilePath.Text = dialog.SelectedPath;
            }

        }

        private void csvPathLabel_click(object sender, EventArgs e)
        {

        }

        private void csvFilePath_TextChanged(object sender, EventArgs e)
        {

        }

        private void csvBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                csvFilePath.Text = dialog.SelectedPath;
            }
        }

        private void niceHashCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            niceHashFilePath.Visible = niceHashCheckBox.Checked;
            niceHashFilePathLabel.Visible = niceHashCheckBox.Checked;
            niceHashBrowse.Visible = niceHashCheckBox.Checked;
        }
        private void niceHashFilePathLabel_Click(object sender, EventArgs e)
        {

        }

        private void niceHashFilePath_TextChanged(object sender, EventArgs e)
        {

        }


        private void niceHashBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                niceHashFilePath.Text = dialog.FileName;
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            writeToLog();
            Debug.WriteLine(logFilePath.Text);
            this.Close();

        }


        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            
        }

        public void writeToLog()
        {
            //string filePath = logFilePath.Text + @"\config.txt";
            string filePath = Path.Combine(Environment.CurrentDirectory, "config.txt");
            using (StreamWriter writer = File.AppendText(filePath))
            {
                writer.WriteLine("logFilePath={0}", logFilePath.Text);

                Debug.WriteLine(logFilePath.Text);
                writer.WriteLine("csvFilePath={0}", csvFilePath.Text);
                if (niceHashCheckBox.Checked)
                {
                    writer.WriteLine("niceHashFilePath={0}", niceHashFilePath.Text);
                }
            }
        }
    }
}
    