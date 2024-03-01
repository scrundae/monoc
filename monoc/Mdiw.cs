using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using NLua;

namespace monoc
{
    public partial class Mdiw : Form
    {
        public static Lua lua = new Lua();
        bool plugins = true;
        public Mdiw(string[] args)
        {
           
            Console.WriteLine("Booting into MONOC!");
            InitializeComponent();
            foreach (string arg in args)
            {
                if (arg == "-plugins")
                {
                    plugins = false;
                }
                else if (arg == "+fullscreen") // Changed from "else" to "else if"
                {
                    this.WindowState = FormWindowState.Normal;
                    this.FormBorderStyle = FormBorderStyle.None;
                    this.TopMost = true;
                    this.WindowState = FormWindowState.Maximized;
                }
                // No action taken for other arguments
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            lua.RegisterFunction("makemodal", null, typeof(MessageBox).GetMethod("Show", new Type[] { typeof(string), typeof(string) }));
            //lua.RegisterFunction("println", null, typeof(LuaBridge).GetMethod("println"));
            LuaBridge luaBridge = new LuaBridge();
            LuaBridge.mainForm = this;
            //luaBridge.Init();
            lua["monoc"] = luaBridge;
            Startup stu = new Startup();
            stu.MdiParent = this;
            stu.WindowState = FormWindowState.Maximized;
            stu.Show();

            DirectoryInfo d = new DirectoryInfo("mods/menubar");
            if (plugins)
            {
                foreach (FileInfo f in d.GetFiles("*.lua"))
                {
                    ToolStripMenuItem tsmi = new ToolStripMenuItem();
                    tsmi.Text = f.Name;
                    tsmi.Click += (s, ev) => ModLoadFromMenuBar(s, ev, f.FullName);
                    tsmi.Image = Properties.Resources.plugin;
                    modsToolStripMenuItem.DropDownItems.Add(tsmi);
                }
            }
            else
            {
                ToolStripMenuItem tsmi = new ToolStripMenuItem();
                tsmi.Text = "Plugins are disabled.";
                modsToolStripMenuItem.DropDownItems.Add(tsmi);
            }
        }

        private void ModLoadFromMenuBar(object sender, EventArgs e, string pathToLoad)
        {
            
            try
            {
                lua.DoFile(pathToLoad);
            }
            catch(Exception ex)
            {
                MessageBox.Show(":-(\n\nThe Lua Compiler has experienced an error\n\nMore details:\n\n" + ex.Message, "Lua Error");
            }
        }


        private void startupWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Startup stu = new Startup();
            stu.MdiParent = this;
            stu.WindowState = FormWindowState.Maximized;
            stu.Show();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            scriptwrite sw = new scriptwrite();
            sw.MdiParent = this;
            sw.WindowState = FormWindowState.Maximized;
            sw.Show();
        }

        private void fullscreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.WindowState = FormWindowState.Maximized;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About abt = new About();
            abt.ShowDialog();
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //lua.DoString(toolStripTextBox1.Text);
        }

        private void runCurrentDocumentsTextAsLuaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Control ctrl in this.ActiveMdiChild.Controls)
            {
                if(ctrl is RichTextBox rtb)
                {
                    foreach (string str in rtb.Lines)
                    {
                        lua.DoString(str);
                    }
                }
            }
        }

        private void windowedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.WindowState = FormWindowState.Normal;
            this.TopMost = false;
        }

        private void removeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            scriptwrite.keywords.Clear();
        }

        private void addToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            
        }

        private void setToolStripMenuItem_Click(object sender, EventArgs e)
        {
            scriptwrite.keywords.Clear();
            string[] keywrds = File.ReadAllLines(Application.StartupPath + "/lang/" + toolStripTextBox3.Text + ".LangFile");
            foreach (string keywrd in keywrds)
            {
                scriptwrite.keywords.Add(keywrd);
            }
        }

        private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrintDialog pd = new PrintDialog();
            pd.ShowDialog();
        }
    }
    public class LuaBridge
    {
        public static Form mainForm;
        public string CurrentDocumentText;
        Timer timer;
        public void Init()
        {
            timer.Enabled = true;
            timer.Interval = 1;
            timer.Start();
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            foreach (Control control in mainForm.ActiveMdiChild.Controls)
            {
                if (control is RichTextBox rtf)
                {
                    CurrentDocumentText = rtf.Text;
                }
            }
        }

        public void println(string line)
        {
            foreach(Control tb in mainForm.ActiveMdiChild.Controls)
            {
                if (tb is RichTextBox richTextBox)
                {
                    richTextBox.AppendText("\n" + line + "\n");
                }
            }
        }
        public void print(string line)
        {
            foreach (Control tb in mainForm.ActiveMdiChild.Controls)
            {
                if (tb is RichTextBox richTextBox)
                {
                    richTextBox.AppendText(line);
                }
            }
        }
        public void clear()
        {
            foreach (Control tb in mainForm.ActiveMdiChild.Controls)
            {
                if (tb is RichTextBox richTextBox)
                {
                    richTextBox.Text = "";
                }
            }
        }
    }
}
