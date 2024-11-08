using System;
using System.Threading;
using System.Linq;
using System.Timers;
using System.Net.NetworkInformation;
using System.Diagnostics;


namespace Task_Scheduler_Console
{
    internal class Program
    {
        
        static void Main(string[] args)
        {

            var bandwithThread = new Thread(Bandwith_Track);
            var memoryThread = new Thread(Memory_Track);
            var cpuThread = new Thread(CPU_Track);
            bandwithThread.Start();
            memoryThread.Start();
            cpuThread.Start();

            
        }

        
        private static void CPU_Track()
        {
            PerformanceCounter cpuUsageCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            PerformanceCounter cpuIdleCounter = new PerformanceCounter("Processor", "% Idle Time", "_Total");
            PerformanceCounter cpuInterruptsCounter = new PerformanceCounter("Processor", "Interrupts/sec", "_Total");

            while (true)
            {
                float cpuUsage = cpuUsageCounter.NextValue();
                float cpuIdle = cpuIdleCounter.NextValue();
                float cpuInterrupts = cpuInterruptsCounter.NextValue();

                Console.WriteLine("CPU Usage: {0}% ||| CPU Idle: {1}% ||| CPU Interrupts/sec: {2}", cpuUsage, cpuIdle, cpuInterrupts);

                Thread.Sleep(1000);
            }
        }





        private static void Memory_Track()
        {

            PerformanceCounter availableMemoryCounter = new PerformanceCounter("Memory", "Available MBytes");
            PerformanceCounter committedBytesCounter = new PerformanceCounter("Memory", "Committed Bytes");

            while (true)
            {
                float availableMemory = availableMemoryCounter.NextValue();
                float committedMemory = committedBytesCounter.NextValue();

                float committedMemoryGB = committedMemory / (1024 * 1024 * 1024); 

               
                Console.WriteLine("Available Memory: {0} MB || Committed Memory: {1} GB", availableMemory, committedMemoryGB);

                
                Thread.Sleep(1000);
            }
        }





        private static void Bandwith_Track()
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();

            var oldBytesRec = 0L;
            var oldBytesSent = 0L;
            var startTime = DateTime.Now;

            while (true) { 

                var newBytesRec = 0L;
                var newBytesSent = 0L;
                DateTime currentTime = DateTime.Now;
                TimeSpan elapsedTime = currentTime - startTime;

                foreach (var ni in interfaces)
                {

                    if (ni.Name.Contains("Ethernet") && ni.OperationalStatus == OperationalStatus.Up)
                    {
                        var stats = ni.GetIPv4Statistics();
                        newBytesSent = stats.BytesSent;
                        newBytesRec = stats.BytesReceived;

                    }


                }

                    double sendSpeed = 0;
                double recSpeed = 0;
                if (elapsedTime.TotalSeconds > 0) {

                    sendSpeed = (newBytesSent - oldBytesSent) * 8 / (elapsedTime.TotalSeconds * 1_000_000.0);
                    recSpeed = (newBytesRec - oldBytesRec) * 8 / (elapsedTime.TotalSeconds * 1_000_000.0);
                }




                
                Console.WriteLine("Upload Speed: {0} Mb/s ||| Download Speed: {1} Mb/s", sendSpeed.ToString("0.00"), recSpeed.ToString("0.00"));
                oldBytesSent = newBytesSent;
                oldBytesRec = newBytesRec;
                startTime = DateTime.Now;
                Thread.Sleep(1_000);
            }
            
        }
    }
}
