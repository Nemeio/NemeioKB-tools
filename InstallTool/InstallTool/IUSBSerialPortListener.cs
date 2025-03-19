using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstallTool
{
    interface IUSBSerialPortListener
    {
        void DataReceived(byte[] data);
    }
}
