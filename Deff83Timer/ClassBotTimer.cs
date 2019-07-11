using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Deff83Timer
{
    class ClassBotTimer
    {
        public Dictionary<string, Process> processis = new Dictionary<string, Process>();
        public List<Programm> listProgramm = new List<Programm>();
        public bool isBotWork = false;
        private Form1 form1;
        private int delay = 1; //промежуток через который делать обход
        public int tek_time;
        public string logFile;
        enum TypeLog{
            Wrong = 1,
            Error = 2,
            Info = 3
            
        }

        public ClassBotTimer(Form1 form1, string logFile)
        {
            this.form1 = form1;
            this.logFile = logFile;
        }
        public void timerBot()
        {
            tek_time = 0; //количество в минутах
            while (isBotWork)
            {
                foreach (Programm itemProgramm in listProgramm) //обход программ
                {
                    
                        if (itemProgramm.timestart <= tek_time)    //(itemProgramm.timestart*6 <= tek_time)
                    {
                        //MessageBox.Show(itemProgramm.name);
                        
                            cmdStart(itemProgramm);
                            itemProgramm.timestart += itemProgramm.timerDelay;
                            break;
                        
                        
                    }
                    if (!itemProgramm.isStarting)
                    {
                        itemProgramm.timestart += delay;
                    }
                }
                tek_time += delay;
                
                Thread.Sleep(1000*60*delay);    //1мин
                
            }
        }
        private async void cmdStart(Programm pr)
        {
            await Task.Run(() =>
            {
                try
                {
                    Process proccess = Process.Start(pr.pathProg, pr.attributs);
                    if (processis.ContainsKey(pr.name)) { }
                    else
                    {
                        
                        processis.Add(pr.name, proccess);
                        
                    }
                    setLog(TypeLog.Info, "запущена программа " + pr.name + " " + pr.pathProg + " с параметрами " + pr.attributs);

                }
                catch (Exception e) { setLog(TypeLog.Info, "запущена программа " + pr.name + " ошибка " + e.Message); }
               
            });
        }
        private void setLog(TypeLog type, string log)
        {
            if (logFile != null)
            {
                using(StreamWriter sWriter = new StreamWriter(logFile, true))
                {
                    sWriter.WriteLine("[" + DateTime.Now +"]" + "[" + type.ToString() + "] " + log);
                }
            }
        }
    }
    class Programm{
        public string name { get; set; }
        public string pathProg { get; set; }
        public string attributs { get; set; }
        public int timestart { get; set; }
        public int timerDelay { get; set; }
        public bool isStarting { get; set; } = true;
        public Programm(string name, string pathProg, string attributs, int timestart, int timerDelay)
        {
            this.name = name;
            this.pathProg = pathProg;
            this.attributs = attributs;
            this.timestart = timestart;
            this.timerDelay = timerDelay;
        }

    }
}
