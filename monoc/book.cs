using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;

namespace monoc
{
    public partial class book : Form
    {
        public book()
        {
            InitializeComponent();
        }

        private void book_Load(object sender, System.EventArgs e)
        {
            string[] files = Directory.GetFiles("pages");
            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                TreeNode tds = treeView1.Nodes.Add(fi.Name);
                tds.Tag = fi.FullName;
                //tds.StateImageIndex = 1;
            }
        }
    }
}