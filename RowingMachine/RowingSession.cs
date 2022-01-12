using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
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

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        public RowingSession()
        {
            outputCSV = new StreamWriter("Session_" + DateTime.Now.ToString("u", CultureInfo.CreateSpecificCulture("de-DE")).Replace(':', '-').Replace('Z', '_') + ".csv");
            outputCSV.WriteLine("T" + Seperator + "Duration" + Seperator + "Distance" + Seperator + "One" + Seperator + "Speed" + Seperator + "StrokesPerMinute" + Seperator + "Watts" + Seperator + "CaloriesPerHour" + Seperator + "Zero" + Seperator + "Level" + Seperator + "Message");

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

        }
    }
}
