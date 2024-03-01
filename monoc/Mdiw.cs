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
using Microsoft.VisualBasic;
using monoc;

namespace monoc
{
    public partial class Mdiw : Form
    {
        public static Lua lua = new Lua();
        bool plugins = true;
        public List<string> ScriptObjects = new List<string> ();
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
            luaBridge.Init();
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
            //ScriptObjects.Add(toolStripTextBox1.Text);
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

        private void disambiguationPanelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string value = Interaction.InputBox("Please enter your code:", "Disambiguation Client", "");
            List<string> keywordsAndObjects = new List<string>();

            // Add keywords
            foreach (string keyword in scriptwrite.keywords)
            {
                string modKeyword = keyword.Replace("\\b(", "").Replace(")\\b", "");
                keywordsAndObjects.Add(modKeyword);
            }

            // Add objects
            foreach (string obj in scriptwrite.objects)
            {
                string modObj = obj.Replace("\\b(", "").Replace(")\\b", "");
                keywordsAndObjects.Add(modObj);
            }

            string closestMatch = FindClosestMatch(value, keywordsAndObjects);
            if (closestMatch.Length > 0)
            {
                DialogResult result = MessageBox.Show("The closest match was: " + closestMatch + "\nCopy to clipboard?\n\nYES = Copy to clipboard\nNO = Show next result\nCANCEL = Neither", "Disambiguation Client", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                {
                    Clipboard.SetText(closestMatch);
                }
                else if (result == DialogResult.No)
                {
                    // Handle showing next result
                }
                else
                {
                    // Handle other actions
                }
            }
            else
            {
                MessageBox.Show("Could not find a close match... somehow...", "Disambiguation Client: Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static string FindClosestMatch(string searchTerm, List<string> strings)
        {
            string closestMatch = null;
            int minDistance = int.MaxValue;

            foreach (string str in strings)
            {
                int distance = ComputeLevenshteinDistance(searchTerm, str);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestMatch = str;
                }
            }

            return closestMatch;
        }

        static int ComputeLevenshteinDistance(string s, string t)
        {
            int[,] d = new int[s.Length + 1, t.Length + 1];

            for (int i = 0; i <= s.Length; i++)
                d[i, 0] = i;

            for (int j = 0; j <= t.Length; j++)
                d[0, j] = j;

            for (int j = 1; j <= t.Length; j++)
            {
                for (int i = 1; i <= s.Length; i++)
                {
                    int cost = (s[i - 1] == t[j - 1]) ? 0 : 1;

                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }

            return d[s.Length, t.Length];
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void addToListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            scriptwrite.keywords.Add("\\b(" + toolStripTextBox1.Text + ")\\b");
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.ShowDialog();
            File.WriteAllLines(sfd.FileName, scriptwrite.keywords);
        }
        static List<string> GetTypeNamesFromDll(string dllPath)
        {
            List<string> typeNames = new List<string>();

            try
            {
                Assembly assembly = Assembly.LoadFrom(dllPath);
                Type[] types = assembly.GetTypes();

                foreach (Type type in types)
                {
                    typeNames.Add(type.FullName);
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                Console.WriteLine("Error loading types from the DLL:");
                foreach (Exception loaderException in ex.LoaderExceptions)
                {
                    Console.WriteLine(loaderException.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return typeNames;
        }

        private void addReferenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select the dynamic link library...";
            ofd.ShowDialog();
            List<string> list = GetTypeNamesFromDll(ofd.FileName);
            foreach (string s in list)
            {
                Console.WriteLine(s);
                scriptwrite.objects.Add(s);
            }
        }

        private void luaConsoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string com = Interaction.InputBox("Lua Command:", "LuaConsole", "monoc:");
            lua.DoString(com);
        }

    }
    public class LuaBridge
    {
        public static Form mainForm;
        public string CurrentDocumentText;
        Timer timer = new Timer();
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
            foreach (Control tb in mainForm.ActiveMdiChild.Controls)
            {
                if (tb is RichTextBox richTextBox)
                {
                    richTextBox.AppendText("\n" + line + "\n");
                }
            }
        }
        public void disambiguate(string value)
        {
            List<string> keywordsAndObjects = new List<string>();

            // Add keywords
            foreach (string keyword in scriptwrite.keywords)
            {
                string modKeyword = keyword.Replace("\\b(", "").Replace(")\\b", "");
                keywordsAndObjects.Add(modKeyword);
            }

            // Add objects
            foreach (string obj in scriptwrite.objects)
            {
                string modObj = obj.Replace("\\b(", "").Replace(")\\b", "");
                keywordsAndObjects.Add(modObj);
            }

            string closestMatch = Mdiw.FindClosestMatch(value, keywordsAndObjects);
            if (closestMatch.Length > 0)
            {
                DialogResult result = MessageBox.Show("The closest match was: " + closestMatch + "\nCopy to clipboard?\n\nYES = Copy to clipboard\nNO = Show next result\nCANCEL = Neither", "Disambiguation Client", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                {
                    Clipboard.SetText(closestMatch);
                }
                else if (result == DialogResult.No)
                {
                    // Handle showing next result
                }
                else
                {
                    // Handle other actions
                }
            }
            else
            {
                MessageBox.Show("Could not find a close match... somehow...", "Disambiguation Client: Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        public void setlang(string lang)
        {
            scriptwrite.keywords.Clear();
            string[] keywrds = File.ReadAllLines(Application.StartupPath + "/lang/" + lang + ".LangFile");
            foreach (string keywrd in keywrds)
            {
                scriptwrite.keywords.Add(keywrd);
            }
        }
    }
}
    