﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Deff83Timer
{
    public partial class FormAddProg : Form
    {
        public FormAddProg()
        {
            InitializeComponent();
        }

        private void FormAddProg_Load(object sender, EventArgs e)
        {
            if (textBoxName.CanSelect) textBoxName.Select();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
