using System;
using System.IO;

namespace SystemLogger
{
    public class TargetProcess //represents one of the classes in the apps watch list
    {
        string processName;
        double cpuLoadAvg = 0, cpuTempMax = 0, cpuTempAvg = 0;
        double gpuLoadAvg = 0, gpuTempMax = 0, gpuTempAvg = 0;
        double numReadings = 0;
        DateTime startTime, endTime;
        TimeSpan sessionLength;

        public TargetProcess(string processName)
        {
            this.processName = processName;
            startTime = DateTime.Now;
        }

        public DateTime getStartTime()
        {
            return startTime;
        }

        public DateTime getEndTime()
        {
            return endTime;
        }

        public string getPorcessName()
        {
            return processName;
        }

        public void setEndTime()
        {
            endTime = DateTime.Now;
            sessionLength = endTime - startTime;
        }

        public void incrementNumReadings()
        {
            numReadings++;
        }

        public void updateTemperatures(int otherCpuTemp, int otherGpuTemp)
        {
            cpuTempMax = Math.Max(cpuTempMax, otherCpuTemp);
            cpuTempAvg += otherCpuTemp;
            gpuTempMax = Math.Max(gpuTempMax, otherGpuTemp);
            gpuTempAvg += otherGpuTemp;
        }

        public void addToLoadData(double cpuLoad, double gpuLoad)
        {
            cpuLoadAvg += cpuLoad;
            gpuLoadAvg += gpuLoad;
        }

        public void averageTempAndLoadData()
        {
            cpuTempAvg /= numReadings;
            cpuLoadAvg /= numReadings;
            gpuTempAvg /= numReadings;
            gpuLoadAvg /= numReadings;
        }


        public void writeToLogFile(string filePath)
        {
            using (StreamWriter writer = File.AppendText(filePath))
            {
                writer.WriteLine("{0} -- CPU Max Temp: {1}; CPU Avg Temp: {2}; CPU Avg Load: {3}%; GPU Max Temp: {4}; GPU Avg Temp: {5}; GPU Avg Load: {6}%; Session Length: {7} hours, {8} minutes",
                endTime.ToShortDateString(), cpuTempMax, Math.Round(cpuTempAvg, 0), Math.Round(cpuLoadAvg, 2), gpuTempMax, Math.Round(gpuTempAvg, 0), Math.Round(gpuLoadAvg, 2), sessionLength.Hours, sessionLength.Minutes);
            }
        }



   
    }
}
