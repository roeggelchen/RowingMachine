using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RowingMachineApp
{
    public class RowingStroke : INotifyPropertyChanged
    {
        private int _t;
        public int T
        {
            get { return _t; }
        }

        private TimeSpan _duration;
        public TimeSpan Duration
        {
            get { return _duration; }
        }

        private int _distance;
        public int Distance
        {
            get { return _distance; }
        }

        private int _one;
        public int One
        {
            get { return _one; }
        }

        private TimeSpan _speed;
        public TimeSpan Speed
        {
            get { return _speed; }
        }

        private int _strokesPerMinute;
        public int StrokesPerMinute
        {
            get { return _strokesPerMinute; }
        }

        private int _watts;
        public int Watts
        {
            get { return _watts; }
        }
        private int _calories;
        public int Calories
        {
            get { return _calories; }
        }

        private int _zero;
        public int Zero
        {
            get { return _zero; }
        }

        private int _level;
        public int Level
        {
            get { return _level; }
        }
        private string _message;
        public string Message
        {
            get { return _message; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }
        public void UpdateStrokeFromMessage(string fromMessage)
        {
            _message = fromMessage;

            if (fromMessage.Length >= 28)
            {
                _t = Int32.Parse(fromMessage.Substring(1, 1));

                OnPropertyChanged(new PropertyChangedEventArgs("T"));
                //_duration
                int h = Int32.Parse(fromMessage.Substring(2, 1));
                int mm = Int32.Parse(fromMessage.Substring(3, 2));
                int ss = Int32.Parse(fromMessage.Substring(5, 2));
                _duration = new TimeSpan(h, mm, ss);
                OnPropertyChanged(new PropertyChangedEventArgs("Duration"));
                _distance = Int32.Parse(fromMessage.Substring(7, 5));
                OnPropertyChanged(new PropertyChangedEventArgs("Distance"));
                _one = Int32.Parse(fromMessage.Substring(12, 1));
                OnPropertyChanged(new PropertyChangedEventArgs("One"));
                mm = Int32.Parse(fromMessage.Substring(13, 2));
                ss = Int32.Parse(fromMessage.Substring(15, 2));
                _speed = new TimeSpan(0, mm, ss);
                //_speed = Int32.Parse(fromMessage.Substring(13, 4));
                OnPropertyChanged(new PropertyChangedEventArgs("Speed"));
                _strokesPerMinute = Int32.Parse(fromMessage.Substring(17, 3));
                OnPropertyChanged(new PropertyChangedEventArgs("StrokesPerMinute"));
                _watts = Int32.Parse(fromMessage.Substring(20, 3));
                OnPropertyChanged(new PropertyChangedEventArgs("Watts"));
                _calories = Int32.Parse(fromMessage.Substring(23, 4));
                OnPropertyChanged(new PropertyChangedEventArgs("Calories"));
                _zero = Int32.Parse(fromMessage.Substring(27, 1));
                OnPropertyChanged(new PropertyChangedEventArgs("Zero"));
                _level = Int32.Parse(fromMessage.Substring(28, 1));
                OnPropertyChanged(new PropertyChangedEventArgs("Level"));
            }
        }

        public RowingStroke(string fromMessage)
        {
            UpdateStrokeFromMessage(fromMessage);
        }
    }
}
