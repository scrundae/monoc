using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using NLua;

namespace monoc
{
    public partial class scriptwrite : Form
    {
        //Dont even know how tf this worked but it did
        public static List<string> keywords = new List<string> { };
        public static List<string> objects = new List<string> { };
        public scriptwrite()
        {
            InitializeComponent();
        }

        private void codeRichTextBox_TextChanged(object sender, EventArgs e)
        {
            

            // getting comments (inline or multiline)
            string comments = @"(//|--|#)" + ".+?$";
            MatchCollection commentMatches = Regex.Matches(codeRichTextBox.Text, comments, RegexOptions.Multiline);

            // getting strings
            string strings = "\".+?\"";
            MatchCollection stringMatches = Regex.Matches(codeRichTextBox.Text, strings);

            // saving the original caret position + forecolor
            int originalIndex = codeRichTextBox.SelectionStart;
            int originalLength = codeRichTextBox.SelectionLength;
            Color originalColor = Color.Black;

            // MANDATORY - focuses a label before highlighting (avoids blinking)
            TitleLabel.Focus();

            // removes any previous highlighting (so modified words won't remain highlighted)
            codeRichTextBox.SelectionStart = 0;
            codeRichTextBox.SelectionLength = codeRichTextBox.Text.Length;
            codeRichTextBox.SelectionColor = originalColor;

            // scanning keywords...
            foreach (string keyword in keywords)
            {
                MatchCollection keywordMatches = Regex.Matches(codeRichTextBox.Text, keyword);
                foreach (Match m in keywordMatches)
                {
                    codeRichTextBox.SelectionStart = m.Index;
                    codeRichTextBox.SelectionLength = m.Length;
                    codeRichTextBox.SelectionColor = Color.Blue;
                }
            }

            foreach (Match m in stringMatches)
            {
                codeRichTextBox.SelectionStart = m.Index;
                codeRichTextBox.SelectionLength = m.Length;
                codeRichTextBox.SelectionColor = Color.Brown;
            }

            foreach (Match m in commentMatches)
            {
                codeRichTextBox.SelectionStart = m.Index;
                codeRichTextBox.SelectionLength = m.Length;
                codeRichTextBox.SelectionColor = Color.Green;
            }

            

            // restoring the original colors, for further writing
            codeRichTextBox.SelectionStart = originalIndex;
            codeRichTextBox.SelectionLength = originalLength;
            codeRichTextBox.SelectionColor = originalColor;

            // giving back the focus
            codeRichTextBox.Focus();
        }


        private void scriptwrite_Load(object sender, EventArgs e)
        {
            
        }
    }
}
