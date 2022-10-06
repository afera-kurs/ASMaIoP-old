using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace ASMaIoP.General.Client
{
    /*
            serialPort = new SerialPort();
            serialPort.PortName = "COM4";
            serialPort.BaudRate = 9600;
            serialPort.Open();
            while (true)
     */

    public static class ArduinoApplicationAPI
    {
        static SerialPort CurrentArduionoPort = null;
        static Thread FindThread = null;
        static bool IsFind = false;
        static bool ShutdownFindThread = false;

        private static void Finder()
        {
            while (true)
            {
                if (ShutdownFindThread) return;
                if (CurrentArduionoPort.BytesToRead > 0)
                {
                    if (CurrentArduionoPort.ReadLine() == "ASMaIoP_DEVICE_0")
                    {
                        CurrentArduionoPort.Write("ASMaIoP_APPLCATION_1");
                        IsFind = true;
                        return;
                    }
                }
            }
        }

        public static bool StartFindArduino()
        {
            CurrentArduionoPort = new SerialPort();
            CurrentArduionoPort.PortName = "COM3";
            CurrentArduionoPort.BaudRate = 9600;
            CurrentArduionoPort.Open();

            // test arduino protocol
            FindThread = new Thread(Finder);
            FindThread.Start();
            return true;
        }

        public static bool GetFindValue()
        {
            return IsFind;
        }

        public static void ShutdownFinder()
        {
            ShutdownFindThread = true;
            FindThread.Join();
        }
    }
}
