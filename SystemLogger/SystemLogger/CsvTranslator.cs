using System;
using System.Collections.Generic;
using System.Text;

namespace SystemLogger
{
    class CsvTranslator
    {
        public int gpuTempCol, cpuTempCol, gpuLoadCol, cpuLoadCol;

        public CsvTranslator(int gpuTempCol, int cpuTempCol, int gpuLoadCol, int cpuLoadCol)
        {
            this.gpuTempCol = gpuTempCol;
            this.cpuTempCol = cpuTempCol;
            this.gpuLoadCol = gpuLoadCol;
            this.cpuLoadCol = cpuLoadCol;
        }


    }
}
