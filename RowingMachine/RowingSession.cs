using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RowingMachineApp
{
    public class RowingSession : INotifyPropertyChanged
    {
        public static string Seperator = ";";
        private StreamWriter outputCSV;

        private List<RowingStroke> _strokes;

        private RowingStroke _currentStroke;

        public RowingStroke CurrentStroke
        {
            get { return _currentStroke; }
        }

        private int _sqlSessionID;
        public int SQLSessionID
        {
            get { return _sqlSessionID; }
            set { _sqlSessionID = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }


        //DB
        //HttpWebRequest _httpWebRequest;
        //StreamWriter _httpStreamWriter;

        public RowingSession()
        {
            outputCSV = new StreamWriter("Session_" + DateTime.Now.ToString("u", CultureInfo.CreateSpecificCulture("de-DE")).Replace(':', '-').Replace('Z', '_') + ".csv");
            outputCSV.WriteLine("T" + Seperator + "Duration" + Seperator + "Distance" + Seperator + "One" + Seperator + "Speed" + Seperator + "StrokesPerMinute" + Seperator + "Watts" + Seperator + "CaloriesPerHour" + Seperator + "Zero" + Seperator + "Level" + Seperator + "Message");

            //_httpWebRequest = (HttpWebRequest)WebRequest.Create("http://127.0.0.1:8000/rowing/api/v1/rowingstroke/");
            //_httpWebRequest.ContentType = "application/json";
            //_httpWebRequest.Method = "POST";

            //_httpStreamWriter = new StreamWriter(_httpWebRequest.GetRequestStream());
            //_httpStreamWriter.AutoFlush = true;

            _strokes = new List<RowingStroke>();
            _currentStroke = new RowingStroke("A000000000000000000000000000000");
            //AddStroke(_currentStroke);
        }

        public void StopSession()
        {
            outputCSV.Dispose();
        }

        public void AddStroke(RowingStroke stroke)
        {
            _currentStroke.UpdateStrokeFromMessage(stroke.Message);
            _strokes.Add(stroke);
            OnPropertyChanged(new PropertyChangedEventArgs("CurrentStroke"));
            outputCSV.WriteLine(stroke.T + Seperator + stroke.Duration + Seperator + stroke.Distance + Seperator + stroke.One + Seperator + stroke.Speed + Seperator + stroke.StrokesPerMinute + Seperator + stroke.Watts + Seperator + stroke.Calories + Seperator + stroke.Zero + Seperator + stroke.Level + Seperator + stroke.Message.Substring(0,29));

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://127.0.0.1:8000/rowing/api/v1/rowingstroke/");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            //_httpStreamWriter = new StreamWriter(_httpWebRequest.GetRequestStream());

            //sql DB
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{\"rowing_session\": \"" + _sqlSessionID + "\"," +
                                    "\"t\": \"" + stroke.T + "\"," +
                                    "\"distance\": \"" + stroke.Distance + "\"," +
                                    "\"one\": \"" + stroke.One + "\"," +
                                    "\"speed\": \"" + stroke.Speed + "\"," +
                                    "\"strokes_per_minute\": \"" + stroke.StrokesPerMinute + "\"," +
                                    "\"watts\": \"" + stroke.Watts + "\"," +
                                    "\"colories_per_hour\": \"" + stroke.Calories + "\"," +
                                    "\"zero\": \"" + stroke.Zero + "\"," +
                                    "\"level\": \"" + stroke.Level + "\"," +
                                    "\"message\": \"" + stroke.Message.Replace("\n", "").Replace("\r", "") + "\"," +
                                    "\"time_stamp\": \"" + stroke.TimeStamp.ToString("yyyy-MM-ddTHH:mm:sszzz") + "\"}";

                //string json = "{\"rowing_session\": 12,\"t\": \"T\",\"distance\": 4,\"one\": 0,\"speed\": \"05:05:05\",\"strokes_per_minute\": 4,\"watts\": 23,\"colories_per_hour\": 65,\"zero\": 0,\"level\": 1,\"message\": \"Jip\",\"time_stamp\": \"2022-01-15T02:11:00+01:00\"}";

                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                //string json = "{ \"Atlantic/Canary\": \"GMT Standard Time\", \"Europe/Lisbon\": \"GMT Standard Time\", \"Antarctica/Mawson\": \"West Asia Standard Time\", \"Etc/GMT+3\": \"SA Eastern Standard Time\", \"Etc/GMT+2\": \"UTC-02\", \"Etc/GMT+1\": \"Cape Verde Standard Time\", \"Etc/GMT+7\": \"US Mountain Standard Time\", \"Etc/GMT+6\": \"Central America Standard Time\", \"Etc/GMT+5\": \"SA Pacific Standard Time\", \"Etc/GMT+4\": \"SA Western Standard Time\", \"Pacific/Wallis\": \"UTC+12\", \"Europe/Skopje\": \"Central European Standard Time\", \"America/Coral_Harbour\": \"SA Pacific Standard Time\", \"Asia/Dhaka\": \"Bangladesh Standard Time\", \"America/St_Lucia\": \"SA Western Standard Time\", \"Asia/Kashgar\": \"China Standard Time\", \"America/Phoenix\": \"US Mountain Standard Time\", \"Asia/Kuwait\": \"Arab Standard Time\" }";
                //var data = (JObject)JsonConvert.DeserializeObject(result);
                //int id = data["id"].Value<int>();
                //CurrentSession.SQLSessionID = id;
            }
        }
    }
}
