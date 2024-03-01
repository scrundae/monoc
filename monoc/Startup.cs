using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace monoc
{
    public partial class Startup : Form
    {
        public Startup()
        {
            InitializeComponent();
            //scriptwrite.keywords = new List<string>() { @"\bEgg\b" };
        }

        private void monocT_Click(object sender, EventArgs e)
        {

        }

        private void ctw_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Close();
        }

        private void Startup_Load(object sender, EventArgs e)
        {
            webBrowser1.CoreWebView2InitializationCompleted += WebBrowser1_CoreWebView2InitializationCompleted;
            webBrowser1.Source = new Uri("file:///" + Application.StartupPath + "/startup-page.html");
        }

        private void WebBrowser1_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                webBrowser1.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            }
            else
            {
                // Handle initialization failure if necessary
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (webBrowser1.Source == new Uri("file:///" + Application.StartupPath + "/ctw"))
            {
                this.Close();
            }
        }
    }
}
