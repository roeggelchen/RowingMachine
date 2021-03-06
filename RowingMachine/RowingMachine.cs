using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RowingMachineApp
{
    public class RowingMachine : INotifyPropertyChanged
    {
        //Attribute
        private string _comPort;
        public string ComPort
        {
            get { return _comPort; }
        }

        private int _c;
        public int C
        {
            get { return _c; }
        }

        private int _t;
        public int T
        {
            get { return _t; }
        }

        private string _version;
        public string Version
        {
            get { return _version; }
        }

        private int _level;
        public int Level
        {
            get { return _level; }
            set
            {
                if (value > 0 && value <= _maxLevel)
                {
                    //_level = value;
                    //Send L

                    machineSerial.WriteLine("L" + value);
                    _receivedLine = false;
                    while (!_receivedLine) { }
                    //_level = value;

                }
            }
        }

        private int _maxLevel;
        public int MaxLevel
        {
            get { return _maxLevel; }
        }

        private int _h;
        public int H
        {
            get { return _h; }
        }

        public bool IsConnected
        {
            get { return machineSerial != null && machineSerial.IsOpen; }
        }

        private RowingSession _currentSession;
        public RowingSession CurrentSession
        {
            get { return _currentSession; }
        }

        //private Variables 
        SerialPort machineSerial;

        //DB
        HttpWebRequest httpWebRequest;


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        //bool received = false;

        public int Connect(string comPort)
        {
            _comPort = comPort;


            machineSerial = new SerialPort(_comPort);

            machineSerial.BaudRate = 9600;
            machineSerial.Parity = Parity.None;
            machineSerial.StopBits = StopBits.One;
            machineSerial.DataBits = 8;
            machineSerial.Handshake = Handshake.None;
            machineSerial.RtsEnable = true;

            machineSerial.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            machineSerial.Open();

            //DB
            //httpWebRequest = (HttpWebRequest)WebRequest.Create("http://192.168.178.100:8000/rowing/api/v1/rowingsession/");
            //httpWebRequest.ContentType = "application/json";
            //httpWebRequest.Method = "POST";

            if (IsConnected)
            {
                machineSerial.WriteLine("C");
                while (!_receivedLine) { }
                _receivedLine = false;
                machineSerial.WriteLine("T");
                while (!_receivedLine) { }
                _receivedLine = false;
                machineSerial.WriteLine("V");
                while (!_receivedLine) { }
                _receivedLine = false;
                machineSerial.WriteLine("L");
                while (!_receivedLine) { }
                _receivedLine = false;
                machineSerial.WriteLine("H");
                while (!_receivedLine) { }
                _receivedLine = false;
                machineSerial.WriteLine("R");
                while (!_receivedLine) { }
                _receivedLine = false;

                OnPropertyChanged(new PropertyChangedEventArgs("IsConnected"));
                return 0;
            }
            else { return 1; }
        }

        public bool StartSession()
        {
            if (IsConnected)
            {
                _currentSession = new RowingSession();

                var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://192.168.178.100:8000/rowing/api/v1/rowingsession/");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    //string json = "{\"com_port\": \"" + _comPort + "\"," +
                    //                "\"c\": \"" + _c + "\"," +
                    //                "\"t\": \"" + _t + "\"," +
                    //                "\"version\": \"" + _version + "\"," +
                    //                "\"max_level\": \"" + _maxLevel + "\"," +
                    //                "\"h\": \"" + _h + "\"," +
                    //                "\"start_time\": \"" + DateTime.Now.ToString("yyyy-mm-ddTHH:mm:sszzz") + "\"," +
                    //                "\"end_time\": \"" + DateTime.Now.ToString("yyyy-mm-ddTHH:mm:sszzz") + "\"}";
                    //string blub = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");
                    string json =   "{\"user\": \"" + _currentSession.User + "\"," +
                                    "\"com_port\": \"" + _comPort + "\"," +
                                    "\"c\": \"" + _c + "\"," +
                                    "\"t\": \"" + _t + "\"," +
                                    "\"version\": \"" + _version + "\"," +
                                    "\"max_level\": \"" + _maxLevel + "\"," +
                                    "\"h\": \"" + _h + "\"," +
                                    "\"distance\": \"" + _currentSession.CurrentDistance + "\"," +
                                    "\"start_time\": \"" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz") + "\"," +
                                    "\"end_time\": \"" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz") + "\"}";

                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    //string json = "{ \"Atlantic/Canary\": \"GMT Standard Time\", \"Europe/Lisbon\": \"GMT Standard Time\", \"Antarctica/Mawson\": \"West Asia Standard Time\", \"Etc/GMT+3\": \"SA Eastern Standard Time\", \"Etc/GMT+2\": \"UTC-02\", \"Etc/GMT+1\": \"Cape Verde Standard Time\", \"Etc/GMT+7\": \"US Mountain Standard Time\", \"Etc/GMT+6\": \"Central America Standard Time\", \"Etc/GMT+5\": \"SA Pacific Standard Time\", \"Etc/GMT+4\": \"SA Western Standard Time\", \"Pacific/Wallis\": \"UTC+12\", \"Europe/Skopje\": \"Central European Standard Time\", \"America/Coral_Harbour\": \"SA Pacific Standard Time\", \"Asia/Dhaka\": \"Bangladesh Standard Time\", \"America/St_Lucia\": \"SA Western Standard Time\", \"Asia/Kashgar\": \"China Standard Time\", \"America/Phoenix\": \"US Mountain Standard Time\", \"Asia/Kuwait\": \"Arab Standard Time\" }";
                    var data = (JObject)JsonConvert.DeserializeObject(result);
                    int id = data["id"].Value<int>();
                    CurrentSession.SQLSessionID = id;
                }

                machineSerial.WriteLine("R");
                return true;
            }
            else { return false; }
        }

        public bool StopSession()
        {
            if (IsConnected && _currentSession != null)
            {

                _currentSession.StopSession();
                _currentSession = null;

                machineSerial.WriteLine("R");
                return true;
            }
            else { return false; }
        }

        bool _receivedLine = false;

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            //string indata = sp.ReadExisting();
            //txtInput.Text += indata;
            //Console.WriteLine("Data Received:");
            //Console.WriteLine(indata);
            //received = true;
            bool waitForLine = true;
            while (waitForLine)
            {
                try
                {
                    string message = sp.ReadLine();

                    Console.WriteLine("Line Received:");
                    Console.WriteLine(message);
                    waitForLine = false;

                    char input = message[0];
                    if(input == 'C' || input == 'c')
                    {
                        _c = Int32.Parse(message.Substring(1));
                        _maxLevel = Int32.Parse(message.Substring(4,1));
                        OnPropertyChanged(new PropertyChangedEventArgs("C"));
                    }
                    else if (input == 'T' || input == 't')
                    {
                        _t = Int32.Parse(message.Substring(1));
                        OnPropertyChanged(new PropertyChangedEventArgs("T"));
                    }
                    else if (input == 'L' || input == 'l')
                    {
                        _level = Int32.Parse(message.Substring(1));
                        OnPropertyChanged(new PropertyChangedEventArgs("Level"));
                    }
                    else if (input == 'H' || input == 'h')
                    {
                        _h = Int32.Parse(message.Substring(1));
                        OnPropertyChanged(new PropertyChangedEventArgs("H"));
                    }
                    else if (input == 'V' || input == 'v')
                    {
                        _version = "Version " +
                            message.Substring(1, 1) + "." +
                            message.Substring(2, 1) + "." +
                            message.Substring(3, 1) + " (" +
                            message.Substring(4, 2) + "/20" +
                            message.Substring(6, 2) + ")";
                        OnPropertyChanged(new PropertyChangedEventArgs("Version"));
                    }
                    else if (input == 'A' || input == 'a')
                    {

                        if(_currentSession != null && message.Length >= 28)
                        {
                            _currentSession.AddStroke(new RowingStroke(message));
                        }
                    }
                    _receivedLine = true;
                }
                catch (TimeoutException) { }
            }
        }

    }

}
