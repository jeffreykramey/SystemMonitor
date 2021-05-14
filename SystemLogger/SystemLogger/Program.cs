﻿using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Management;
using Microsoft.VisualBasic.FileIO;
using System.Diagnostics;
using System.Globalization;

namespace SystemLogger
{
    class Program
    {
        static HashSet<String> appsToWatch = new HashSet<string>(File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, @"appsToWatch.txt")));
        static Dictionary<string, TargetProcess> runningApps = new Dictionary<string, TargetProcess>();
        static String csvFileDir, logFilesPath, niceHashFilePath = null;

        static void Main(string[] args)
        {
            NotifyIcon tray = new NotifyIcon();
            tray.Icon = new System.Drawing.Icon(Path.Combine(Environment.CurrentDirectory, @"pussyCat.ico"));
            
            tray.Visible = true;
            tray.BalloonTipText = "meow";
            tray.ShowBalloonTip(1);

            Directory.CreateDirectory("LogFiles"); //make a logFiles dir if it doesn't already exist
            string configFilePath = Path.Combine(Environment.CurrentDirectory, @"LogFiles\config.txt");
            if (!File.Exists(configFilePath))
            {
                var formProcess = Process.Start(Path.Combine(Environment.CurrentDirectory, @"ConfigInit\ConfigInit.exe"));
                formProcess.WaitForExit();
            }
            parseFilePaths(configFilePath);
            startOpenHwMonitor();
            startMiner();
            
            ManagementScope scope = new ManagementScope(Environment.MachineName + @"\root\cimv2"); 
            scope.Options.EnablePrivileges = true;
            try
            {
                ManagementEventWatcher startWatch = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
                startWatch.EventArrived += new EventArrivedEventHandler(startWatchingApp);
                startWatch.Start();
                ManagementEventWatcher stopWatch = new ManagementEventWatcher(
                new WqlEventQuery("SELECT * FROM Win32_ProcessStopTrace"));
                stopWatch.EventArrived += new EventArrivedEventHandler(stopWatchingApp);
                stopWatch.Start();
                Console.WriteLine("Press any key to exit");
                while (!Console.KeyAvailable) System.Threading.Thread.Sleep(50);
                startWatch.Stop();
                stopWatch.Stop();
            }
            catch (ManagementException e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
            }
        }

        static void startWatchingApp(object sender, EventArrivedEventArgs e)
        {
            String processName = (String)e.NewEvent.Properties["ProcessName"].Value;
            if (appsToWatch.Contains(processName))
            {
                if (!runningApps.ContainsKey(processName))
                {
                    runningApps.Add(processName, new TargetProcess());
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Found Target Process: {0}", processName);
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    runningApps[processName].incrementInstanceCount();
                }
                killMiner();
            }
            else
            {
                Console.WriteLine("Process started: {0}", processName);
            }
            
            

        }

        static void stopWatchingApp(object sender, EventArrivedEventArgs e)
        {
            String processName = (String)e.NewEvent.Properties["ProcessName"].Value;
            if(runningApps.ContainsKey(processName))
            {
                TargetProcess tp = runningApps[processName];
                tp.decrementInstanceCount();
                if (tp.getInstanceCount() <= 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Killing Target Process: {0}", processName);
                    Console.ForegroundColor = ConsoleColor.White;
                    
                    runningApps.Remove(processName);
                    tp.setEndTime();
                    int numDaysLogged = tp.getEndTime().DayOfYear - tp.getStartTime().DayOfYear; //covers cases where monitoring covers multi-day periods    
                    for (int i = 0; i <= numDaysLogged; i++)
                    {
                        string csvFilePath = buildCsvFilePath(tp.getStartTime(), i);
                        TextFieldParser parser = initCsvParser(csvFilePath);
                        parseComponentReadings(ref parser, ref tp);
                        tp.averageTempAndLoadData();
                        writeToLog(processName, tp);
                     }
                }
                startMiner();
            }
            else
            {
                Console.WriteLine("Process stopped: {0}", processName);
            }
            
        }
        
        static public void startMiner()
        {
            if(niceHashFilePath == null)
            {
                return;
            }
            Process[] pname = Process.GetProcessesByName("NiceHashQuickMiner");
            if (pname.Length == 0)
            {
                using (Process quickMiner = new Process())
                {
                    quickMiner.StartInfo.UseShellExecute = false;
                    quickMiner.StartInfo.FileName = niceHashFilePath;
                    quickMiner.StartInfo.WorkingDirectory = niceHashFilePath[0..^22];
                    quickMiner.Start();
                }
            }
            else
            {
                Console.WriteLine("NiceHash is already running");
            }
        }

        static public void killMiner()
        {
            if (niceHashFilePath == null)
            {
                return;
            }
            Process[] quickMiner = Process.GetProcessesByName("NiceHashQuickMiner");
            Process[] excavator = Process.GetProcessesByName("excavator");

            quickMiner[0].Close();
            excavator[0].Close();
            foreach (var process in Process.GetProcessesByName("NiceHashQuickMiner"))
            {
                killTheFamily(process.Id);
                //process.CloseMainWindow();
                //process.Kill();
            }
        }

        static void killTheFamily(int pid)
        {
            ManagementObjectSearcher processSearcher = new ManagementObjectSearcher
                    ("Select * From Win32_Process Where ParentProcessID=" + pid);
            ManagementObjectCollection processCollection = processSearcher.Get();

            try
            {
                Process proc = Process.GetProcessById(pid);
                if (!proc.HasExited)
                {
                    proc.Kill();
                }
                    
            }
            catch (ArgumentException)
            {
                // Process already exited.
            }

            if (processCollection != null)
            {
                foreach (ManagementObject manObj in processCollection)
                {
                    killTheFamily(Convert.ToInt32(manObj["ProcessID"])); //recursivley kill the child processes
                }
            }
        }

        static public void startOpenHwMonitor()
        {
            Process[] pname = Process.GetProcessesByName("OpenHardwareMonitor");
            if (pname.Length == 0)
            {
                string openHwMonitorFilePath = Path.Combine(Environment.CurrentDirectory, @"OpenHardwareMonitor\OpenHardwareMonitor.exe");
                Process.Start(openHwMonitorFilePath);
            }
            else
            {
                Console.WriteLine("Open Hardware Monitor is already running");
            }
        }


        static public void parseComponentReadings(ref TextFieldParser parser, ref TargetProcess tp)
        {
            while (!parser.EndOfData)
            {
                String[] row = parser.ReadFields();
                DateTime rowTime = DateTime.ParseExact(row[0], "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                if (rowTime >= tp.getStartTime()) //check for times within current app session
                {
                    if (rowTime > tp.getEndTime())
                    {
                        break;
                    }
                    tp.incrementNumReadings(); //numReadings will be used to calculate averages upon final calculations
                    updateComponentVars(row, ref tp);
                }

            }
        }

        static public void updateComponentVars(string[] row, ref TargetProcess tp)
        {
            int rowCpuTemp = Int32.Parse(row[18]);
            int rowGpuTemp = Int32.Parse(row[35]);
            tp.updateTemperatures(rowCpuTemp, rowGpuTemp);
            double cpuLoad = Convert.ToDouble(row[9]);
            double gpuLoad= Convert.ToDouble(row[39]);
            tp.addToLoadData(cpuLoad, gpuLoad);
        }

        static public TextFieldParser initCsvParser(string csvFilePath)
        {
            TextFieldParser parser = new TextFieldParser(csvFilePath); //Better to store the entire config file into hashMap for easy lookup
            
            
            parser.SetDelimiters(",");
            parser.ReadLine(); //skip first header
            parser.ReadLine(); //skip second header
            return parser;
        }

        static public void parseFilePaths(string configPath)
        {
            TextFieldParser parser = new TextFieldParser(configPath);
            logFilesPath = parser.ReadLine().Substring(12);
            Console.WriteLine("logFilesPath {0}", logFilesPath);

            csvFileDir = parser.ReadLine().Substring(12);
            Console.WriteLine("csvFilesPath {0}", csvFileDir);
            if (!parser.EndOfData)
            {
                niceHashFilePath = parser.ReadLine().Substring(17);
            }
            
            if(niceHashFilePath != null)
            {
                Console.WriteLine("niceHashFilePath {0}", niceHashFilePath);
            }
        }


        static public void writeToLog(string processName, TargetProcess tp)
        {
            string filePath = logFilesPath + @"\" + processName[0..^4] + ".txt";
            tp.writeToLogFile(filePath);
        }

        static public string buildCsvFilePath(DateTime startTime, int i)
        {
            var addDayToStartTime = startTime.AddDays(i);
            return csvFileDir + @"\OpenHardwareMonitorLog-" + addDayToStartTime.ToString("yyyy-MM-dd") + @".csv";
        }



        
    }
}

//find correct index based on core #
//function to find correct process name syntax -> auto add to txt file
