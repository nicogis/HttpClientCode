using System;
using System.Windows.Forms;

namespace HttpClientCode
{
    public partial class FrmGenerateHttpClientCode : Form
    {
        private string MESSAGEBOXTEXT = "HttpClientCode";

        public FrmGenerateHttpClientCode()
        {
            InitializeComponent();
        }

        public void SetText(string text)
        {
            this.rtbCode.Text = text;
        }
        


        private void btnCopyToClipBoard_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(rtbCode.Text);
            MessageBox.Show("Text Copied to ClipBoard", this.MESSAGEBOXTEXT, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://nicogis.blogspot.com");
        }
    }
}
