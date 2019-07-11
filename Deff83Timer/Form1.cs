using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Deff83Timer
{
    public partial class Form1 : Form
    {
        private List<TextBox> textboxs = new List<TextBox>();
        public RegistryKey myProgramm { get;  set; }
        public RegistryKey currentUser { get;  set; }
        private string registrPath = @"Software\MyProgrammDeff83";
        private string path, lofFile, pathbalanceFile;
        private ClassBotTimer classBotTimer;
        private DateTime startProgDeffTimer;
        //импорт цветной прогресс бар
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        public Form1()
        {
            InitializeComponent();
            currentUser = Registry.CurrentUser;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            initial();
        }

        private void initial()
        {
            
            using (myProgramm = currentUser.CreateSubKey(registrPath))
            {
                path = (string)myProgramm.GetValue("pathConfigTimer");
                lofFile = (string)myProgramm.GetValue("pathLogTimer");
                pathbalanceFile = (string)myProgramm.GetValue("pathBalanceFile");
            };
            if (lofFile != null)
                textBoxLog.Text = lofFile;
            if (pathbalanceFile != null)
                textBoxBalance.Text = pathbalanceFile;
            classBotTimer = new ClassBotTimer(this, lofFile);

            if (path != null)
            {
                textBoxPath.Text = path;
                filereaderConfig(path);
            }
            

        }

        private List<Button> buttons = new List<Button>();
        private List<Button> checks = new List<Button>();

        private List<ProgressBar> progressbarss = new List<ProgressBar>();
        private List<TextBox> textboxess = new List<TextBox>(); //Баланс
        private List<TextBox> textboxessName = new List<TextBox>(); 

        private void filereaderConfig(string path)
        {
            
            using (StreamReader filestream = new StreamReader(path))
            {
                string line;
                while((line = filestream.ReadLine()) != null)
                {
                    string[] commandsString = line.Split('|');
                    string name_command = commandsString[0];
                    string pathProg_command = (commandsString.Length > 1) ? commandsString[1] : "";
                    string attrib_command = (commandsString.Length > 2) ? commandsString[2] : "";
                    int startTime_command = (commandsString.Length > 3) ? Int32.Parse(commandsString[3]) : 0;
                    int delayTime_command = (commandsString.Length > 4) ? Int32.Parse(commandsString[4]) : 0;

                    FlowLayoutPanel flow = new FlowLayoutPanel();
                    flow.FlowDirection = FlowDirection.LeftToRight;
                    flow.Width = 730;
                    flow.Height = 27;
                    TextBox textbox = new TextBox();    //Баланс
                    textbox.Text = "";
                    textbox.Width = 200;
                    // labelm.Height = 27;
                    textbox.BackColor = Color.FromArgb(240, 230, 140);
                    textbox.Margin = new Padding(1);
                    textbox.Font = new Font("Times New Roman", 13, FontStyle.Regular);

                    TextBox textboxName = new TextBox();
                    textboxName.Text = name_command;
                    textboxName.Width = 200;
                    // labelm.Height = 27;
                    textboxName.BackColor = Color.FromArgb(240, 230, 140);
                    textboxName.Margin = new Padding(1);
                    textboxName.Font = new Font("Times New Roman", 13, FontStyle.Regular);

                    Button button = new Button();
                    button.Margin = new Padding(0);
                    button.Height = 27;
                    button.Width = 100;
                    button.Text = "параметры";
                    button.Font = new Font("Times New Roman", 13, FontStyle.Regular);
                    button.Name = buttons.Count +"";
                    button.Click += Button_Click;


                    ProgressBar progressbar = new ProgressBar();
                    progressbar.Text = "";
                    progressbar.Width = 170;
                    // labelm.Height = 27;
                    progressbar.BackColor = Color.FromArgb(240, 230, 140);
                    progressbar.Margin = new Padding(1);
                    progressbar.Font = new Font("Times New Roman", 13, FontStyle.Regular);
                    SendMessage(progressbar.Handle, 0x410, 3, 0);//yellow

                    Button buttonOffOn = new Button();
                    buttonOffOn.Width = 50;
                    buttonOffOn.Margin = new Padding(0,0,0,0);
                    buttonOffOn.Text = "WAIT";
                    buttonOffOn.BackColor = Color.Yellow;
                    buttonOffOn.Click += ButtonOffOn_Click;
                    buttonOffOn.Name = buttons.Count + " onoff";

                    flow.Controls.AddRange(new Control[]{ textboxName, textbox, button, progressbar, buttonOffOn });
                    flowLayoutPanel1.Controls.Add(flow);
                    buttons.Add(button);
                    Programm pro = new Programm(name_command, pathProg_command, attrib_command, startTime_command, delayTime_command);
                    classBotTimer.listProgramm.Add(pro);
                    progressbarss.Add(progressbar);
                    checks.Add(buttonOffOn);
                    textboxess.Add(textbox);
                    textboxessName.Add(textboxName);

                }
            }
            classBotTimer.isBotWork = true;
            goBotTimer();
            startProgDeffTimer = DateTime.Now;
            timer1.Start();
        }

        private void ButtonOffOn_Click(object sender, EventArgs e)
        {
           if( ((Button)sender).Text == "OFF")
            {
                int number_button = Int32.Parse((((Button)sender).Name).Split(' ')[0]);
                classBotTimer.listProgramm[number_button].isStarting = true;
                ((Button)sender).Text = "WAIT";
                ((Button)sender).BackColor = Color.Yellow;
                SendMessage(progressbarss[number_button].Handle, 0x410, 3, 0);//yellow
                textboxessName[number_button].BackColor = Color.FromArgb(240, 230, 140);    //yellow
                textboxess[number_button].BackColor = Color.FromArgb(240, 230, 140);   //yellow
                int startPro = classBotTimer.listProgramm[number_button].timestart * 1000 * 60;//промежуток времени когда запустится программа
                int now = (int)((TimeSpan)(DateTime.Now - startProgDeffTimer)).TotalMilliseconds;//промежуток от старта программы до сейчас

                int minus = startPro - now;
                if (minus <= 0)
                {
                    classBotTimer.listProgramm[number_button].timestart = classBotTimer.tek_time + 1;
                }
            }
            else if (((Button)sender).Text == "WAIT" || ((Button)sender).Text == "ON")
            {
                int number_button = Int32.Parse((((Button)sender).Name).Split(' ')[0]);
                classBotTimer.listProgramm[number_button].isStarting = false;
                ((Button)sender).Text = "OFF";
                ((Button)sender).BackColor = Color.Red;
                SendMessage(progressbarss[number_button].Handle, 0x410, 2, 0);//red
                textboxessName[number_button].BackColor = Color.FromArgb(220, 220, 220);   //grey
                textboxess[number_button].BackColor = Color.FromArgb(220, 220, 220);    //grey
            }
        }

        public async void goBotTimer()
        {
            await Task.Run(() =>
            {
                classBotTimer.timerBot();
            });
        }

        private void Button_Click(object sender, EventArgs e)
        {
            new FormParametr(path, Int32.Parse(((Button)sender).Name)).Show();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            textBoxPath.Text = ((OpenFileDialog)sender).FileName;
            using (myProgramm = currentUser.OpenSubKey(registrPath, true))
                myProgramm.SetValue("pathConfigTimer", textBoxPath.Text);
            filereaderConfig(textBoxPath.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void показатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
            }
        }

        private Process proccesscat;
        bool isCat = false;
        private string pathCat = @"C:\Users\User\Desktop\wallpaper.bat";

        private void timer1_Tick(object sender, EventArgs e)
        {
            int countProg = 0;
            for (int i=0; i<progressbarss.Count; i++)
            {
                Programm pro = classBotTimer.listProgramm[i];
                int startPro = pro.timestart*1000*60;//промежуток времени когда запустится программа
                int now = (int) ((TimeSpan) (DateTime.Now - startProgDeffTimer)).TotalMilliseconds;//промежуток от старта программы до сейчас

                int minus = startPro - now ;
                if (minus < 0) minus = 0;

                progressbarss[i].Text = "осталось "+minus+"секунд";
                if (pro.timerDelay*1000*60 < minus) {
                    progressbarss[i].Maximum = startPro;
                    progressbarss[i].Value = minus;
                } else
                {
                    progressbarss[i].Maximum = pro.timerDelay*1000*60;
                    progressbarss[i].Value = minus;
                }
                int hource = (int)(minus / (1000*60*60));
                int minut = (int)((minus - (hource * 1000 * 60 * 60)) / (1000 * 60));
                int sec = (int)((minus - (hource * 1000 * 60 * 60) - (minut * 1000 * 60)) / 1000);
                buttons[i].Text = hource + ":" + minut.ToString("00") + ":" + sec.ToString("00");
                if (classBotTimer.processis.ContainsKey(pro.name))
                {
                    buttons[i].BackColor = Color.FromArgb(124, 252, 0); //lite green
                                                                        // textboxessName[i].BackColor = Color.FromArgb(152, 251, 152);    //green
                                                                        // textboxess[i].BackColor = Color.FromArgb(152, 251, 152);   //green
                                                                        // SendMessage(progressbarss[i].Handle, 0x410, 1, 0);//green
                                                                        //  checks[i].BackColor = Color.Green;
                                                                        //  checks[i].Text = "ON";
                    countProg++;
                      Process proc = classBotTimer.processis[pro.name];
                    if (proc.HasExited) {
                        classBotTimer.processis.Remove(pro.name);
                        doexitCode(proc.ExitCode, i);
                    }
                }
                else {
                    buttons[i].BackColor = Color.White; //white
                    
                    
                    
                }
            }
            if(countProg > 0 && !isCat)
            {
                //запустить кота
                startCat();
                
                isCat = true;
            }
            if (countProg == 0 && isCat)
            {
                //остановить кота
                Process[] proc = Process.GetProcesses();
                foreach (Process process in proc)
                    if (process.ProcessName == "mpv")
                    {
                        process.Kill();
                    }
                
                isCat = false;
            }
        }

        private void doexitCode(int exitCode, int numberProg)
        {
            switch (exitCode)
            {
                case 0:
                    try
                    {
                        using(StreamReader sr = new StreamReader(pathbalanceFile))
                        {
                            string text = gotoline(numberProg, sr);
                            textboxess[numberProg].Text = text;
                        }
                    }
                    catch (Exception e) { MessageBox.Show(e.Message); }
                    break;
                case 2: //страница не загрузилась (2 попытки webGO)
                    int now = (int)((TimeSpan)(DateTime.Now - startProgDeffTimer)).TotalMinutes;    //времяя 60*1000
                    classBotTimer.listProgramm[numberProg].timestart = now + 1;
                    break;
                case 3: //ошибка исполнения
                    int now3 = (int)((TimeSpan)(DateTime.Now - startProgDeffTimer)).TotalMinutes;    //времяя 60*1000
                    classBotTimer.listProgramm[numberProg].timestart = now3 + 1;
                    break;
            }
        }

        private async void startCat()
        {
            await Task.Run(() =>
            {
                Thread.Sleep(10000);
                proccesscat = Process.Start(pathCat);
            });
        }

        private void buttonlog_Click(object sender, EventArgs e)
        {
            openFileDialogLog.ShowDialog();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (((Button)sender).Text == "OFF")
            {
                for (int i = 0; i < checks.Count; i++)
                     ButtonOffOn_Click((Button)checks[i], null);
                ((Button)sender).Text = "ON";
                ((Button)sender).BackColor = Color.Lime;
               
            }
            else if (((Button)sender).Text == "ON")
            {
                for (int i = 0; i < checks.Count; i++)
                {
                    if (((Button)checks[i]).Text == "WAIT" || ((Button)checks[i]).Text == "ON") ButtonOffOn_Click((Button)checks[i], null);
                }
                ((Button)sender).Text = "OFF";
                ((Button)sender).BackColor = Color.Red;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            new FormAddProg().Show();
        }

        private void buttonBalance_Click(object sender, EventArgs e)
        {
            openFileDialogBalance.ShowDialog();
        }

        private void openFileDialogBalance_FileOk(object sender, CancelEventArgs e)
        {
            textBoxBalance.Text = ((OpenFileDialog)sender).FileName;
            using (myProgramm = currentUser.OpenSubKey(registrPath, true))
                myProgramm.SetValue("pathBalanceFile", textBoxBalance.Text);
            pathbalanceFile = textBoxBalance.Text;
        }

        private void openFileDialogLog_FileOk(object sender, CancelEventArgs e)
        {
            textBoxLog.Text = ((OpenFileDialog)sender).FileName;
            using (myProgramm = currentUser.OpenSubKey(registrPath, true))
                myProgramm.SetValue("pathLogTimer", textBoxLog.Text);
            classBotTimer.logFile = textBoxLog.Text;
        }
        private string gotoline(int line, StreamReader stream)
        {
            string linetext = "";
            stream.DiscardBufferedData();
            stream.BaseStream.Seek(0, SeekOrigin.Begin);
            for (int pos = 0; pos < line+1; pos++) linetext = stream.ReadLine();
            return linetext;
        }
    }
}
