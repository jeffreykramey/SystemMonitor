using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using Microsoft.VisualBasic.FileIO;
using System.Diagnostics;
using System.Globalization;

namespace systemLogger
{
    class Program
    {
        static DateTime startTime, endTime;
       // static String currApp;
        static HashSet<String> appsToWatch = new HashSet<string>(File.ReadAllLines(@"C:\Users\jeffr\Code\systemLogger\systemLogger\appsToWatch.txt"));
        static HashSet<String> runningApps = new HashSet<string>();
        static String csvFileDir, logFilesPath, niceHashFilePath;

        



        static void Main(string[] args)
        {
            string configFilePath = Path.Combine(Environment.CurrentDirectory, @"LogFiles\config.txt");
            if (!File.Exists(configFilePath))
            {
                var formProcess = Process.Start(Path.Combine(Environment.CurrentDirectory, @"configInit\configInit.exe"));
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
                startWatch.EventArrived += new EventArrivedEventHandler(startMonitoringProcess);
                startWatch.Start();
                ManagementEventWatcher stopWatch = new ManagementEventWatcher(
                new WqlEventQuery("SELECT * FROM Win32_ProcessStopTrace"));
                stopWatch.EventArrived += new EventArrivedEventHandler(stopMonitoringProcess);
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

        static void startMonitoringProcess(object sender, EventArrivedEventArgs e)
        {
            String processName = (String)e.NewEvent.Properties["ProcessName"].Value;
            if (appsToWatch.Contains(processName))
            {
                runningApps.Add(processName);
                //currApp = processName;
                startTime = DateTime.Now;
                Console.WriteLine("Found Target: {0}", processName);
            }
            Console.WriteLine("Process started: {0}", processName);

        }

        static void stopMonitoringProcess(object sender, EventArrivedEventArgs e)
        {
            String processName = (String)e.NewEvent.Properties["ProcessName"].Value;
            //if (currApp == processName)
            if(appsToWatch.Contains(processName))
            {
                Console.WriteLine("Process stopped: {0}", processName);
                appsToWatch.Remove(processName);
                //processName = null;
                endTime = DateTime.Now;
                int numDaysLogged = endTime.DayOfYear - startTime.DayOfYear; //covers cases where monitoring covers multi-day periods    
                for(int i = 0; i <= numDaysLogged; i++)
                {
                    double cpuLoadAvg = 0, gpuLoadAvg = 0, readingCount = 0, cpuMax = 0, cpuTempAvg = 0, gpuMax = 0, gpuTempAvg = 0;
                    string csvFilePath = buildCsvFilePath(startTime, i);
                    TextFieldParser parser = initializeParser(csvFilePath);
                    parseComponentReadings(ref parser, readingCount, ref cpuMax, ref cpuTempAvg, ref cpuLoadAvg, ref gpuMax, ref gpuTempAvg, ref gpuLoadAvg);
                    
                    finalizeComponentDataCalculations(readingCount, ref cpuTempAvg, ref cpuLoadAvg, ref gpuTempAvg, ref gpuLoadAvg);
                    var sessionLength = (endTime - startTime);
                    writeToLog(processName, cpuMax, cpuTempAvg, cpuLoadAvg, gpuMax, gpuTempAvg, gpuLoadAvg, sessionLength);


                }

            }
        }
        
        static public void startMiner()
        {
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


        static public void parseComponentReadings(ref TextFieldParser parser, double readingCount, ref double cpuMax, ref double cpuTempAvg, ref double cpuLoadAvg, ref double gpuMax, ref double gpuTempAvg, ref double gpuLoadAvg)
        {
            while (!parser.EndOfData)
            {
                String[] row = parser.ReadFields();
                DateTime rowTime = DateTime.ParseExact(row[0], "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                if (rowTime >= startTime) //check for times within current app session
                {
                    if (rowTime > endTime)
                    {
                        break;
                    }
                    readingCount++;
                    updateComponentVars(row, ref cpuMax, ref cpuTempAvg, ref cpuLoadAvg, ref gpuMax, ref gpuTempAvg, ref gpuLoadAvg);
                }

            }
        }


        static public TextFieldParser initializeParser(string csvFilePath)
        {
            TextFieldParser parser = new TextFieldParser(csvFilePath);
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
         
            niceHashFilePath = parser.ReadLine().Substring(17);
            if(niceHashFilePath != null)
            {
                Console.WriteLine("niceHashFilePath {0}", niceHashFilePath);
            }
            
            
            
        }


        static public void writeToLog(string currApp, double cpuMax, double cpuTempAvg, double cpuLoadAvg, double gpuMax, double gpuTempAvg, double gpuLoadAvg, TimeSpan sessionLength)
        {
            string filePath = logFilesPath + @"\" + currApp[0..^4] + ".txt";
            using (StreamWriter writer = File.AppendText(filePath))
            {
                writer.WriteLine("{0} -- CPU Max Temp: {1}; CPU Avg Temp: {2}; CPU Avg Load: {3}%; GPU Max Temp: {4}; GPU Avg Temp: {5}; GPU Avg Load: {6}%; Session Length: {7} hours, {8} minutes",
                endTime.ToShortDateString(), cpuMax, Math.Round(cpuTempAvg, 0), Math.Round(cpuLoadAvg, 2), gpuMax, Math.Round(gpuTempAvg, 0), Math.Round(gpuLoadAvg, 2), sessionLength.Hours, sessionLength.Minutes);
            }
        }
        static public string buildCsvFilePath(DateTime startTime, int i)
        {
            var addDayToStartTime = startTime.AddDays(i);
            return csvFileDir + @"\OpenHardwareMonitorLog-" + addDayToStartTime.ToString("yyyy-MM-dd") + @".csv";
        }

        static public void updateComponentVars(string[] row, ref double cpuMax, ref double cpuTempAvg, ref double cpuLoadAvg, ref double gpuMax, ref double gpuTempAvg, ref double gpuLoadAvg)
        {
            int rowCpuTemp = Int32.Parse(row[18]);
            cpuMax = Math.Max(cpuMax, rowCpuTemp);
            cpuTempAvg += rowCpuTemp;
            cpuLoadAvg += Convert.ToDouble(row[9]);
            int rowGpuTemp = Int32.Parse(row[35]);
            gpuMax = Math.Max(gpuMax, rowGpuTemp);
            gpuTempAvg += rowGpuTemp;
            gpuLoadAvg += Convert.ToDouble(row[39]);
        }

        //getting infinity for calculations, maybe need to add each reading to an array for each process?
        static public void finalizeComponentDataCalculations(double readingCount, ref double cpuTempAvg, ref double cpuLoadAvg, ref double gpuTempAvg, ref double gpuLoadAvg)
        {
            cpuTempAvg /= readingCount;
            cpuLoadAvg /= readingCount;
            gpuTempAvg /= readingCount;
            gpuLoadAvg /= readingCount;
        }
    }
}

//find correct index based on core #
//function to find correct process name syntax -> auto add to txt file

