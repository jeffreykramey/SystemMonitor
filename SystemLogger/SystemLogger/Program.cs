using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Management;
using Microsoft.VisualBasic.FileIO;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Linq;

namespace SystemLogger
{
    class Program
    {

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);
        private delegate bool EventHandler(CtrlType sig);
        static EventHandler closureHandler;

        static HashSet<String> appsToWatch = new HashSet<string>(File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, @"appsToWatch.txt")));
        static Dictionary<uint, TargetProcess> runningApps = new Dictionary<uint, TargetProcess>();
        static String csvFileDir, logFilesPath, niceHashFilePath = null;
        static CsvTranslator translator;

        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static bool Handler(CtrlType sig)
        {
            switch (sig)
            {
                case CtrlType.CTRL_C_EVENT:
                    prepareProgramForClose();
                    return true;
                case CtrlType.CTRL_LOGOFF_EVENT:
                    prepareProgramForClose();
                    return true;
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                    prepareProgramForClose();
                    return true;
                case CtrlType.CTRL_CLOSE_EVENT:
                    prepareProgramForClose();
                    return true;
                default:
                    return false;
            }
        }


        static void Main(string[] args)
        {


           

            NotifyIcon trayIcon = new NotifyIcon();
            trayIcon.Icon = new System.Drawing.Icon(Path.Combine(Environment.CurrentDirectory, @"pussyCat.ico"));
            trayIcon.Visible = true;

            string configFilePath = Path.Combine(Environment.CurrentDirectory, @"config.txt");
            Console.WriteLine(configFilePath);
            if (!File.Exists(configFilePath))
            {
                var formProcess = Process.Start(Path.Combine(Environment.CurrentDirectory, @"ConfigInit\ConfigInit.exe"));
                formProcess.WaitForExit();
            }
           
            parseFilePaths(configFilePath);
            logFilesPath = Path.Combine(logFilesPath, "LogFiles");
            Directory.CreateDirectory(logFilesPath); //make a LogFiles dir if it doesn't already exist
            startOpenHwMonitor();
            checkCurrentlyRunningProcesses();
            startMiner();

            

            ManagementScope scope = new ManagementScope(Environment.MachineName + @"\root\cimv2"); 
            scope.Options.EnablePrivileges = true;
            try
            {
                closureHandler += new EventHandler(Handler);
                SetConsoleCtrlHandler(closureHandler, true);
                ConsoleWindow.QuickEditMode(false); //turns console's quick edit mode off

                ManagementEventWatcher startWatch = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
                startWatch.EventArrived += new EventArrivedEventHandler(startWatchingApp);
                startWatch.Start();
                ManagementEventWatcher stopWatch = new ManagementEventWatcher(
                new WqlEventQuery("SELECT * FROM Win32_ProcessStopTrace"));
                stopWatch.EventArrived += new EventArrivedEventHandler(stopWatchingApp);
                stopWatch.Start();
                while (true) System.Threading.Thread.Sleep(50);
                //startWatch.Stop(); //this doesn't seem to be needed
                //stopWatch.Stop();
            }
            catch (ManagementException e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
            }
        }

        public static void checkCurrentlyRunningProcesses()
        {
            Process[] allProcesses = Process.GetProcesses();
            foreach(Process proc in allProcesses)
            {
                if (appsToWatch.Contains(proc.ProcessName + @".exe"))
                {
                    initRunningApps(proc);
                    Console.WriteLine("{0} added to list", proc.ProcessName);
                }
            }
        }

        static void startWatchingApp(object sender, EventArrivedEventArgs e)
        {
            String processName = (String)e.NewEvent.Properties["ProcessName"].Value;
            uint pid =(uint) e.NewEvent.Properties["ProcessID"].Value;

            if (appsToWatch.Contains(processName))
            {
                addProcessToRunningApps(pid, processName);
                killMiner();
            }
            else
            {
                Console.WriteLine("Unwatched process started: {0}, {1}", processName, pid);
            }

        }

        static void initRunningApps(Process proc)
        {
            addProcessToRunningApps((uint) proc.Id, proc.ProcessName + @".exe");
        }

        static void addProcessToRunningApps(uint pid, string processName)
        {
            if (!runningApps.ContainsKey(pid))
            {
                runningApps.Add(pid, new TargetProcess(processName));
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Found Target Process: {0}, {1}", processName, pid);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        static void stopWatchingApp(object sender, EventArrivedEventArgs e)
        {
            String processName = (String)e.NewEvent.Properties["ProcessName"].Value;
            uint pid = (uint)e.NewEvent.Properties["ProcessID"].Value;
            
            if (runningApps.ContainsKey(pid))
            {
                TargetProcess tp = runningApps[pid];
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Killing Target Process: {0}, {1}", processName, pid);
                Console.ForegroundColor = ConsoleColor.White;
                    
                runningApps.Remove(pid);
                processEndOfWatch(tp);
                if(runningApps.Count == 0)
                {
                    Console.WriteLine("Started in stopWarchingApp");
                    startMiner();
                }
                
            }
            else
            {
                Console.WriteLine("Unwatched process stopped: {0}, {1}", processName, pid);
            }
            
        }

        static void processEndOfWatch(TargetProcess tp)
        {
            tp.setEndTime();
            int numDaysLogged = tp.getEndTime().DayOfYear - tp.getStartTime().DayOfYear; //covers cases where monitoring covers multi-day periods    
            for (int i = 0; i <= numDaysLogged; i++)
            {
                string csvFilePath = buildCsvFilePath(tp.getStartTime(), i);
                TextFieldParser parser = initCsvParser(csvFilePath);
                parseComponentReadings(ref parser, ref tp);
                tp.averageTempAndLoadData();
                writeToLog(tp);
            }
        }

        static void prepareProgramForClose()
        {
            var keyList = runningApps.Keys.ToList();
            foreach(var key in keyList)
            {
                processEndOfWatch(runningApps[key]);
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
            int rowCpuTemp = Int32.Parse(row[translator.cpuTempCol]);
            int rowGpuTemp = Int32.Parse(row[translator.gpuTempCol]);
            tp.updateTemperatures(rowCpuTemp, rowGpuTemp);
            double cpuLoad = Convert.ToDouble(row[translator.cpuLoadCol]);
            double gpuLoad= Convert.ToDouble(row[translator.gpuLoadCol]);
            tp.addToLoadData(cpuLoad, gpuLoad);
        }

        static public TextFieldParser initCsvParser(string csvFilePath)
        {
            TextFieldParser parser = new TextFieldParser(csvFilePath);
            parser.SetDelimiters(",");
            if (translator == null)
            {
                setTranslator(parser);
            }
            else
            {
                parser.ReadLine(); //skip first two header rows
                parser.ReadLine();
            }
            return parser;
        }

        static void setTranslator(TextFieldParser parser)
        {
            int gpuTempCol = -1, cpuTempCol = -1, gpuLoadCol = -1, cpuLoadCol = -1;
            string[] coloumnHeaders_1 = parser.ReadFields();
            string[] coloumnHeaders_2 = parser.ReadFields();
            for(int i = 0; i < coloumnHeaders_2.Length; i++)
            {
                if(coloumnHeaders_2[i] == "GPU Core")
                {
                    if (coloumnHeaders_1[i].Contains("temperature"))
                    {
                        gpuTempCol = i;
                        continue;
                    }
                    if (coloumnHeaders_1[i].Contains("load"))
                    {
                        gpuLoadCol = i;
                        continue;
                    }
                }

                if (coloumnHeaders_2[i] == "CPU Package" && coloumnHeaders_1[i].Contains("temperature"))
                {
                    cpuTempCol = i;
                    continue;
                }

                if (coloumnHeaders_2[i] == "CPU Total" && coloumnHeaders_1[i].Contains("load")) 
                {
                    cpuLoadCol = i;
                    continue;
                }
            }
            translator = new CsvTranslator(gpuTempCol, cpuTempCol, gpuLoadCol, cpuLoadCol);
            
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


        static public void writeToLog(TargetProcess tp)
        {
            string filePath = logFilesPath + @"\" +tp.getPorcessName()[0..^4] + ".txt";
            tp.writeToLogFile(filePath);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Wrote log for {0} here: {1}", tp.getPorcessName(), filePath);
            Console.ForegroundColor = ConsoleColor.White;
            
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

