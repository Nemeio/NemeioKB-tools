using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstallTool
{
    class USBSerialPort
    {
        private SerialPort mSerialPort;
        private string mSerialPortName;
        private List<IUSBSerialPortListener> mLstListeners;

        public USBSerialPort(string serialPortName)
        {
            mSerialPortName = serialPortName;
            mSerialPort = new SerialPort();
            mLstListeners = new List<IUSBSerialPortListener>();
        }

        public bool Connect()
        {
            bool bRet = true;

            // Allow the user to set the appropriate properties.
            mSerialPort.PortName = mSerialPortName;
            mSerialPort.BaudRate = 921600; // baudrate is a dummy parameter for CDC ACM
            mSerialPort.Parity = Parity.None;
            mSerialPort.StopBits = StopBits.One;
            mSerialPort.Handshake = Handshake.None;
            mSerialPort.DtrEnable = true;
            mSerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            // Set the read/write timeouts
            mSerialPort.ReadTimeout = 5000;
            mSerialPort.WriteTimeout = 5000;

            Console.WriteLine("Waiting for serial port connection on {0}.", mSerialPortName);
            while (!mSerialPort.IsOpen)
            {
                try
                {
                    mSerialPort.Open();
                }
                catch (Exception)
                {
                    Console.Write(".");
                    System.Threading.Thread.Sleep(1000);
                }
            }
            
            Console.WriteLine("Connected");


            return bRet;
        }

        public void Disconnect()
        {
            try
            {
                mSerialPort.Dispose();
            }
            catch (Exception e)
            {

            }
        }

        public void registerListener(IUSBSerialPortListener listener)
        {
            mLstListeners.Add(listener);
        }


        public void Write(byte[] data)
        {
            mSerialPort.Write(data, 0, data.Length);
        }

        public bool Read(byte[] data)
        {
            int bytesToRead = data.Length;
            bool bRet = true;

            try
            {
                while (bytesToRead != 0)
                {
                    int readBytes = mSerialPort.Read(data, data.Length - bytesToRead, bytesToRead);
                    bytesToRead -= readBytes;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("exception");
                bRet = false;
            }

            return bRet;
        }

        private void DataReceivedHandler(
                        object sender,
                        SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            byte[] data = new byte[sp.BytesToRead];
            sp.Read(data, 0, data.Length);
            foreach (var listener in mLstListeners)
            {
                listener.DataReceived(data);
            }
        }
    }
}
