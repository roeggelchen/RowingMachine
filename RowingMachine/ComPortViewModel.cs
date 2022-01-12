using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RowingMachineApp
{
    public class ComPortViewModel : INotifyPropertyChanged
    {
        public List<string> _portNames;
        private ComPort obj;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        public List<string> PortNames
        {
            get { return _portNames; }
        }


        public ComPortViewModel()
        {
            _portNames = new List<string>();
            string[] ports = SerialPort.GetPortNames();
            foreach (string p in ports)
            {
                _portNames.Add(p);
            }
            OnPropertyChanged(new PropertyChangedEventArgs("PortNames"));
        }
    }
}

