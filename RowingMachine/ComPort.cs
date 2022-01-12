using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RowingMachineApp
{
    public class ComPort
    {
        private int _comportID;
        private string _comportName;
        private long _baud;
        private int _dataBits;
        private StopBits _stopBits;
        private Parity _parity;
        public int ComportID
        {
            get { return _comportID; }
            set { _comportID = value; }
        }

        public string ComportName
        {
            get { return _comportName; }
            set { _comportName = value; }
        }
    }
}
