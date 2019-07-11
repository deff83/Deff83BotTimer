using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Deff83Timer
{
    public partial class FormParametr : Form
    {
        private string path;
        public FormParametr(string path, int i)
        {
            InitializeComponent();
            this.path = path;
            inity(i);
        }

        private void inity(int j)
        {
            using (StreamReader filestream = new StreamReader(path))
            {
                string line;
                int i = 0;
                while ((line = filestream.ReadLine()) != null) { i++; if (i == j+1) break; }
                string[] commandsString = line.Split('|');
                textBox1.Text = commandsString[0];
                textBox2.Text = (commandsString.Length > 1) ? commandsString[1]:"";
                textBox3.Text = (commandsString.Length > 2) ? commandsString[2] : "";
                textBox4.Text = (commandsString.Length > 3) ? commandsString[3] : "";
                textBox5.Text = (commandsString.Length > 4) ? commandsString[4] : "";

            }
        }

    }
}
