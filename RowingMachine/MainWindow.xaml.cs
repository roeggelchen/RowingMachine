using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RowingMachineApp
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public RowingMachine myMachine;
        public ComPortViewModel ComPorts = new ComPortViewModel();

        public MainWindow()
        {
            InitializeComponent();

            myMachine = new RowingMachine();
            DataContext = myMachine;
            cbxComPorts.DataContext = ComPorts;


            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://127.0.0.1:8000/rowing/api/v1/rowingsession/");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                //string json = "{\"user\":\"test\"," +
                //              "\"password\":\"bla\"}";
                string json = "{\"com_port\": \"TEst VS\"," +
                                "\"c\": \"C\"," +
                                "\"t\": \"T\"," +
                                "\"version\": \"V\"," +
                                "\"max_level\": 3," +
                                "\"h\": \"H\"," +
                                "\"start_time\": \"2022-01-14T23:49:00+01:00\"," +
                                "\"end_time\": \"2022-01-14T23:49:00+01:00\"}";

                //string json = "{\"com_port\":\"COM_VS\",\"c\":\"C\",\"t\":\"T\",\"version\":\"V\",\"max_level\":3,\"h\":\"H\",\"start_time\":\"2022-01-14T23:29:00+01:00\",\"end_time\":\"2022-01-14T23:46:00+01:00\"}";

                streamWriter.Write(json);
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            myMachine.StartSession();
            grdCurrentStroke.DataContext = myMachine.CurrentSession.CurrentStroke;
            btnStart.IsEnabled = false;
            btnStop.IsEnabled = true;
        }


        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            myMachine.StopSession();
            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            myMachine.Connect(cbxComPorts.SelectedItem.ToString());
            if(myMachine.IsConnected)
            {
                btnConnect.IsEnabled = false;
                btnLvlDown.IsEnabled = true;
                btnLvlUp.IsEnabled = true;
                btnStart.IsEnabled = true;
            }
        }

        private void cbxComPorts_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            //geht noch nicht
            //if(cbxComPorts.ItemsSource.GetType() )
            cbxComPorts.SelectedItem = 1;
        }

        private void btnLvlDown_Click(object sender, RoutedEventArgs e)
        {
            myMachine.Level = Int32.Parse(lblLevel.Content.ToString()) - 1;
        }

        private void btnLvlUp_Click(object sender, RoutedEventArgs e)
        {
            myMachine.Level = Int32.Parse(lblLevel.Content.ToString()) + 1;
        }
    }
}
